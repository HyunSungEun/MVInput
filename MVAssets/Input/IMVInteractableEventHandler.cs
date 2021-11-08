using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
namespace Movements.XR.Input {
    public interface IMVInteractableEventHandler : IEventSystemHandler
    {
        public bool IsHoldAcceptable();
        public void OnInteractableStarted(     MVInputAction action, MVInteractableEventData eventData        ); // MVInteractableEventData로 수정 요
        public void OnInteractableDragged(     MVInputAction action, MVInteractableEventData<Pose> eventData  );
        public void OnInteractableCompleted(   MVInputAction action, MVInteractableEventData eventData        );
        public void OnInteractableCanceled(    MVInputAction action, MVInteractableEventData eventData        );
    }
}