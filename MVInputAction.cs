using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MVInputAction 
{ 
    public MVInputAction(string description, float createdFrameCount, float createdTime)
    {
        this.description = description;
        this.createdFrameCount = createdFrameCount;
        this.createdTime = createdTime;
    }

    string description;
    public string Description { get { return description; } }
    float createdFrameCount;
    public float CreatedFrameCount { get { return createdFrameCount; } }
    float createdTime;
    public float CreatedTime { get { return createdTime; } }
}
