using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;


namespace Movements.XR.HoloLens
{
    public class MVInputMonoTosser : MonoBehaviour, IMixedRealityInputHandler, IMixedRealityInputHandler<MixedRealityPose> , IMixedRealitySourceStateHandler
    {
        static MVInputMonoTosser _instance=null;
        private void Awake()
        {
            if (_instance == null) { _instance = this; }
            else
            {
                Destroy(this);
            }
        }

        void OnEnable()
        {
            CoreServices.InputSystem?.RegisterHandler<IMixedRealityInputHandler>(this);
            CoreServices.InputSystem?.RegisterHandler<IMixedRealityInputHandler<MixedRealityPose>>(this);
        }
        void OnDisable()
        {
            CoreServices.InputSystem?.UnregisterHandler<IMixedRealityInputHandler>(this);
            CoreServices.InputSystem?.UnregisterHandler<IMixedRealityInputHandler<MixedRealityPose>>(this);
        }

        void IMixedRealityInputHandler<MixedRealityPose>.OnInputChanged(InputEventData<MixedRealityPose> eventData)
        {
            if (eventData.InputSource.SourceType != InputSourceType.Hand) return;
            MVInput.OnInputChanged(eventData);
        }

        void IMixedRealityInputHandler.OnInputDown(InputEventData eventData)
        {
            if (eventData.InputSource.SourceType != InputSourceType.Hand) return;
            MVInput.OnInputDown(eventData);
        }

        void IMixedRealityInputHandler.OnInputUp(InputEventData eventData)
        {
            if (eventData.InputSource.SourceType != InputSourceType.Hand) return;
            MVInput.OnInputUp(eventData);
        }

      
        void IMixedRealitySourceStateHandler.OnSourceDetected(SourceStateEventData eventData)
        {
            MVInput.OnSourceDetected(eventData);
        }

        void IMixedRealitySourceStateHandler.OnSourceLost(SourceStateEventData eventData)
        {
            MVInput.OnSourceLost(eventData);
        } 
        
        
        
        // Update is called once per frame
        void Update()
        {
            MVInput.Update();
        }
        private void OnPreCull()
        {
            //late update후의 콜백으로 사용
          //  MVInput.MoveFrameCursor();
        }
        
    }
}