using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

namespace Movements.XR.HoloLens
{
    public class MVInputHandManager
    {
        Hand[] hands;

        public int HandCount
        {
            get
            {
                int result = 0;
                foreach (Hand hand in hands)
                {
                    if (hand != null) result++;
                }
                return result;
            }
        }
        public int ClickCount
        {
            get
            {
                int result = 0;
                foreach (Hand hand in hands)
                {
                    if (hand != null)
                    {
                        switch (hand.HandClickState)
                        {
                            case Hand.ClickState.Down:
                            case Hand.ClickState.Clicking:
                            case Hand.ClickState.Up:
                                result++;
                                break;
                        }
                    }
                }
                return result;
            }
        }
        public Hand GetHand(HandSide handSide)
        {
            return hands[(int)handSide];
        }
        public Hand[] Hands { get { return hands; } }



        public void Initialize()
        {
            handID = 0;
            hands = new Hand[2];
        }

        public void OnInputDown(InputEventData eventData)
        {
            Hand hand = hands[HandIdx(eventData.Handedness)];
            if (hand == null)
            {
            }
            else
            {
                hand.SetClick(Hand.ClickState.Down);
            }
        }
        public void OnInputUp(InputEventData eventData)
        {
            Hand hand = hands[HandIdx(eventData.Handedness)];
            if (hand == null)
            {
            }
            else
            {
                hand.SetClick(Hand.ClickState.Up);
                hand.RaiseHandClickUp();
            }
        }

        public void OnInputChanged(InputEventData<MixedRealityPose> eventData)
        {
            Handedness handedness = eventData.Handedness;
            Hand hand = hands[HandIdx(handedness)];
            if (hand == null) return;
            Pose pose = new Pose(eventData.InputData.Position, eventData.InputData.Rotation);
            hand.SetChangedPose(pose);

            //디버깅
            if (eventData.Handedness == Handedness.Right && eventData.MixedRealityInputAction.AxisConstraint == AxisType.SixDof)
            {
                GameObject.Find("DebugShow")?.GetComponent<DebugShow>().DebugInputs(eventData);
            }
        }
        /// <summary>
        /// Click State에 따라 threshold 뒤의 Click State Update 반영
        /// </summary>
        void HandStateUpdate()
        {
            foreach (Hand hand in hands)
            {
                if (hand == null) continue;
                switch (hand.HandClickState)
                {
                    case Hand.ClickState.Down:

                        //Down 후 HoldStartDuration 까지 Up이 아닌 경우 "Hold" InputAction 
                        if (hand.LastDownTime + MVInput.HoldStartDuration > Time.time) continue;
                        hand.SetClick(Hand.ClickState.Clicking);
                        hand.NowActions.Add(new MVInputAction("Hold", Time.frameCount, Time.time));


                        GameObject.FindObjectOfType<DebugShow>().debugActionTxt.text += Time.frameCount + "Hold";


                        break;
                    case Hand.ClickState.Up:
                        //Up 후 이전 상태에 따라 처리
                            switch (hand.StateStack.Pop())
                            {
                                case Hand.ClickState.Down:
                                //Down 후 Hold로 상태 변화 전에 UP일 시 "Click" InputAction 
                                if (hand.LastDownTime + MVInput.HoldStartDuration > Time.time)
                                {
                                    hand.NowActions.Add(new MVInputAction("Click", Time.frameCount, Time.time));



                                    GameObject.FindObjectOfType<DebugShow>().debugActionTxt.text += Time.frameCount + "Click";

                                }
                                    break;
                                case Hand.ClickState.Clicking:
                                //이전 상태 Clicking (InputAction Hold) 인 경우 Hold 종료
                                    hand.NowActions.Clear();

                                GameObject.FindObjectOfType<DebugShow>().debugActionTxt.text += Time.frameCount + "HoldOUT";
                               
                                break;
                            }
                        
                        hand.SetClick(Hand.ClickState.None);
                        break;
                }
               
                for(int i = 0; i < hand.NowActions.Count; i++)
                {
                    //한 프레임 내에서 Click 액션 종료 배제
                    if (hand.NowActions[i].CreatedFrameCount == Time.frameCount) continue;
                    if (hand.NowActions[i].Description == "Click")
                    {
                        //Click 액션 (Like MRTK에서 Select 액션, 일반적인 경우 GetDown())은 한 프레임 후 종료 
                        hand.NowActions.RemoveAt(i);

                        GameObject.FindObjectOfType<DebugShow>().debugActionTxt.text += Time.frameCount + "ClickOUT";
                    }
                } 
            }
        }
    
            /// <summary>
            /// 손 인식 확인
            /// </summary>
            void HandOnCheck()
        {
            Handedness handedness = Handedness.Left;
            for (int i = 0; i < 2; i++)
            {
                if (i == 1) handedness = Handedness.Right;

                Hand hand = hands[HandIdx(handedness)];
                
                if (IsHandOn(handedness))
                {
                    // 인식된 손에 따라 Hand생성
                    if (hand == null)
                    {
                        hands[HandIdx(handedness)] = new Hand(ProvideHandID(), handedness);
                        Debug.Log(handedness+"핸드 생성"+hands[HandIdx(handedness)].ID);
                    }
                }
                else
                {
                    //인식이 안 된 손의 Hand 제거
                    //Hand가 클릭 상태일 때는 Up 이벤트를 Raise하고 제거 
                    if (hand != null)
                    {
                        Debug.Log(handedness + "핸드 제거" + hand.ID + "클릭스테이트" + hand.HandClickState);
                        switch (hand.HandClickState)
                        {
                            case Hand.ClickState.None:
                                hands[HandIdx(handedness)] = null;
                                break;
                            case Hand.ClickState.Down:
                                hand.SetClick(Hand.ClickState.Up);
                                hand.RaiseHandClickUp();
                                hands[HandIdx(handedness)] = null;
                                break;
                            case Hand.ClickState.Clicking:
                                hand.SetClick(Hand.ClickState.Up);
                                hand.RaiseHandClickUp();
                                hands[HandIdx(handedness)] = null;
                                break;
                            //OnInputUp에서 이미 Click Up Event Raise 발생했으므로 hand.RaiseHandClickUp(); 생략
                            case Hand.ClickState.Up:
                                hands[HandIdx(handedness)] = null;
                                break;
                        }
                    }
                }
            }
        }

        public void OnLateUpdate()
        {
            HandOnCheck();
            HandStateUpdate();
        }


        int handID;
        int ProvideHandID()
        {
            return handID++;
        }


        int HandIdx(Microsoft.MixedReality.Toolkit.Utilities.Handedness handedness)
        {
            switch (handedness)
            {
                case Handedness.Left:
                    return (int)Hand.ConvertHand(Handedness.Left);
                case Handedness.Right:
                    return (int)Hand.ConvertHand(Handedness.Right);
                case Handedness.None:
                case Handedness.Both:
                case Handedness.Other:
                case Handedness.Any:
                    break;
            }
            return 0;
        }

        bool IsHandOn(Handedness handedness)
        {
            Ray ray;
            return InputRayUtils.TryGetHandRay(handedness, out ray);
        }
        
    }
}