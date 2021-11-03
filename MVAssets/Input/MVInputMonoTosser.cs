using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if WINDOWS_UWP
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
#endif

namespace Movements.XR.Input
{
    /// <summary>
    /// MVInput에 Handler 넘겨주는 역할
    /// </summary>
    public class MVInputMonoTosser : MonoBehaviour
#if WINDOWS_UWP
        , IMixedRealityInputHandler, IMixedRealityInputHandler<MixedRealityPose>
#elif UNITY_ANDROID && OCULUS_QUEST_2
#endif
    {
        void OnEnable()
        {
#if WINDOWS_UWP
            CoreServices.InputSystem?.RegisterHandler<IMixedRealityInputHandler>(this);
            CoreServices.InputSystem?.RegisterHandler<IMixedRealityInputHandler<MixedRealityPose>>(this);
#endif
        }
        void OnDisable()
        {
#if WINDOWS_UWP
            CoreServices.InputSystem?.UnregisterHandler<IMixedRealityInputHandler>(this);
            CoreServices.InputSystem?.UnregisterHandler<IMixedRealityInputHandler<MixedRealityPose>>(this);
#endif
        }
#if WINDOWS_UWP
        void IMixedRealityInputHandler<MixedRealityPose>.OnInputChanged(InputEventData<MixedRealityPose> eventData)
        {
            if (IsSubjectInputEventData(eventData) == false) return;
            //if((eventData.InputData.Position - Vector3.zero).magnitude >0f) Debug.Log(Time.frameCount + "OnInputChanged" + eventData.Handedness +"_"+ eventData.MixedRealityInputAction.Description + "_" + eventData.MixedRealityInputAction.AxisConstraint.ToString() + "디버그" + getDebugString(eventData));
            //Input Action Pointer Pose의 Pose를 핸드 Pose에 적용
            if (eventData.MixedRealityInputAction.Description != "Pointer Pose") return;

            MVInput.OnInputChanged(new MVInputEventData<Pose>(MVInput.ConvertToHandSide(eventData.Handedness)
                ,new Pose(eventData.InputData.Position, eventData.InputData.Rotation)));
        }
        void IMixedRealityInputHandler.OnInputDown(InputEventData eventData) {

            if (IsSubjectInputEventData(eventData) == false) return;
            //  Debug.Log(Time.frameCount + "OnInputDown"+eventData.Handedness + eventData.MixedRealityInputAction.Description+eventData.MixedRealityInputAction.AxisConstraint.ToString()+"디버그"+ getDebugString(eventData));
            //Input Action Select를 클릭 상태에 적용
            if (eventData.MixedRealityInputAction.Description != "Select") return;

            MVInput.OnInputDown(new MVInputEventData(MVInput.ConvertToHandSide(eventData.Handedness) )); 
        }
        void IMixedRealityInputHandler.OnInputUp(InputEventData eventData) {
            if (IsSubjectInputEventData(eventData) == false) return;
            // Debug.Log(Time.frameCount + "OnInputUp" + eventData.Handedness + eventData.MixedRealityInputAction.Description + eventData.MixedRealityInputAction.AxisConstraint.ToString() + "디버그" + getDebugString(eventData));

            if (eventData.MixedRealityInputAction.Description != "Select") return;
            MVInput.OnInputUp(new MVInputEventData(MVInput.ConvertToHandSide(eventData.Handedness)));
        }

#elif UNITY_ANDROID && OCULUS_QUEST_2

        void OnInputChanged(MVInputEventData<Pose> eventData)=> MVInput.OnInputChanged(eventData);
        void OnInputDown(MVInputEventData eventData) => MVInput.OnInputDown(eventData);
        void OnInputUp(MVInputEventData eventData) => MVInput.OnInputUp(eventData);
#endif

        void Update() {
                MVInput.Update(); 
        }
        private void LateUpdate() {
#if WINDOWS_UWP
            MVInput.LateUpdate();
#elif UNITY_ANDROID && OCULUS_QUEST_2

            for(int i = 0; i < MVInput.OVRHands.Length; i++)
            {
                HandSide handSide = (HandSide)i;
                MVPinchRecognizer pinchRecognizer = MVInput.OVRHands[i].GetComponent<MVPinchRecognizer>();
                //핀치 상태 확인 후 OnInPutChanged 등 이벤트  발생

                OnInputChanged(new MVInputEventData<Pose>(handSide, pinchRecognizer.OnPinchChanged()));
                if (pinchRecognizer.OnPinchDown()) OnInputDown(new MVInputEventData(handSide));
                if (pinchRecognizer.OnPinchUp()) OnInputUp(new MVInputEventData(handSide));
            }
            MVInput.LateUpdate(); 
#endif
        }
        //LateUpdate 후의 유니티 콜백이지만 렌더러가 있어야 가능
        private void OnPreCull()    => MVInput.OnPreCull();

#if WINDOWS_UWP
        bool IsSubjectInputEventData(InputEventData eventData)
        {
            if (eventData.InputSource.SourceType != InputSourceType.Hand) return false;
            if (!(eventData.Handedness == Handedness.Left || eventData.Handedness == Handedness.Right)) return false;
            return true;
        }
#endif
    }
}