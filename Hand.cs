using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Movements.XR.HoloLens
{
    public class Hand
    {
        public Hand(int handID,HandSide handSide)
        {
            this.id = handID;
            this.handSide = handSide;
            clickState = ClickState.None;
        }
        public Hand(int handID, Microsoft.MixedReality.Toolkit.Utilities.Handedness handSide) : this(handID, ConvertHand(handSide)) { }
       
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
        float lastClickChangedFrameCount;
        /// <summary>
        /// ClickState 변화가 마지막으로 일어난 frameCount
        /// </summary>
        public float LastClickChangedFrameCount { get { return lastClickChangedFrameCount; } }

        System.Action ClickUpEvent;

        Pose changedPose;
        
        public void SetChangedPose(Pose pose) {
            if(clickState== ClickState.Clicking)
            {
                deltaPosition = pose.position - changedPose.position;
            }
            changedPose = pose;
        }

        public void RagisterClickUpHandler(bool subscribe, System.Action callback) {
            if (subscribe) ClickUpEvent += callback;
            else ClickUpEvent -= callback;
        }
        /// <summary>
        /// Click Up 이벤트 Raise
        /// </summary>
        public void RaiseHandClickUp()
        {
            if(ClickUpEvent!=null) ClickUpEvent();
        }

       
        public void SetClick(ClickState state)
        {
            switch (state)
            {
                case ClickState.None:
                    clickState = ClickState.None;
                    lastClickChangedFrameCount = Time.frameCount;
                    break;
                case ClickState.Down:
                    clickState = ClickState.Down;
                    lastClickChangedFrameCount = Time.frameCount;
                    rawPose = changedPose;
                    break;
                case ClickState.Clicking:
                    clickState = ClickState.Clicking;
                    lastClickChangedFrameCount = Time.frameCount;
                    break;
                case ClickState.Up:
                    clickState = ClickState.Up;
                    lastClickChangedFrameCount = Time.frameCount;
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
        public static HandSide ConvertHand(Microsoft.MixedReality.Toolkit.Utilities.Handedness handedness)
        {
            HandSide result = HandSide.None;
            switch (handedness)
            {
                case Microsoft.MixedReality.Toolkit.Utilities.Handedness.Left:
                    result = HandSide.Left;
                    break;
                case Microsoft.MixedReality.Toolkit.Utilities.Handedness.Right:
                    result = HandSide.Right;
                    break;
            }
            return result;
        } 
        public string GetDebugString()
        {
            string result = string.Empty;
            result = string.Format("{0}_{1}_Click:{2}_Raw:{3}_delta:{4}_lastFrame:{5}", handSide.ToString(), id, clickState.ToString(), rawPose.PoseStr(), deltaPosition.V3Str(), lastClickChangedFrameCount);
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