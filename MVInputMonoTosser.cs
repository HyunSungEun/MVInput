using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

using TMPro;


namespace Movements.XR.HoloLens
{
    public class MVInputMonoTosser : MonoBehaviour, IMixedRealityInputHandler, IMixedRealityInputHandler<MixedRealityPose>
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

           // CoreServices.InputSystem?.RegisterHandler<IMixedRealityPointerHandler>(this);
        }
        void OnDisable()
        {
            CoreServices.InputSystem?.UnregisterHandler<IMixedRealityInputHandler>(this);
            CoreServices.InputSystem?.UnregisterHandler<IMixedRealityInputHandler<MixedRealityPose>>(this);

          //  CoreServices.InputSystem?.UnregisterHandler<IMixedRealityPointerHandler>(this);
        }

        void IMixedRealityInputHandler<MixedRealityPose>.OnInputChanged(InputEventData<MixedRealityPose> eventData)=> MVInput.OnInputChanged(eventData);
        void IMixedRealityInputHandler.OnInputDown(InputEventData eventData) => MVInput.OnInputDown(eventData);
        void IMixedRealityInputHandler.OnInputUp(InputEventData eventData) => MVInput.OnInputUp(eventData);


        void Update() => MVInput.Update();
        private void LateUpdate() => MVInput.LateUpdate();
        //LateUpdate 후의 콜백이지만 렌더러가 있어야 가능
        private void OnPreCull()    => MVInput.OnPreCull();

        
    }
}