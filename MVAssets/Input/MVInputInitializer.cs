using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Movements.XR.Input
{
    public class MVInputInitializer : MonoBehaviour
    {

#if UNITY_ANDROID && OCULUS_QUEST_2
        [SerializeField]
        OVRManager ovrManager;
        OVRHand[] OVRHands;
        Transform trackingSpace;
#endif
        void Start()
        {
#if UNITY_ANDROID && OCULUS_QUEST_2
            trackingSpace = ovrManager?.transform.Find("TrackingSpace");
            if (trackingSpace == null)
            {
                Debug.Log("MYLOG: Can't Find trackingSpace , set OVRCameraRig Prefab Instance as ovRmanager.");
                return;
            }
            Transform lHA = trackingSpace.transform.Find("LeftHandAnchor");
            Transform RHA = trackingSpace.transform.Find("RightHandAnchor");
            if(lHA==null || RHA == null)
            {
                Debug.Log("MYLOG: Can't Find handHanger , set OVRCameraRig Prefab Instance as ovRmanager.");
                return;
            }
            OVRHand lOVRHand = lHA.GetComponentInChildren<OVRHand>();
            OVRHand rOVRHand = RHA.GetComponentInChildren<OVRHand>();
            if (lOVRHand == null || rOVRHand == null)
            {
                Debug.Log("MYLOG: Can't Find OVRHand in handHanger , have hanger keep OVRHandPrefab Prefab Instance as a child.");
                return;
            }
            OVRHands = new OVRHand[] { lOVRHand, rOVRHand };

            foreach (OVRHand hand in OVRHands)
            {
                if(hand.gameObject.GetComponent<MVPinchRecognizer>() == null)
                hand.gameObject.AddComponent<MVPinchRecognizer>();
            }
            MVInput.Initialize(OVRHands,trackingSpace);
            Debug.Log("MYLOG: MVInput_Oculus Init");
#elif WINDOWS_UWP
            MVInput.Initialize();
#elif UNITY_ANDROID && (OCULUS_QUEST_2 == false)
//안드로이드 (터치스크린) 경우
            MVInput.Initialize();   //이 함수에서 처리
#endif
        }

        private void Update()
        {
#if UNITY_ANDROID && OCULUS_QUEST_2
            if(OVRInput.IsControllerConnected(OVRInput.Controller.Hands) == false)
            {
                MVInput.DeInitialize();
                Debug.Log("MYLOG: MVInput_Oculus DeInit");
            }
            else if(MVInput.IsInitialized == false && (OVRHands!=null && trackingSpace!=null))
            {
                MVInput.Initialize(OVRHands, trackingSpace);
                Debug.Log("MYLOG: MVInput_Oculus ReInit");
            }
#endif
        }


    }
}