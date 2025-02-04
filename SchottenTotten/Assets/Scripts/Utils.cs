using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PRS //Position, Rotation, Scale�� ��� class
{
    public Vector3 pos;
    public Quaternion rot;
    public Vector3 scale;

    public PRS(Vector3 pos, Quaternion rot, Vector3 scale)
    {
        this.pos = pos;
        this.rot = rot;
        this.scale = scale;
    }
}

public class Utils
{
    public static Quaternion QI => Quaternion.identity;
    public static Quaternion Q180 => Quaternion.Euler(0, 0, 180.0f);

    public static Vector3 MousePos
    {
        get
        {
            Vector3 result = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            result.z = -10;
            return result;
        }
    }
}
