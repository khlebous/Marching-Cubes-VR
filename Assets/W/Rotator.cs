using Assets.MarchingCubesGPU.Scripts;
using MarchingCubesGPUProject;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator
{
    public Vector3 GetRotation_1(Vector3 startTerrainPosition, Vector3 startTerrainRotation, Vector3 startPos, Vector3 currentPos)
    {
        var startVec = startPos - startTerrainPosition;
        var currentVec = currentPos - startTerrainPosition;

        var rotationChange = Quaternion.FromToRotation(startVec, currentPos).eulerAngles;
        var rotation = NormalizeRotation(ComposeRotations(rotationChange, startTerrainRotation));

        return rotation;
    }
    public Vector3 GetRotation_2(Vector3 startTerrainPosition, Vector3 startTerrainRotation, Vector3 startPos, Vector3 currentPos)
    {
        var startVec = startPos - startTerrainPosition;
        var currentVec = currentPos - startTerrainPosition;

        var horRotationChange = GetHorizontalRotation(startVec, currentVec);
        var rotation = NormalizeRotation(ComposeRotations(horRotationChange, startTerrainRotation));

        return rotation;
    }
    public Vector3 GetRotation_3(Vector3 startTerrainPosition, Vector3 startTerrainRotation, Vector3 startPos, Vector3 currentPos)
    {
        var startVec = startPos - startTerrainPosition;
        var currentVec = currentPos - startTerrainPosition;

        var horRotationChange = GetHorizontalRotation(startVec, currentVec);
        var verRotationChange = GetVerticalRotation(startVec, currentVec);

        if (Math.Abs(horRotationChange.y) > Math.Abs(verRotationChange.x) && Math.Abs(horRotationChange.y) > Math.Abs(verRotationChange.z))
        {
            var rotation = NormalizeRotation(ComposeRotations(horRotationChange, startTerrainRotation));
            return rotation;
        }
        else
        {
            var rotation = NormalizeRotation(ComposeRotations(verRotationChange, startTerrainRotation));
            return rotation;
        }
    }
    public Vector3 GetRotation_4(Vector3 startTerrainPosition, Vector3 startTerrainRotation, Vector3 startPos, Vector3 currentPos)
    {
        var startVec = startPos - startTerrainPosition;
        var currentVec = currentPos - startTerrainPosition;

        var horRotationChange = GetHorizontalRotation(startVec, currentVec);
        var verRotationChange = GetVerticalRotation(startVec, currentVec);

        if (Math.Abs(horRotationChange.y) > Math.Abs(verRotationChange.x) && Math.Abs(horRotationChange.y) > Math.Abs(verRotationChange.z))
        {
            var rotation = NormalizeRotation(ComposeRotations(horRotationChange, startTerrainRotation));
            return rotation;
        }
        else
        {
            var rotation = NormalizeRotation(ComposeRotations(verRotationChange, startTerrainRotation));

            var boundaryAngle = 10f;
            if (Math.Abs(rotation.x) < boundaryAngle && Math.Abs(rotation.z) < boundaryAngle)
            {
                rotation.x = 0;
                rotation.z = 0;
            }

            return rotation;
        }
    }
    public Vector3 GetRotation_5(Vector3 startTerrainPosition, Vector3 startTerrainRotation, Vector3 startPos, Vector3 currentPos)
    {
        var startVec = startPos - startTerrainPosition;
        var currentVec = currentPos - startTerrainPosition;

        var horRotationChange = GetHorizontalRotation(startVec, currentVec);
        var verRotationChange = GetVerticalRotation(startVec, currentVec);

        if (Math.Abs(horRotationChange.y) > Math.Abs(verRotationChange.x) && Math.Abs(horRotationChange.y) > Math.Abs(verRotationChange.z))
        {
            var rotation = NormalizeRotation(ComposeRotations(horRotationChange, startTerrainRotation));
            return rotation;
        }
        else
        {
            var rotation = NormalizeRotation(ComposeRotations(verRotationChange, startTerrainRotation));

            var boundaryAngle = 10f;
            if (Math.Abs(rotation.x) < boundaryAngle && Math.Abs(rotation.z) < boundaryAngle)
            {
                rotation.x = 0;
                rotation.z = 0;
            }

            //does not work
            //rotation = Vector3.Max(rotation, new Vector3(-30, rotation.y, -30));
            //rotation = Vector3.Min(rotation, new Vector3(30, rotation.y, 30));


            return rotation;
        }
    }

    private Vector3 GetHorizontalRotation(Vector3 startVec, Vector3 currentVec)
    {
        var horCurrVec = Vector3.ProjectOnPlane(currentVec, Vector3.up);
        var horStartVec = Vector3.ProjectOnPlane(startVec, Vector3.up);
        var horRotationChange = Quaternion.FromToRotation(horStartVec, horCurrVec).eulerAngles;
        horRotationChange = NormalizeRotation(horRotationChange);

        return horRotationChange;
    }
    private Vector3 GetVerticalRotation(Vector3 startVec, Vector3 currentVec)
    {
        var verPlaneNormal = Vector3.Cross(Vector3.up, startVec);
        var verStartVec = Vector3.ProjectOnPlane(startVec, verPlaneNormal);
        var verCurrVec = Vector3.ProjectOnPlane(currentVec, verPlaneNormal);
        var verRotationChange = Quaternion.FromToRotation(verStartVec, verCurrVec).eulerAngles;
        verRotationChange = NormalizeRotation(verRotationChange);

        return verRotationChange;
    }

    private Vector3 ComposeRotations(Vector3 r1, Vector3 r2)
    {
        var rotation = (Quaternion.Euler(r1) * Quaternion.Euler(r2)).eulerAngles;
        return rotation;
    }

    private Vector3 NormalizeRotation(Vector3 rotation)
    {
        var normalizedRotation = new Vector3();
        normalizedRotation.x = NormalizeAngle(rotation.x);
        normalizedRotation.y = NormalizeAngle(rotation.y);
        normalizedRotation.z = NormalizeAngle(rotation.z);

        return normalizedRotation;
    }
    private float NormalizeAngle(float angle)
    {
        while (angle <= -180) angle += 360;
        while (angle > 180) angle -= 360;

        return angle;
    }
}
