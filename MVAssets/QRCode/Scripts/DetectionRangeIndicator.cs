using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class DetectionRangeIndicator : MonoBehaviour
{
    public Material lineMat;
    float lineWidth = 0.01f;
    LineRenderer lineRenderer;
    int standardPointsNumber = 8;
    LineRenderer[] standardLineRenderers;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.useWorldSpace = false;
        lineRenderer.material = lineMat;
        standardLineRenderers = new LineRenderer[standardPointsNumber];
        Transform slp = new GameObject("StandardLineRenderer").transform;
        slp.parent = transform;
        for (int i = 0; i < standardPointsNumber; i++)
        {
            GameObject g = new GameObject("StandardLineRenderer_" + i);
            g.transform.parent = slp;
            standardLineRenderers[i]= g.AddComponent<LineRenderer>();
            standardLineRenderers[i].startWidth = lineWidth;
            standardLineRenderers[i].endWidth = lineWidth;
            standardLineRenderers[i].useWorldSpace = false;
            standardLineRenderers[i].material = lineMat;
        }
    }

    [ContextMenu("df")]
    public void qwe()
    {
        DrawRangeLines(2f, 1f, 30f, 10);
    }


    public void DrawRangeLines(float height, float basicHeight,float thetaDegree, int smoothArcFactor)
    {
        Vector3[] topArc, middleArc, bottomArc;
        if (smoothArcFactor > 10) smoothArcFactor = 10;
        if (smoothArcFactor < 1) smoothArcFactor = 1;

        int arcPointNumbers = (standardPointsNumber * smoothArcFactor) +1;

        topArc = new Vector3[arcPointNumbers];
        middleArc = new Vector3[arcPointNumbers];
        bottomArc = new Vector3[arcPointNumbers];

        topArc = GetArcPoints(height * Mathf.Tan(thetaDegree * Mathf.Deg2Rad), arcPointNumbers , height);
        middleArc = GetArcPoints(basicHeight * Mathf.Tan(thetaDegree * Mathf.Deg2Rad), arcPointNumbers, basicHeight);
        bottomArc = GetArcPoints(basicHeight * Mathf.Tan(thetaDegree * Mathf.Deg2Rad), arcPointNumbers, 0f);






        lineRenderer.positionCount = topArc.Length *3;

        int i = 0;
        foreach(Vector3 v in topArc )
        {
            lineRenderer.SetPosition(i, topArc[i]);
            i++;
        }
        int n = 0;
        foreach (Vector3 v in middleArc)
        {
            lineRenderer.SetPosition(i, middleArc[n]);
            i++;
            n++;
        }
        n = 0;
        foreach (Vector3 v in bottomArc)
        {
            lineRenderer.SetPosition(i, bottomArc[n]);
            i++;
            n++;
        }

        foreach(LineRenderer l in standardLineRenderers)
        {
            l.positionCount = 0;
            l.positionCount = 3;
        }

        Vector3[][] tempArcs = new Vector3[][] { topArc, middleArc, bottomArc };

        for(int s = 1; s < standardPointsNumber; s++)
        {
            int standardIndex = s * smoothArcFactor;
            for(int l = 0; l < 3; l++)
            {
                standardLineRenderers[s].SetPosition(l,tempArcs[ l][ standardIndex]);//스탠다드 포인트 위치 인덱스 획득해서 넣어
            }

        }

    }

    Vector3[] GetArcPoints(float radius, int pointNum , float height)
    {
        List<Vector3> list = new List<Vector3>();
        for(int i = 0; i < pointNum; i++)
        {
            float theta = (((float)i * (1 / ((float)pointNum -1) ))  *360f ) * Mathf.Deg2Rad;
            Debug.Log($"{i}번째 세타값 {theta}");
            list.Add(new Vector3(  Mathf.Cos(theta)*radius, height, Mathf.Sin(theta)*radius));
        }
        return list.ToArray();
    }
}
