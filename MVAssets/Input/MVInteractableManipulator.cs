using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Movements.XR.Input
{

    public class MVInteractableManipulator : MonoBehaviour ,IMVInteractableEventHandler
    {
        [System.Flags]
        public enum InteractableMode
        {
            None=0,
            Near
        }
        Plane lastChangedPlane; //Plane.Raycast
        //아이디어 = 핸드와 물체 hitpoint
        Vector3 destination;
        float lerpSpeed;

        public bool Interactable;
        

        bool IMVInteractableEventHandler.IsHoldAcceptable()
        {
            return Interactable;
        }

        void IMVInteractableEventHandler.OnInteractableStarted(MVInputAction action, MVInteractableEventData eventData)
        {
            if (action.Description != "MVHold") return;
            Hand hand = MVInput.GetHand(eventData.HandSide);
            if (hand == null) return;
        }

        void IMVInteractableEventHandler.OnInteractableDragged(MVInputAction action, MVInteractableEventData<Pose> eventData)
        {

        }

        void IMVInteractableEventHandler.OnInteractableCompleted(MVInputAction action, MVInteractableEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        void IMVInteractableEventHandler.OnInteractableCanceled(MVInputAction action, MVInteractableEventData eventData)
        {
            throw new System.NotImplementedException();
        }
    }
}