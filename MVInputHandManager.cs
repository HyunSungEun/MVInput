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
                foreach(Hand hand in hands)
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
                        switch (hand.clickState)
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




        public void Initialize()
        {
            handID = 0;
            hands = new Hand[2];
        }

        public void OnInputDown(InputEventData eventData)
        {
            Hand hand = GetHandByHandedness(eventData.Handedness);
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
            Hand hand = GetHandByHandedness(eventData.Handedness);
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
            Hand hand = GetHandByHandedness(handedness);
            if (hand == null) return;
            Pose pose = new Pose(eventData.InputData.Position,eventData.InputData.Rotation);
            hand.SetChangedPose(pose);
        }

        void HandStateUpdate()
        {
            foreach(Hand hand in hands)
            {
                if (hand == null) continue;
                //한 프레임의 LateUpdate에서 바로 State 업데이트 배제
                if (hand.LastClickChangedFrameCount == Time.frameCount) continue;
                switch (hand.clickState)
                {
                    case Hand.ClickState.Down:
                        hand.SetClick(Hand.ClickState.Clicking);
                        break;
                    case Hand.ClickState.Up:
                        hand.SetClick(Hand.ClickState.None);
                        break;
                }
            }
        }

        void HandOnCheck()
        {
            Handedness handedness = Handedness.Left;
            for (int i = 0; i < 2; i++)
            {
                if (i == 1) handedness = Handedness.Right;

                Hand hand = GetHandByHandedness(handedness);
                if (IsHandOn(handedness))
                {
                    if (hand == null)
                    {
                      //  hand = new Hand(ProvideHandID());
                      //  Debug.Log(handedness+"핸드 생성" + hand.ID+"핸즈"+hands[0].ID);
                       if(handedness == Handedness.Right)
                        {
                            hands[1] = new Hand(ProvideHandID());
                            Debug.Log(handedness + "핸드 생성" + "핸즈" + hands[1]?.ID);
                        }
                    }
                }
                else
                {
                    if (hand != null)
                    {
                        Debug.Log("제거 dkdlel="+hand.ID);
                        switch (hand.clickState)
                        {
                            case Hand.ClickState.None:
                                Debug.Log(handedness + "핸드 제거" + hand.ID+"클릭스테이트"+ hand.clickState);
                                if (handedness == Handedness.Right)
                                {
                                    hands[1] = null;
                                }
                                break;
                            case Hand.ClickState.Down:
                                hand.RaiseHandClickUp();
                                hand = null;
                                break;
                            case Hand.ClickState.Clicking:
                                hand.RaiseHandClickUp();
                                hand = null;
                                break;
                            case Hand.ClickState.Up:
                                Debug.Log(handedness + "핸드 제거" + hand.ID + "클릭스테이트" + hand.clickState);
                                if (handedness == Handedness.Right)
                                {
                                    hands[1] = null;
                                }
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


        Hand GetHandByHandedness(Microsoft.MixedReality.Toolkit.Utilities.Handedness handedness)
        {
            switch (handedness)
            {
                case Handedness.Left:
                    return hands[(int)Hand.ConvertHand(Handedness.Left)];
                case Handedness.Right:
                    return hands[(int)Hand.ConvertHand(Handedness.Right)];
                case Handedness.None:
                case Handedness.Both:
                case Handedness.Other:
                case Handedness.Any:
                    break;
            }
            return null;
        }

        bool IsHandOn(Handedness handedness)
        {
            Ray ray;
            return InputRayUtils.TryGetHandRay(handedness, out ray);
        }
    }
}