using Assets.MarchingCubesGPU.Scripts;
using MarchingCubesGPUProject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathExtension
{
    public static Vector4 ToVector4(this Vector3 v)
    {
        return new Vector4(v.x, v.y, v.z, 1);
    }

    public static Vector3 ToVector3(this Vector4 v)
    {
        return new Vector3(v.x, v.y, v.z);
    }

    public static float[] ToFloats(this Matrix4x4 m)
    {
        //matrix has to be transposed but then in shader is ok
        //https://feedback.unity3d.com/suggestions/add-setmatrix-to-compute-shaders

        return new float[]
        {
            m.m00, m.m10, m.m20, m.m30,
            m.m01, m.m11, m.m21, m.m31,
            m.m02, m.m12, m.m22, m.m32,
            m.m03, m.m13, m.m23, m.m33,
        };
    }
}
