using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movements.XR.Input;
public class HandlerTest : MonoBehaviour , IMVInteractableEventHandler
{
    public Transform[] pointerLines;
    public Material mat;
    public Material changedMat;
    bool IMVInteractableEventHandler.IsHoldAcceptable()
    {
        return true;
    }

    void IMVInteractableEventHandler.OnInteractableCanceled(MVInputAction action, MVInputEventData eventData)
    {
        mat.color = Color.magenta;
    }

    void IMVInteractableEventHandler.OnInteractableChanged(MVInputAction action, MVInputEventData<Pose> eventData)
    {
        if (changedMat.color == Color.yellow) changedMat.color = Color.black;
        else changedMat.color = Color.yellow;
    }

    void IMVInteractableEventHandler.OnInteractableCompleted(MVInputAction action, MVInputEventData eventData)
    {
        if (action.Description == "MVClick")
        {
            if (mat.color == Color.red) mat.color = Color.cyan;
            else mat.color = Color.red;
                
        }
        if (action.Description == "MVHold") mat.color = Color.green;
    }

    void IMVInteractableEventHandler.OnInteractableStarted(MVInputAction action, MVInputEventData eventData)
    {
        if (action.Description == "MVHold") mat.color = Color.blue;
    }

    private void Update()
    {
        int i = 0;
        foreach(Hand hand in  MVInput.Hands)
        {
            if(hand==null)
            {
                pointerLines[i].gameObject.SetActive( false);
                    
            }
            else
            {
                pointerLines[i].gameObject.SetActive(true);
                pointerLines[i].SetPositionAndRotation(hand.NowPose.position, hand.NowPose.rotation);
            }
            i++;
        }
    }
}
