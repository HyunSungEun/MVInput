using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Movements.XR.HoloLens;
public class DebugShow : MonoBehaviour
{
    public TextMeshPro debugScreen;
    public TextMeshPro debugScreenHand;
    public TextMeshPro debugScreenPointerHandleLeft;
    public TextMeshPro debugScreenPointerHandleRight;
    private void Update()
    {
        debugScreen.text= string.Format("HC_{0},CC_{1}", MVInput.HandCount, MVInput.ClickCount);
        debugScreenPointerHandleLeft.text = MVInput.GetHand(HandSide.Left)==null?"Left_NULL": MVInput.GetHand(HandSide.Left).GetDebugString();
        debugScreenPointerHandleRight.text = MVInput.GetHand(HandSide.Right) == null ? "Right_NULL" : MVInput.GetHand(HandSide.Right).GetDebugString();
    }
}
