using Assets.MarchingCubesGPU.Scripts;
using MarchingCubesGPUProject;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RotationHelper
{
    public static Vector3 GetRotation_1(Vector3 objPos, Vector3 objRotation, Vector3 startConPos, Vector3 currentConPos)
    {
        var startVec = startConPos - objPos;
        var currentVec = currentConPos - objPos;

        var rotationChange = Quaternion.FromToRotation(startVec, currentVec).eulerAngles;
        var rotation = NormalizeRotation(ComposeRotations(rotationChange, objRotation));

        return rotation;
    }
    public static Vector3 GetRotation_2(Vector3 objPos, Vector3 objRotation, Vector3 startConPos, Vector3 currentConPos)
    {
        var startVec = startConPos - objPos;
        var currentVec = currentConPos - objPos;

        var horChange = GetHorizontalRotation(startVec, currentVec);
        var rotation = NormalizeRotation(ComposeRotations(horChange, objRotation));

        return rotation;
    }
    public static Vector3 GetRotation_3(Vector3 objPos, Vector3 objRotation, Vector3 startConPos, Vector3 currentConPos)
    {
        var startVec = startConPos - objPos;
        var currentVec = currentConPos - objPos;

        var horChange = GetHorizontalRotation(startVec, currentVec);
        var verChange = GetVerticalRotation(startVec, currentVec);

        if (Math.Abs(horChange.y) > Math.Abs(verChange.x) && Math.Abs(horChange.y) > Math.Abs(verChange.z))
        {
            var rotation = NormalizeRotation(ComposeRotations(horChange, objRotation));
            return rotation;
        }
        else
        {
            var rotation = NormalizeRotation(ComposeRotations(verChange, objRotation));
            return rotation;
        }
    }
    public static Vector3 GetRotation_4(Vector3 objPos, Vector3 objRotation, Vector3 startConPos, Vector3 currentConPos)
    {
        var startVec = startConPos - objPos;
        var currentVec = currentConPos - objPos;

        var horChange = GetHorizontalRotation(startVec, currentVec);
        var verChange = GetVerticalRotation(startVec, currentVec);

        if (Math.Abs(horChange.y) > Math.Abs(verChange.x) && Math.Abs(horChange.y) > Math.Abs(verChange.z))
        {
            var rotation = NormalizeRotation(ComposeRotations(horChange, objRotation));
            return rotation;
        }
        else
        {
            var rotation = NormalizeRotation(ComposeRotations(verChange, objRotation));

            var boundaryAngle = 10f;
            if (Math.Abs(rotation.x) < boundaryAngle && Math.Abs(rotation.z) < boundaryAngle)
            {
                rotation.x = 0;
                rotation.z = 0;
            }

            return rotation;
        }
    }
    public static Vector3 GetRotation(Vector3 objPos, Vector3 objRotation, Vector3 startConPos, Vector3 currentConPos)
    {
        var startVec = startConPos - objPos;
        var currentVec = currentConPos - objPos;

        var horChange = GetHorizontalRotation(startVec, currentVec);
        var verChange = GetVerticalRotation(startVec, currentVec);

        if (Math.Abs(horChange.y) > Math.Abs(verChange.x) && Math.Abs(horChange.y) > Math.Abs(verChange.z))
        {
            var rotation = NormalizeRotation(ComposeRotations(horChange, objRotation));
            return rotation;
        }
        else
        {
            var rotation = NormalizeRotation(ComposeRotations(verChange, objRotation));


            var boundaryAngle = 5f;
            if (Math.Abs(rotation.x) < boundaryAngle && Math.Abs(rotation.z) < boundaryAngle)
            {
                rotation.x = 0;
                rotation.z = 0;
            }

            rotation = Vector3.Max(rotation, new Vector3(-80, rotation.y, -80));
            rotation = Vector3.Min(rotation, new Vector3(80, rotation.y, 80));


            return rotation;
        }
    }

    private static Vector3 GetHorizontalRotation(Vector3 startVec, Vector3 currentVec)
    {
        var horCurrVec = Vector3.ProjectOnPlane(currentVec, Vector3.up);
        var horStartVec = Vector3.ProjectOnPlane(startVec, Vector3.up);
        var horChange = Quaternion.FromToRotation(horStartVec, horCurrVec).eulerAngles;
        horChange = NormalizeRotation(horChange);

        return horChange * 5;
    }
    private static Vector3 GetVerticalRotation(Vector3 startVec, Vector3 currentVec)
    {
        var verPlaneNormal = Vector3.Cross(Vector3.up, startVec);
        var verCurrVec = Vector3.ProjectOnPlane(currentVec, verPlaneNormal);
        var verChange = Quaternion.FromToRotation(startVec, verCurrVec).eulerAngles;
        verChange = NormalizeRotation(verChange);

        return verChange * 5;
    }

    private static Vector3 ComposeRotations(Vector3 r1, Vector3 r2)
    {
        var rotation = (Quaternion.Euler(r1) * Quaternion.Euler(r2)).eulerAngles;
        return rotation;
    }

    private static Vector3 NormalizeRotation(Vector3 rotation)
    {
        var normalizedRotation = new Vector3();
        normalizedRotation.x = NormalizeAngle(rotation.x);
        normalizedRotation.y = NormalizeAngle(rotation.y);
        normalizedRotation.z = NormalizeAngle(rotation.z);

        //hot-fix
        if (normalizedRotation.z == 180 && normalizedRotation.y == 180)
        {
            normalizedRotation.x = NormalizeAngle(180 - normalizedRotation.x);
            normalizedRotation.z -= 180;
            normalizedRotation.y -= 180;
        }

        return normalizedRotation;
    }
    private static float NormalizeAngle(float angle)
    {
        while (angle <= -180) angle += 360;
        while (angle > 180) angle -= 360;

        return angle;
    }
}
