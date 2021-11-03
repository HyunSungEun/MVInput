using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyExtendMethod 
{
    public static string V3Str(this Vector3 v) { return string.Format("({0:N2},{1:N2},{2:N2})", v.x, v.y, v.z); }
    public static string QtStr(this Quaternion q) {
        Vector3 temp = q.eulerAngles;
        return temp.V3Str();
    }
    public static string PoseStr(this Pose pose)
    {
        Vector3 v = pose.position;
        Quaternion q = pose.rotation;
        return string.Format("[{0},{1}]",v.V3Str(), q.QtStr());
    }
}
