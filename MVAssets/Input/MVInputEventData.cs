using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Movements.XR.Input
{
    public class MVInputEventData
    {
        HandSide handSide;
        System.DateTime eventTime;

        public MVInputEventData(HandSide handSide)
        {
            this.handSide = handSide;
            eventTime = System.DateTime.Now;
        }

        public HandSide HandSide { get { return handSide; } }
        public System.DateTime EventTime { get { return eventTime; } }
    }
    public class MVInputEventData<T> : MVInputEventData
    {
        T inputData;

        public MVInputEventData(HandSide handSide, T data) : base(handSide)
        {
            inputData = data;
        }

        public T InputData { get { return inputData; } }
    }
}