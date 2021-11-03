using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace Movements.XR.HandGuide
{
    public class MVHandGuide : MonoBehaviour
    {
        Transform lineEndPoint;
        LineRenderer line;
        Canvas descriptionCanvas;
        float maxLineLength = 1f;

        private void Awake()
        {
            lineEndPoint = transform.GetChild(0).Find("LineEnd");
            line = GetComponentInChildren<LineRenderer>();
            descriptionCanvas = GetComponentInChildren<Canvas>();
        }
        public void Place(Vector3 guidePos, Vector3 lookPos, string description)
        {
            float distance = (guidePos - lookPos).magnitude;
            descriptionCanvas.transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>().text = description;
            maxLineLength = Mathf.Min(distance, 2f);
            transform.position = guidePos;
            transform.LookAt(lookPos);
        }

        private void Update()
        {
            line.SetPosition(1, Vector3.forward * lineEndPoint.localPosition.z * maxLineLength);
            descriptionCanvas.transform.LookAt(Camera.main.transform);
            descriptionCanvas.transform.rotation = Quaternion.Euler(0f, descriptionCanvas.transform.rotation.eulerAngles.y, 0f);
        }
    }
}