using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Movements.XR.Input
{
    public class MVPinchRecognizer : MonoBehaviour
    {
#if UNITY_ANDROID && OCULUS_QUEST_2
  float pinchUpThreshold = 0.6f;
        OVRHand ovrHand;
        private void Start()
        {
            ovrHand = GetComponent<OVRHand>();
        }
        bool lastPinched = false;
        bool nowPinch = false;
        public bool OnPinchDown()
        {
            return lastPinched == false && nowPinch == true;
        }
        public bool OnPinchUp()
        {
            return lastPinched == true && nowPinch == false;
        }
        public Pose OnPinchChanged()
        {
            Vector3 pointerPosition = MVInput.TrackingSpace.TransformPoint(ovrHand.PointerPose.position);
            Vector3 forwardDir = MVInput.TrackingSpace.TransformDirection(ovrHand.PointerPose.forward);
            return new Pose(pointerPosition, Quaternion.LookRotation(forwardDir));
        }
        void Update()
        {
            if (ovrHand.IsDataHighConfidence && ovrHand.IsPointerPoseValid)
            {

                lastPinched = nowPinch;

                if (ovrHand.GetFingerIsPinching(OVRHand.HandFinger.Index) == false && ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Index) < pinchUpThreshold)
                    nowPinch = false;
                else if (ovrHand.GetFingerIsPinching(OVRHand.HandFinger.Index) && ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Index) > 0.9f)
                {
                    nowPinch = true;
                }
                else
                {
                    nowPinch = lastPinched;
                }
            }
            else
            {
                nowPinch = false;
                lastPinched = false;
            }
        }
#endif
    }
}
