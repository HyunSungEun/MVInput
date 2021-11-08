using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if WINDOWS_UWP
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
#endif
using System;


namespace Movements.XR.Input {
    public class MVInput
    { 
        public enum MVInputMode
        {
            None,
            TouchController,
            HandTracking,
            TouchScreen,
            MAX
        }
        public class InteractableService
        {

            public static float InteractableFindRayMaxLength = 10f;
            //핸드매니저에서 이벤트 raise타이밍 받아와서 raise를 시켜줌
            public static void RaiseOnInteractableStartedEvent(Hand hand, MVInputAction action)
            {
                if (hand == null) return;
                IMVInteractableEventHandler handler = GetMVInteractableEventHandlerByHandRay(hand);
                if (handler == null) return;
                
                if (action.Description == "MVHold" && handler.IsHoldAcceptable() == false) return;

                handler.OnInteractableStarted(action, new MVInteractableEventData(hand));
                hand.BeHoldingHandler = handler;
            }
            public static void RaiseOnInteractableDraggedEvent(Hand hand, MVInputAction action)
            {
                if (hand == null) return;
                if (hand.BeHoldingHandler == null) return;
                hand.BeHoldingHandler.OnInteractableDragged(action, new MVInteractableEventData<Pose>(new Pose(), hand));
            }
            public static void RaiseOnInteractableCompletedEvent(Hand hand, MVInputAction action)
            {
                if (hand == null) return;
                if (action.Description == "MVHold")
                {
                    if (hand.BeHoldingHandler == null) return;
                    hand.BeHoldingHandler.OnInteractableCompleted(action, new MVInteractableEventData(hand));
                    hand.BeHoldingHandler = null;
                    return;
                }
                IMVInteractableEventHandler handler = GetMVInteractableEventHandlerByHandRay(hand);
                if (handler == null) return;
                handler.OnInteractableCompleted(action, new MVInteractableEventData(hand));
            }
            public static void RaiseOnInteractableCanceledEvent(Hand hand, MVInputAction action)
            {
                if (hand == null) return;
                if (action.Description == "MVHold" && hand.BeHoldingHandler != null )
                {
                    hand.BeHoldingHandler.OnInteractableCanceled(action, new MVInteractableEventData(hand));
                }
                hand.BeHoldingHandler = null;
            }

            static IMVInteractableEventHandler GetMVInteractableEventHandlerByHandRay(Hand hand)
            {
                if (hand == null) return null;
                RaycastHit hit;
                Ray ray = GetHandRay(hand);
                bool raycast =  Physics.Raycast(ray, out hit, InteractableFindRayMaxLength);

                if (raycast) {
                    GameObject g = hit.collider.gameObject;
                    if (g == null) Debug.Log("게임오브젝트 부재");
                    else Debug.Log($"MYLOG: 레이캐스트성공 {g.name}_핸들러가 있는가?{g.GetComponent<IMVInteractableEventHandler>() != null}_힛포인트{hit.point.V3Str()}");
                        }

                return hit.collider.gameObject.GetComponent<IMVInteractableEventHandler>();
            }

            public static Ray GetHandRay(Hand hand)
            {
                if (hand == null) return default(Ray);
                Ray ray = new Ray(hand.NowPose.position + hand.NowPose.forward * 0.05f, hand.NowPose.forward);
                return ray;
            } 
        }

        static MVInputMode inputMode;
        public static MVInputMode InputMode { get { return inputMode; } }


        public static readonly float HoldStartDuration = 0.5f;//MRTK MixedRealityInputSimulationProfile.HoldStartDuration 프로퍼티 참고
        static GameObject monoTosser;
        static MVInputHandManager handManager;
        /// <summary>
        /// 인식된 손의 개수
        /// </summary>
        public static int HandCount { get { if (!IsInitialized) return 0; return handManager.HandCount; } }
        /// <summary>
        /// 클릭 처리 중인 손의 개수
        /// </summary>
        public static int ClickCount { get { if (!IsInitialized) return 0; return handManager.ClickCount; } }
        public static Hand GetHand(HandSide handSide) 
        {
            if (!IsInitialized) return null;
            return handManager.GetHand(handSide);
        }
        public static Hand[] Hands { get { if (!IsInitialized) return null; return handManager.Hands; } }


#if UNITY_ANDROID && OCULUS_QUEST_2
        static OVRHand[] ovrHands;
        public static OVRHand[] OVRHands { get { return ovrHands; } }

        static Transform trackingSpace;
        public static Transform TrackingSpace { get { return trackingSpace; } }
#endif

        static bool enableCondition;
        
        public static bool IsInitialized
        {
            get
            {
                return enableCondition && (monoTosser != null);
            }
        }

        public static void Initialize(
#if UNITY_ANDROID && OCULUS_QUEST_2
            OVRHand[] ovrHands, Transform ts
#endif
            )
        {
#if UNITY_ANDROID && (OCULUS_QUEST_2 == false)
            inputMode = MVInputMode.TouchScreen;
            enableCondition = false;
            Debug.Log("MYLOG: Platform is Android TouchScreen. Use UnityEngine.Input as inputsystem instead.");
            return;
#endif
            if (monoTosser == null)
            {
                monoTosser = new GameObject("MVInput_MonoTosser");
                monoTosser.AddComponent<MVInputMonoTosser>();
#if UNITY_ANDROID && OCULUS_QUEST_2
                MVInput.ovrHands = ovrHands;
                MVInput.trackingSpace = ts;
#endif
            }
            handManager = new MVInputHandManager();
            enableCondition = true;
            inputMode = MVInputMode.HandTracking;
        }

        public static void DeInitialize()
        {
            enableCondition = false;
            if(handManager!=null) handManager.Destruct();
            handManager = null;
            GameObject.Destroy(monoTosser.gameObject);
            monoTosser = null;
#if  UNITY_ANDROID && OCULUS_QUEST_2
            inputMode = MVInputMode.TouchController;
#elif WINDOWS_UWP
            inputMode = MVInputMode.None;
#endif
        }




        public static void Update()
        {
            if (!IsInitialized) return;
           
        }
        public static void LateUpdate()
        {
            if (!IsInitialized) return;
            handManager.OnLateUpdate();
        }
        public static void OnPreCull()
        {
        }




        public static void OnInputChanged(MVInputEventData<Pose> eventData)
        {
            if (!IsInitialized) return;
            Hand hand = Hands[(int)eventData.HandSide];
            if (hand == null) return;
            //pose에 변화가 없으면 이벤트 생략
            if (hand.NowPose == eventData.InputData) return;
            handManager.OnInputChanged(eventData);
        }


      
        /// <summary>
        /// Pinch 시 Called by MRTK 
        /// </summary>
        /// <param name="eventData"></param>
      
        public static void OnInputDown(MVInputEventData eventData)
        {
            if (!IsInitialized) return;
            handManager.OnInputDown(eventData);
        }


        /// <summary>
        /// Pinch 후 종료 시  Called by MRTK 
        /// </summary>
        /// <param name="eventData"></param>
     

        public static void OnInputUp(MVInputEventData eventData)
        {
            if (!IsInitialized) return;
            handManager.OnInputUp(eventData);
        }


        public static bool IsHandOn(HandSide handSide)
        {
            bool result = false;

#if WINDOWS_UWP
            Ray ray;
            Handedness handedness = Handedness.None;
            switch (handSide)
            {
                case HandSide.Left:
                    handedness = Handedness.Left;
                    break;
                case HandSide.Right:
                    handedness = Handedness.Right;
                    break;
                case HandSide.None:
                case HandSide.Max:
                    return false;
            }

            result = InputRayUtils.TryGetHandRay(handedness, out ray);


#elif UNITY_ANDROID && OCULUS_QUEST_2
            OVRHand ovrHand = OVRHands[(int)handSide];
            result =  ovrHand.IsDataHighConfidence && ovrHand.IsDataValid && ovrHand.IsPointerPoseValid && ovrHand.IsTracked && ovrHand.isActiveAndEnabled;
#endif

            return result;
        }
#if WINDOWS_UWP
        public static HandSide ConvertToHandSide(Handedness handedness)
        {
            switch (handedness)
            {
                case Handedness.Left:
                    return HandSide.Left;
                case Handedness.Right:
                    return HandSide.Right;

                default:
                    return HandSide.None;
            }
        }
#endif
    }
}
