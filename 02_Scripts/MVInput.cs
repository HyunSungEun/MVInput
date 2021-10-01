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

        static int savingFrames;
        static int handFrameCursor;
        static Hand[] hands;
        static GameObject monoTosser;

        public static bool IsInitialized
        {
            get
            {
                return (monoTosser != null && hands != null);
            }
        }

        static void Initialize()
        {
            if (monoTosser == null)
            {
                monoTosser = new GameObject("MVInput_MonoTosser");
                monoTosser.AddComponent<MVInputMonoTosser>();
            }
            hands = new Hand[(int)HandSide.Max];

            for (int j = 0; j < (int)HandSide.Max; j++)
            {
                hands[j] = new Hand();
            }

            handFrameCursor = 0;
        }
        /*
        static int GetHandCursor(bool next,int step, int now)
        {
            if (next)
            {
                int result=now;

                for (int i = 0; i < step; i++)
                {
                    result=(result + 1 >= savingFrames) ? 0 : (result + 1);
                }
                return result;
            }
            else
            {
                int result = now;

                for (int i = 0; i < step; i++)
                {
                    result = (result - 1 < 0) ? savingFrames - 1 : (result - 1);
                }
                return result;
            }
        }
        public static void MoveFrameCursor()
        {
            handFrameCursor = GetHandCursor(true, 1, handFrameCursor);
        }

        public static int HandCount
        {
            get
            {
                Ray ray;
                int result = 0;
                if (InputRayUtils.TryGetHandRay(Handedness.Left, out ray)) result++;
                if (InputRayUtils.TryGetHandRay(Handedness.Right, out ray)) result++;
                return result;
            }
        }
        public static int ClickCount
        {
            get
            {
                int result = 0;

                foreach (var controller in CoreServices.InputSystem.DetectedControllers)
                {
                    if(controller.Enabled && controller.InputSource.SourceType== InputSourceType.Hand)
                    {
                        IMixedRealityPointer[] dpointers = controller.InputSource.Pointers; //컨트롤러의 포인터목록
                        //dpointers[0].

                        var pointers = new HashSet<IMixedRealityPointer>();

                        // Find all valid pointers
                        foreach (var inputSource in CoreServices.InputSystem.DetectedInputSources)
                        {
                            foreach (var pointer in inputSource.Pointers)
                            {
                                if (pointer.IsInteractionEnabled && !pointers.Contains(pointer))
                                {
                                    pointers.Add(pointer);
                                }
                            }
                        }





                        if (controller.ControllerHandedness == Handedness.Left)
                        {
                            foreach (MixedRealityInteractionMapping inputMapping in controller.Interactions)
                            {
                                if (inputMapping.InputType== DeviceInputType.Select && inputMapping.BoolData)
                                {
                                    result++;
                                }
                            }
                        }
                        else if (controller.ControllerHandedness == Handedness.Right)
                        {
                            foreach (MixedRealityInteractionMapping inputMapping in controller.Interactions)
                            {
                                if (inputMapping.InputType == DeviceInputType.Select && inputMapping.BoolData)
                                {
                                    result++;
                                }
                            }
                        }
                    }
                }
                return result;
            }
        }

        private Tuple<InputSourceType, Handedness>[] inputSources = new Tuple<InputSourceType, Handedness>[]
       {
          //  new Tuple<InputSourceType, Handedness>(InputSourceType.Eyes, Handedness.Any) ,
         //   new Tuple<InputSourceType, Handedness>(InputSourceType.Head, Handedness.Any) ,
            new Tuple<InputSourceType, Handedness>(InputSourceType.Hand, Handedness.Left) ,
            new Tuple<InputSourceType, Handedness>(InputSourceType.Hand, Handedness.Right)
       };


        Hand GetHand(HandSide handSide)
        {
            return new Hand();
        }
        static void SetHandsForNextFrame()
        {
            for(int i = 0; i < (int)HandSide.Max; i++)
            {
                hands[handFrameCursor, i] = new Hand();//false로 초기화 하기.
            }
            foreach (var controller in CoreServices.InputSystem.DetectedControllers)
            {
                if (controller.Enabled && controller.InputSource.SourceType == InputSourceType.Hand&& controller.TrackingState== TrackingState.Tracked)
                {
                    if(controller.ControllerHandedness== Handedness.Left)
                    {
                        hands[handFrameCursor, (int)HandSide.Left] = new Hand(); // 디텍팅으로만 초기화
                    }
                    else if(controller.ControllerHandedness == Handedness.Right)
                    {
                        hands[handFrameCursor, (int)HandSide.Right] = new Hand();
                    }
                }
            }
        }
      */
        public static void Update()
        {
            if (!IsInitialized) return;
            StringBuilder sb = new StringBuilder();
            if (hands[(int)HandSide.Left] != null)
            {
                Hand hand = hands[(int)HandSide.Left];
                sb.Append("Left_"+hand.clickState.ToString());

            }
            

            //SetHandsForNextFrame();
        }

        public static void OnInputChanged(InputEventData<MixedRealityPose> eventData)
        {
            if (eventData.InputSource.SourceType != InputSourceType.Hand) return;
            Hand hand = hands[GetHandIdx(eventData.Handedness)];
            Pose pose = new Pose(eventData.InputData.Position, eventData.InputData.Rotation);
            if (hand == null) { 
                hand = new Hand();
                hand.SetClick(Hand.ClickState.Down, pose);
            }

            hand.SetClick(Hand.ClickState.Clicking, pose);
            
        }

        public static void OnInputDown(InputEventData eventData)
        {
            if (eventData.InputSource.SourceType != InputSourceType.Hand) return;
            Debug.Log(eventData.SourceId + "OnInputDown");

            Hand hand = hands[GetHandIdx(eventData.Handedness)];
            IMixedRealityPointer activePointer=null;
            foreach (IMixedRealityPointer pointer  in eventData.InputSource.Pointers)
            {
                if (pointer.IsActive) { activePointer = pointer; break; }
            }
            Pose pose = new Pose(activePointer.Position, activePointer.Rotation);

            if (hand == null)
            {
                hand = new Hand();
            }

            hand.SetClick(Hand.ClickState.Down, pose);
        }

        public static void OnInputUp(InputEventData eventData)
        {
            if (eventData.InputSource.SourceType != InputSourceType.Hand) return;
            Debug.Log(eventData.SourceId + "OnInputUp");


            Hand hand = hands[GetHandIdx(eventData.Handedness)];

            IMixedRealityPointer activePointer = null;
            foreach (IMixedRealityPointer pointer in eventData.InputSource.Pointers)
            {
                if (pointer.IsActive) { activePointer = pointer; break; }
            }
            Pose pose = new Pose(activePointer.Position, activePointer.Rotation);


            if (hand == null)
            {
                hand = new Hand();
                hand.SetClick(Hand.ClickState.Down, pose);
            }

            hand.SetClick(Hand.ClickState.Up, pose);
        }


        public static void OnSourceDetected(SourceStateEventData eventData)
        {
            Debug.Log(eventData.Controller.TrackingState.ToString() + "OnSourceDetected");
            if (hands[GetHandIdx(eventData.Controller.ControllerHandedness)] == null)
            {
                hands[GetHandIdx(eventData.Controller.ControllerHandedness)] = new Hand();
            }
        }

        public static void OnSourceLost(SourceStateEventData eventData)
        {
            Debug.Log("OnSourceLost");
            hands[GetHandIdx(eventData.Controller.ControllerHandedness)] = null;
        }

        static int GetHandIdx(Handedness hand) { return (int)Hand.ConvertHand(hand); }



        string getDebugString(InputEventData e)
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
