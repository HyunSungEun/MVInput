using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Movements.XR.Input
{
    public class MVInteractableEventData
    {
        public MVInteractableEventData(Hand hand)
        {
            eventTime = System.DateTime.Now;
            Hand = hand;
        }
        System.DateTime eventTime;
        public System.DateTime EventTime { get { return eventTime; } }
        public HandSide HandSide { get { return Hand.HandSide; } }
        public Hand Hand;

    }
    public class MVInteractableEventData<RaycastHit> : MVInteractableEventData
    {
        RaycastHit data;

        public MVInteractableEventData(RaycastHit data, Hand hand) : base( hand)
        {
            this.data = data;
        }

        public RaycastHit Data { get { return data; } }
    }
}