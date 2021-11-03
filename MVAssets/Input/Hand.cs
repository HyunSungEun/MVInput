using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Movements.XR.Input
{
    public class Hand
    {
        public Hand(int handID,HandSide handSide)
        {
            this.id = handID;
            this.handSide = handSide;
            clickState = ClickState.None;
            NowActions = new List<MVInputAction>();
            StateStack = new Stack<ClickState>();
        }
       
        int id;
        public int ID { get { return id; } }
        HandSide handSide;
        public HandSide HandSide { get { return handSide; } }
       
        Pose rawPose;
        /// <summary>
        /// 클릭 시점의 Pose
        /// </summary>
        public Pose RawPose { get { return rawPose; } }
        /// <summary>
        /// 클릭 시점의 Pos의 forward
        /// </summary>
        public Vector3 forward { get { return rawPose.forward; } }
        /// <summary>
        /// 클릭 시점의 Pos의 up
        /// </summary>
        public Vector3 up { get { return rawPose.up; } }
        /// <summary>
        /// 클릭 시점의 Pos의 right
        /// </summary>
        public Vector3 right { get { return rawPose.right; } }
        Vector3 deltaPosition;
        /// <summary>
        /// 직전 프레임 대비 현재 이동한 Position
        /// </summary>
        public Vector3 DeltaPosition { get { return deltaPosition; } }
        public Vector3 rawPosition { get { return rawPose.position; } }
        ClickState clickState;
        public ClickState HandClickState { get { return clickState; } }

        float lastDownTime;
        /// <summary>
        /// ClickState 변화가 마지막으로 일어난 frameCount
        /// </summary>
        public float LastDownTime { get { return lastDownTime; } }

        Pose changedPose;
        public Pose NowPose { get { return changedPose; } }
        public List<MVInputAction> NowActions;
        public Stack<ClickState> StateStack;

        //MVHold Started Handler
        public IMVInteractableEventHandler BeHoldingHandler;

        public void SetChangedPose(Pose pose) {
            if(clickState== ClickState.Clicking)
            {
                deltaPosition = pose.position - changedPose.position;
            }
            changedPose = pose;
        }
       
        public void SetClick(ClickState state)
        {
            switch (state)
            {
                case ClickState.None:
                    clickState = ClickState.None;
                    break;
                case ClickState.Down:
                    clickState = ClickState.Down;
                    lastDownTime = Time.time;
                    rawPose = changedPose;
                    StateStack.Push(ClickState.Down);
                    break;
                case ClickState.Clicking:
                    clickState = ClickState.Clicking;
                    StateStack.Push(ClickState.Clicking);
                    break;
                case ClickState.Up:
                    clickState = ClickState.Up;
                    break;
            }
        }

        public enum ClickState
        {
            None,
            Down,
            Clicking,
            Up,
            Max
        }

        public string GetDebugString()
        {
            string result = string.Empty;
            result = string.Format("{0}_{1}_Click:{2}_Raw:{3}_delta:{4}_lastFrame:{5}", handSide.ToString(), id, clickState.ToString(), rawPose.PoseStr(), deltaPosition.V3Str(), lastDownTime);
            return result;
        }

    }


    public enum HandSide
    {
        Left=0,
        Right,
        None,
        Max
    }

    public enum HandJoint
    {
        /// <summary>
        /// The palm.
        /// </summary>
        Palm,
        /// <summary>
        /// The wrist.
        /// </summary>
        Wrist,
        /// <summary>
        /// The lowest joint of the thumb.
        /// </summary>
        ThumbMetacarpal,
        /// <summary>
        /// The second joint of the thumb.
        /// </summary>
        ThumbProximal,
        /// <summary>
        /// The joint nearest the tip of the thumb.
        /// </summary>
        ThumbDistal,
        /// <summary>
        /// The tip of the thumb.
        /// </summary>
        ThumbTip,
        /// <summary>
        /// The lowest joint of the index finger.
        /// </summary>
        IndexMetacarpal,
        /// <summary>
        /// The knuckle joint of the index finger.
        /// </summary>
        IndexProximal,
        /// <summary>
        /// The middle joint of the index finger.
        /// </summary>
        IndexIntermediate,
        /// <summary>
        /// The joint nearest the tip of the index finger.
        /// </summary>
        IndexDistal,
        /// <summary>
        /// The tip of the index finger.
        /// </summary>
        IndexTip,
        /// <summary>
        /// The lowest joint of the middle finger.
        /// </summary>
        MiddleMetacarpal,
        /// <summary>
        /// The proximal joint of the middle finger. 
        /// </summary>
        MiddleProximal,
        /// <summary>
        /// The middle joint of the middle finger.
        /// </summary>
        MiddleIntermediate,
        /// <summary>
        /// The joint nearest the tip of the middle finger.
        /// </summary>
        MiddleDistal,
        /// <summary>
        /// The tip of the middle finger.
        /// </summary>
        MiddleTip,
        /// <summary>
        /// The lowest joint of the ring finger.
        /// </summary>
        RingMetacarpal,
        /// <summary>
        /// The knuckle of the ring finger.
        /// </summary>
        RingProximal,
        /// <summary>
        /// The middle joint of the ring finger.
        /// </summary>
        RingIntermediate,
        /// <summary>
        /// The joint nearest the tip of the ring finger.
        /// </summary>
        RingDistal,
        /// <summary>
        /// The tip of the ring finger.
        /// </summary>
        RingTip,
        /// <summary>
        /// The lowest joint of the little finger.
        /// </summary>
        LittleMetacarpal,
        /// <summary>
        /// The knuckle joint of the little finger.
        /// </summary>
        LittleProximal,
        /// <summary>
        /// The middle joint of the little finger.
        /// </summary>
        LittleIntermediate,
        /// <summary>
        /// The joint nearest the tip of the little finger.
        /// </summary>
        LittleDistal,
        /// <summary>
        /// The tip of the little finger.
        /// </summary>
        LittleTip,
    }
}