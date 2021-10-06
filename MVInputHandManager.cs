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
            Pose pose = new Pose(eventData.InputData.Position,eventData.InputData.Rotation);
            hand.SetChangedPose(pose);

            //�����
            if(eventData.Handedness== Handedness.Right && eventData.MixedRealityInputAction.AxisConstraint == AxisType.SixDof)
            {
                GameObject.Find("DebugShow")?.GetComponent<DebugShow>().DebugInputs(eventData);
            }
        }
        /// <summary>
        /// Click State�� ���� �� ������ ���� Click State Update �ݿ�
        /// </summary>
        void HandStateUpdate()
        {
            foreach(Hand hand in hands)
            {
                if (hand == null) continue;
                //�� �������� LateUpdate���� �ٷ� State ������Ʈ ����
                //handler callback�� LateUpdate���� ������ ������ frameCount�� ���� �����ӳ����� ���� ����
                if (hand.LastClickChangedFrameCount == Time.frameCount) continue;
                switch (hand.HandClickState)
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
        /// <summary>
        /// �� �ν� Ȯ��
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
                    // �νĵ� �տ� ���� Hand����
                    if (hand == null)
                    {
                        hands[HandIdx(handedness)] = new Hand(ProvideHandID(), handedness);
                        Debug.Log(handedness+"�ڵ� ����"+hands[HandIdx(handedness)].ID);
                    }
                }
                else
                {
                    //�ν��� �� �� ���� Hand ����
                    //Hand�� Ŭ�� ������ ���� Up �̺�Ʈ�� Raise�ϰ� ���� 
                    if (hand != null)
                    {
                        Debug.Log(handedness + "�ڵ� ����" + hand.ID + "Ŭ��������Ʈ" + hand.HandClickState);
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
                            //OnInputUp���� �̹� Click Up Event Raise �߻������Ƿ� hand.RaiseHandClickUp(); ����
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