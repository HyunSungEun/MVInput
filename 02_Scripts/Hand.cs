using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Movements.XR.HoloLens
{
    public class Hand
    {
        public Hand()
        {
            clickState = ClickState.None;
        }
        public Pose rawPose;
        public Vector3 forward { get { return rawPose.forward; } }
        public Vector3 up { get { return rawPose.up; } }
        public Vector3 right { get { return rawPose.right; } }
        public Vector3 deltaPosition;
        public Vector3 rawPosition;
        public ClickState clickState;
        Vector3 formerPosition;

        public string GetDebugString()
        {
            string result = string.Empty;
            if (clickState == ClickState.None)
            {

            }
            return result;
        }
        public void SetClick(ClickState state , Pose pose)
        {
            switch (state)
            {
                case ClickState.Down:
                    clickState = ClickState.Down;
                    rawPose = pose;
                    formerPosition = pose.position;
                    deltaPosition = Vector3.zero;
                    rawPosition = pose.position;
                    break;
                case ClickState.Clicking:
                    clickState = ClickState.Clicking;
                    deltaPosition = pose.position - formerPosition;
                    formerPosition = pose.position;
                    break;
                case ClickState.Up:
                    clickState = ClickState.Up;
                    deltaPosition = pose.position - formerPosition;
                    formerPosition = pose.position;
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