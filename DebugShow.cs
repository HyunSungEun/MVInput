using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Movements.XR.HoloLens;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;

public class DebugShow : MonoBehaviour
{
    public TextMeshPro debugScreen;
    public TextMeshPro debugScreenHand;
    public TextMeshPro debugScreenPointerHandleLeft;
    public TextMeshPro debugScreenPointerHandleRight;
    public GameObject LPosDebug;
    public GameObject RPosDebug;
    private void Update()
    {
        debugScreen.text= string.Format("HC_{0},CC_{1}", MVInput.HandCount, MVInput.ClickCount);
        debugScreenPointerHandleLeft.text = MVInput.GetHand(HandSide.Left)==null?"Left_NULL": MVInput.GetHand(HandSide.Left).GetDebugString();
        debugScreenPointerHandleRight.text = MVInput.GetHand(HandSide.Right) == null ? "Right_NULL" : MVInput.GetHand(HandSide.Right).GetDebugString();

        foreach(Hand hand in MVInput.Hands)
        {
            if (hand == null) continue;
            if(hand.HandSide== HandSide.Left)
            {
                LPosDebug.transform.SetPositionAndRotation(MVInput.GetHand(HandSide.Left).RawPose.position, MVInput.GetHand(HandSide.Left).RawPose.rotation);

            }
            else
            {
                RPosDebug.transform.SetPositionAndRotation(MVInput.GetHand(HandSide.Right).RawPose.position, MVInput.GetHand(HandSide.Right).RawPose.rotation);
            }
        }


    }

    public GameObject RPosDebugPointerPose;
    public GameObject RPosDebugGripPose;
    public GameObject RPosDebugIndexFingerPose;
    internal void DebugInputs(InputEventData<MixedRealityPose> eventData)
    {
        GameObject temp = null;
        if(eventData.MixedRealityInputAction.Description=="Pointer Pose")
        {
            temp = RPosDebugPointerPose;
        }
        if (eventData.MixedRealityInputAction.Description == "Grip Pose")
        {
            temp = RPosDebugGripPose;
        }
        if (eventData.MixedRealityInputAction.Description == "Index Finger Pose")
        {
            temp = RPosDebugIndexFingerPose;
        }
        temp?.transform.SetPositionAndRotation(eventData.InputData.Position, eventData.InputData.Rotation);
    }
}
