using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MVPinchRecognizer : MonoBehaviour
{
    
    bool pinched = false;
    float pinchUpThreshold = 0.7f;
    /// <summary>
    /// call when OVRHand.GetFingerIsPinching(OVRHand.HandFinger.Index) == true
    /// </summary>
    public void OnGetFingerIsPinching()
    {

    }
    /// <summary>
    /// call every frame 
    /// </summary>
    /// <param name="strength">OVRHand.GetFingerPinchStrength(OVRHand.HandFinger.Index)</param>
    public void OnGetFingerPinchStrength(float strength)
    {
        
    }
}
