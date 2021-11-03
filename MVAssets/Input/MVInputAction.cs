using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Movements.XR.Input
{
    public struct MVInputAction
    {
        public MVInputAction(string description)
        {
            this.description = description;
            this.createdFrameCount = Time.frameCount;
            this.createdTime = Time.time;
        }

        string description;
        public string Description { get { return description; } }
        float createdFrameCount;
        public float CreatedFrameCount { get { return createdFrameCount; } }
        float createdTime;
        public float CreatedTime { get { return createdTime; } }

    }
}