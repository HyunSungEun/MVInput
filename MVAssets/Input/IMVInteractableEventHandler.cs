using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
namespace Movements.XR.Input {
    public interface IMVInteractableEventHandler : IEventSystemHandler
    {
        public bool IsHoldAcceptable();
        public void OnInteractableStarted(     MVInputAction action, MVInputEventData eventData        );
        public void OnInteractableChanged(     MVInputAction action, MVInputEventData<Pose> eventData  );
        public void OnInteractableCompleted(   MVInputAction action, MVInputEventData eventData        );
        public void OnInteractableCanceled(    MVInputAction action, MVInputEventData eventData        );
    }
}