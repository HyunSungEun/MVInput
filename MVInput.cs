using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.OpenXR;
//using Microsoft.MixedReality.OpenXR;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit;
using System;
using System.Text;


namespace Movements.XR.HoloLens {
    public class MVInput : UnityEngine.Input
    {
        static GameObject monoTosser;
        static MVInputHandManager handManager;
        /// <summary>
        /// 인식된 손의 개수
        /// </summary>
        public static int HandCount { get { return handManager.HandCount; } }
        /// <summary>
        /// 클릭 처리 중인 손의 개수
        /// </summary>
        public static int ClickCount { get { return handManager.ClickCount; } }
        public static Hand GetHand(HandSide handSide) => handManager.GetHand(handSide);
        public static Hand[] Hands { get { return handManager.Hands; } }

        public static bool IsInitialized
        {
            get
            {
                return (monoTosser != null);
            }
        }

        public static void Initialize()
        {
            if (monoTosser == null)
            {
                monoTosser = new GameObject("MVInput_MonoTosser");
                monoTosser.AddComponent<MVInputMonoTosser>();
            }
            handManager = new MVInputHandManager();
            handManager.Initialize();
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







        public static void OnInputChanged(InputEventData<MixedRealityPose> eventData)
        {
            if (IsSubjectInputEventData(eventData) == false) return;
            //if((eventData.InputData.Position - Vector3.zero).magnitude >0f) Debug.Log(Time.frameCount + "OnInputChanged" + eventData.Handedness +"_"+ eventData.MixedRealityInputAction.Description + "_" + eventData.MixedRealityInputAction.AxisConstraint.ToString() + "디버그" + getDebugString(eventData));
            //Input Action Pointer Pose의 Pose를 핸드 Pose에 적용
            if (eventData.MixedRealityInputAction.Description != "Pointer Pose") return;
            handManager.OnInputChanged(eventData);
        }

        public static void OnInputDown(InputEventData eventData)
        {
            if (IsSubjectInputEventData(eventData) == false) return;
            Debug.Log(Time.frameCount + "OnInputDown"+eventData.Handedness + eventData.MixedRealityInputAction.Description+eventData.MixedRealityInputAction.AxisConstraint.ToString()+"디버그"+ getDebugString(eventData));
            //Input Action Select를 클릭 상태에 적용
            if (eventData.MixedRealityInputAction.Description != "Select") return;
            handManager.OnInputDown(eventData);
        }

        public static void OnInputUp(InputEventData eventData)
        {
            if (IsSubjectInputEventData(eventData) == false) return;
            Debug.Log(Time.frameCount + "OnInputUp" + eventData.Handedness + eventData.MixedRealityInputAction.Description + eventData.MixedRealityInputAction.AxisConstraint.ToString() + "디버그" + getDebugString(eventData));

            if (eventData.MixedRealityInputAction.Description != "Select") return;
            handManager.OnInputUp(eventData);
        }

        /// <summary>
        /// MVInput에서 InputEventData 조건 확인 후 HandManager Call
        /// </summary>
        /// <param name="eventData"></param>
        /// <returns></returns>
        static bool IsSubjectInputEventData(InputEventData eventData)
        {
            if (eventData.InputSource.SourceType != InputSourceType.Hand) return false;
            if (!(eventData.Handedness == Handedness.Left || eventData.Handedness == Handedness.Right)) return false;
            return true;
        }

        static string getDebugString(InputEventData e)
        {
            UnityEngine.EventSystems.BaseInputModule baseInputModule =  e.currentInputModule;
            DateTime d = e.EventTime;
            Handedness hand =  e.Handedness;
            IMixedRealityInputSource s =  e.InputSource;
           
            MixedRealityInputAction action =  e.MixedRealityInputAction;

            return string.Format("Time_{0},{1},baseName_{2},sId_{3},sname{4},sType{5},actionDesc_{6},actionId_{7},actionAxis_{8}", d, hand.ToString(), baseInputModule.name, s.SourceId, s.SourceName, s.SourceType
                , action.Description, action.Id, action.AxisConstraint.ToString());
        }
    }
}
