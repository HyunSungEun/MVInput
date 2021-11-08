using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if WINDOWS_UWP
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
#endif
namespace Movements.XR.Input
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



        public MVInputHandManager()
        {
            handID = 0;
            hands = new Hand[2];
        }
        /// <summary>
        /// hand manager ���� �� ȣ�� �ʿ�
        /// </summary>
        public void Destruct()
        {
            foreach(Hand hand in hands)
            {
                if (hand == null) continue;

                foreach (MVInputAction action in hand.NowActions)
                {
                    MVInput.InteractableService.RaiseOnInteractableCanceledEvent(hand, action);
                }
            }
            hands = null;
        }

        public void OnInputDown(MVInputEventData eventData)
        {
            if (hands == null) return;
            Hand hand = hands[HandIdx(eventData.HandSide)];
            if (hand == null)
            {
            }
            else
            {
                hand.SetClick(Hand.ClickState.Down);
            }
        }
        public void OnInputUp(MVInputEventData eventData)
        {
            if (hands == null) return;
            Hand hand = hands[HandIdx(eventData.HandSide)];
            if (hand == null)
            {
            }
            else
            {
                hand.SetClick(Hand.ClickState.Up);
            }
        }

        public void OnInputChanged(MVInputEventData<Pose> eventData)
        {
            if (hands == null) return;
            HandSide handedness = eventData.HandSide;
            Hand hand = hands[HandIdx(handedness)];
            if (hand == null) return;
            Pose pose = eventData.InputData;
            Pose formerPose = hand.NowPose;
            hand.SetChangedPose(pose);
            if (pose == formerPose) return;
            foreach(MVInputAction action in hand.NowActions)
            {
                if (action.Description =="MVHold")
                {
                    MVInput.InteractableService.RaiseOnInteractableDraggedEvent(hand, action);
                }
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

                        //Down �� HoldStartDuration ���� Up�� �ƴ� ��� "MVHold" InputAction 
                        if (hand.LastDownTime + MVInput.HoldStartDuration > Time.time) continue;
                        hand.SetClick(Hand.ClickState.Clicking);

                        MVInputAction action = new MVInputAction("MVHold");
                        hand.NowActions.Add(action);

                        //Ȧ�� ����

                        MVInput.InteractableService.RaiseOnInteractableStartedEvent(hand, action);

                        Debug.Log($"MYLOG: {hand.HandSide.ToString()}_Hold Start_{Time.frameCount}");

                        break;
                    case Hand.ClickState.Up:
                        //Up �� ���� ���¿� ���� ó��

                        if(hand.StateStack==null|| hand.StateStack.Count==0 )
                        {
                            hand.NowActions.Clear();
                            hand.BeHoldingHandler = null;
                            hand.SetClick(Hand.ClickState.None);
                            break;
                        }

                            switch (hand.StateStack.Pop())
                            {
                                case Hand.ClickState.Down:
                                //Down �� Hold�� ���� ��ȭ ���� UP�� �� "MVClick" InputAction 
                                if (hand.LastDownTime + MVInput.HoldStartDuration > Time.time)
                                {
                                    MVInputAction clickAction = new MVInputAction("MVClick");
                                    hand.NowActions.Add(clickAction);

                                    MVInput.InteractableService.RaiseOnInteractableCompletedEvent(hand, clickAction);

                                    Debug.Log($"MYLOG: {hand.HandSide.ToString()}_CLICK Complete_{Time.frameCount}");
                                }
                                    break;
                                case Hand.ClickState.Clicking:
                                //���� ���� Clicking (InputAction Hold) �� ��� Hold ����

                                //Ȧ�� �ǴϽ�

                                for(int i = 0; i < hand.NowActions.Count; i++)
                                {
                                    if(hand.NowActions[i].Description == "MVHold")
                                    {
                                        MVInputAction holdAction = hand.NowActions[i];
                                        MVInput.InteractableService.RaiseOnInteractableCompletedEvent(hand, holdAction);
                                        hand.NowActions.RemoveAt(i);
                                        Debug.Log($"MYLOG: {hand.HandSide.ToString()}_Hold COMPLETE_{Time.frameCount}");
                                    }
                                }
                                break;
                            }
                        hand.StateStack.Clear();
                        hand.SetClick(Hand.ClickState.None);
                        break;
                }

                int n = 0;
                while (n < hand.NowActions.Count)
                {
                    if (hand.NowActions[n].CreatedFrameCount == Time.frameCount)
                    {
                        n++;
                        continue; 
                    }
                    if (hand.NowActions[n].Description == "MVClick")
                    {

                        Debug.Log($"MYLOG: {hand.HandSide.ToString()}_CLICK DELETE_{Time.frameCount}");

                        //Click �׼� (Like MRTK���� Select �׼�, �Ϲ����� ��� GetDown())�� �� ������ �� ���� 
                        hand.NowActions.RemoveAt(n);
                        //Ŭ�� �׼� �������� (�ݹ� ���ʿ�)
                    }
                    else n++;
                }
            }
        }
    
            /// <summary>
            /// �� �ν� Ȯ��
            /// </summary>
        void HandOnCheck()
        {
            HandSide handedness = HandSide.Left;
            for (int i = 0; i < 2; i++)
            {
                if (i == 1) handedness = HandSide.Right;

                Hand hand = hands[HandIdx(handedness)];
                
                if (MVInput.IsHandOn(handedness))
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
                    //OnInteractableCanceled Raise �� ����
                    if (hand != null)
                    {
                        Debug.Log(handedness + "�ڵ� ����" + hand.ID + "Ŭ��������Ʈ" + hand.HandClickState);

                        foreach(MVInputAction action in hand.NowActions)
                        {
                            MVInput.InteractableService.RaiseOnInteractableCanceledEvent(hand, action);
                        }
                        hands[HandIdx(handedness)] = null;
                    }
                }
            }
        }

        public void OnLateUpdate()
        {
            if (hands == null) return;
            HandOnCheck();
            HandStateUpdate();
        }


        int handID;
        int ProvideHandID()
        {
            return handID++;
        }


        int HandIdx(HandSide handedness)
        {
            switch (handedness)
            {
                case HandSide.Left:
                    return (int)HandSide.Left;
                case HandSide.Right:
                    return (int)HandSide.Right;
                case HandSide.None:
                    break;
                case HandSide.Max:
                    break;
            }
            return 0;
        }

    }
}