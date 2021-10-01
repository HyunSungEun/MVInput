using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.OpenXR;
//using Microsoft.MixedReality.OpenXR;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit;
using System;

public class TestMGH : MonoBehaviour,IMixedRealityGestureHandler<MixedRealityPose> , IMixedRealityInputHandler<MixedRealityPose>
{
    void IMixedRealityGestureHandler<MixedRealityPose>.OnGestureUpdated(InputEventData<MixedRealityPose> eventData)
    {
        Debug.Log("OnGestureUpdated" + getDebugString(eventData) + "InputData_" + eventData.InputData.Position);
    }

    void IMixedRealityGestureHandler<MixedRealityPose>.OnGestureCompleted(InputEventData<MixedRealityPose> eventData)
    {
        Debug.Log("OnGestureCompleted" + getDebugString(eventData) + "InputData_" + eventData.InputData.Position);
    }

    void IMixedRealityGestureHandler.OnGestureStarted(InputEventData eventData)
    {
        Debug.Log("OnGestureStarted" + getDebugString(eventData));
    }

    void IMixedRealityGestureHandler.OnGestureUpdated(InputEventData eventData)
    {
        Debug.Log("OnGestureUpdated" + getDebugString(eventData));
    }

    void IMixedRealityGestureHandler.OnGestureCompleted(InputEventData eventData)
    {
        Debug.Log("OnGestureCompleted" + getDebugString(eventData));
    }

    void IMixedRealityGestureHandler.OnGestureCanceled(InputEventData eventData)
    {
        Debug.Log("OnGestureCanceled" + getDebugString(eventData));
    }

    string getDebugString(InputEventData e)
    {
        UnityEngine.EventSystems.BaseInputModule baseInputModule = e.currentInputModule;
        DateTime d = e.EventTime;
        Handedness hand = e.Handedness;
        IMixedRealityInputSource s = e.InputSource;

        MixedRealityInputAction action = e.MixedRealityInputAction;

        return string.Format("Time_{0},{1},baseName_{2},sId_{3},sname{4},sType{5},actionDesc_{6},actionId_{7},actionAxis_{8}", d, hand.ToString(), baseInputModule.name, s.SourceId, s.SourceName, s.SourceType
            , action.Description, action.Id, action.AxisConstraint.ToString());
    }

    void IMixedRealityInputHandler<MixedRealityPose>.OnInputChanged(InputEventData<MixedRealityPose> eventData)
    {
        Debug.Log("OnInputChanged" + getDebugString(eventData) + "InputData_" + eventData.InputData.Position);
    }
}
