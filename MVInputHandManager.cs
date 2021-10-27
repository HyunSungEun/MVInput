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

            //�����
            if (eventData.Handedness == Handedness.Right && eventData.MixedRealityInputAction.AxisConstraint == AxisType.SixDof)
            {
                GameObject.Find("DebugShow")?.GetComponent<DebugShow>().DebugInputs(eventData);
            }
        }
        /// <summary>
        /// Click State�� ���� threshold ���� Click State Update �ݿ�
        /// </summary>
        void HandStateUpdate()
        {
            foreach (Hand hand in hands)
            {
                if (hand == null) continue;
                switch (hand.HandClickState)
                {
                    case Hand.ClickState.Down:

                        //Down �� HoldStartDuration ���� Up�� �ƴ� ��� "Hold" InputAction 
                        if (hand.LastDownTime + MVInput.HoldStartDuration > Time.time) continue;
                        hand.SetClick(Hand.ClickState.Clicking);
                        hand.NowActions.Add(new MVInputAction("Hold", Time.frameCount, Time.time));


                        GameObject.FindObjectOfType<DebugShow>().debugActionTxt.text += Time.frameCount + "Hold";


                        break;
                    case Hand.ClickState.Up:
                        //Up �� ���� ���¿� ���� ó��
                            switch (hand.StateStack.Pop())
                            {
                                case Hand.ClickState.Down:
                                //Down �� Hold�� ���� ��ȭ ���� UP�� �� "Click" InputAction 
                                if (hand.LastDownTime + MVInput.HoldStartDuration > Time.time)
                                {
                                    hand.NowActions.Add(new MVInputAction("Click", Time.frameCount, Time.time));



                                    GameObject.FindObjectOfType<DebugShow>().debugActionTxt.text += Time.frameCount + "Click";

                                }
                                    break;
                                case Hand.ClickState.Clicking:
                                //���� ���� Clicking (InputAction Hold) �� ��� Hold ����
                                    hand.NowActions.Clear();

                                GameObject.FindObjectOfType<DebugShow>().debugActionTxt.text += Time.frameCount + "HoldOUT";
                               
                                break;
                            }
                        
                        hand.SetClick(Hand.ClickState.None);
                        break;
                }
               
                for(int i = 0; i < hand.NowActions.Count; i++)
                {
                    //�� ������ ������ Click �׼� ���� ����
                    if (hand.NowActions[i].CreatedFrameCount == Time.frameCount) continue;
                    if (hand.NowActions[i].Description == "Click")
                    {
                        //Click �׼� (Like MRTK���� Select �׼�, �Ϲ����� ��� GetDown())�� �� ������ �� ���� 
                        hand.NowActions.RemoveAt(i);

                        GameObject.FindObjectOfType<DebugShow>().debugActionTxt.text += Time.frameCount + "ClickOUT";
                    }
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