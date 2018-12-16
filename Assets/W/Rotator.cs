using Assets.MarchingCubesGPU.Scripts;
using MarchingCubesGPUProject;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator
{
    public Vector3 offset = new Vector3(0, -2, 0);

    public Vector3 GetRotation_1(Vector3 startTerrainRotation, Vector3 startPos, Vector3 currentPos)
    {
        var angleVertex = startPos + offset;
        var startVec = startPos - angleVertex;
        var currentVec = currentPos - angleVertex;

        var rotationChange = Quaternion.FromToRotation(startVec, currentVec).eulerAngles;
        var rotation = startTerrainRotation + rotationChange;

        return rotation;
    }

    public Vector3 GetRotation_2(Vector3 startTerrainRotation, Vector3 startPos, Vector3 currentPos)
    {
        var angleVertex = startPos + offset;
        var startVec = startPos - angleVertex;
        var currentVec = currentPos - angleVertex;

        var rotationChange = Quaternion.FromToRotation(startVec, currentVec).eulerAngles;
        var rotation = startTerrainRotation;

        rotation.x += rotationChange.x;
        rotation.z += rotationChange.z;

        return rotation;
    }

    public Vector3 GetRotation_3(Vector3 startTerrainRotation, Vector3 startPos, Vector3 currentPos)
    {
        var angleVertex = startPos + offset;
        var startVec = startPos - angleVertex;
        var currentVec = currentPos - angleVertex;

        var rotationChange = Quaternion.FromToRotation(startVec, currentVec).eulerAngles;
        var rotation = startTerrainRotation;

        rotation.x += rotationChange.x;
        rotation.z += rotationChange.z;

        rotation = NormalizeRotation(rotation);

        return rotation;
    }

    public Vector3 GetRotation_4(Vector3 startTerrainRotation, Vector3 startPos, Vector3 currentPos)
    {
        var angleVertex = startPos + offset;
        var startVec = startPos - angleVertex;
        var currentVec = currentPos - angleVertex;

        var rotationChange = Quaternion.FromToRotation(startVec, currentVec).eulerAngles;
        var rotation = startTerrainRotation;

        rotation.x += rotationChange.x;
        rotation.z += rotationChange.z;

        rotation = NormalizeRotation(rotation);

        var boundaryAngle = 10f;
        if (Math.Abs(rotation.x) < boundaryAngle && Math.Abs(rotation.z) < boundaryAngle)
        {
            rotation.x = 0;
            rotation.z = 0;
        }

        return rotation;
    }

    public Vector3 GetRotation_5(Vector3 startTerrainRotation, Vector3 startPos, Vector3 currentPos)
    {
        var angleVertex = startPos + offset;
        var startVec = startPos - angleVertex;
        var currentVec = currentPos - angleVertex;

        var rotationChange = Quaternion.FromToRotation(startVec, currentVec).eulerAngles;
        Debug.Log(rotationChange);
        var rotation = startTerrainRotation;

        rotation.x += rotationChange.x;
        rotation.z += rotationChange.z;

        rotation = NormalizeRotation(rotation);

        var boundaryAngle = 10f;
        if (Math.Abs(rotation.x) < boundaryAngle && Math.Abs(rotation.z) < boundaryAngle)
        {
            rotation.x = 0;
            rotation.z = 0;
        }

        rotation = Vector3.Max(rotation, new Vector3(-90, rotation.y, -90));
        rotation = Vector3.Min(rotation, new Vector3(90, rotation.y, 90));

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
