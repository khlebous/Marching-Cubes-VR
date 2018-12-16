using Assets.MarchingCubesGPU.Scripts;
using MarchingCubesGPUProject;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator
{
    public Vector3 GetRotation_5(Vector3 startTerrainPosition, Vector3 startTerrainRotation, Vector3 startPos, Vector3 currentPos)
    {
        var startVec = startPos - startTerrainPosition;
        var currentVec = currentPos - startTerrainPosition;

        var horCurrVec = Vector3.ProjectOnPlane(currentVec, Vector3.up);
        var horRotationChange = Quaternion.FromToRotation(startVec, horCurrVec).eulerAngles;
        horRotationChange = NormalizeRotation(horRotationChange);

        var verPlaneNormal = Vector3.Cross(Vector3.up, startVec);
        var verCurrVec = Vector3.ProjectOnPlane(currentVec, verPlaneNormal);
        var verRotationChange = Quaternion.FromToRotation(startVec, verCurrVec).eulerAngles;
        verRotationChange = NormalizeRotation(verRotationChange);

        Debug.Log(horRotationChange);
        Debug.Log(verRotationChange);

        if (Math.Abs(horRotationChange.y) > Math.Abs(verRotationChange.x) && Math.Abs(horRotationChange.y) > Math.Abs(verRotationChange.z))
        {
            var rotation = startTerrainRotation + horRotationChange;
            rotation = NormalizeRotation(rotation);

            return rotation;
        }
        else
        {
            var rotation = startTerrainRotation + verRotationChange;
            rotation = NormalizeRotation(rotation);

            var boundaryAngle = 10f;
            if (Math.Abs(rotation.x) < boundaryAngle && Math.Abs(rotation.z) < boundaryAngle)
            {
                rotation.x = 0;
                rotation.y -= verRotationChange.y;
                rotation.z = 0;
            }

            rotation = Vector3.Max(rotation, new Vector3(-90, rotation.y, -90));
            rotation = Vector3.Min(rotation, new Vector3(90, rotation.y, 90));

            return rotation;
        }
    }

    //public Vector3 GetRotation_5(Vector3 startTerrainPosition, Vector3 startTerrainRotation, Vector3 startPos, Vector3 currentPos)
    //{
    //    var startVec = startPos - startTerrainPosition;
    //    var currentVec = currentPos - startTerrainPosition;

    //    var rotationChange = Quaternion.FromToRotation(startVec, currentVec).eulerAngles;
    //    rotationChange = NormalizeRotation(rotationChange);

    //    if (Math.Abs(rotationChange.y) > Math.Abs(rotationChange.x) && Math.Abs(rotationChange.y) > Math.Abs(rotationChange.z))
    //    {

    //        var projCurrVec = Vector3.ProjectOnPlane(currentVec, Vector3.up);
    //        var horRotationChange = Quaternion.FromToRotation(startVec, projCurrVec).eulerAngles;

    //        var rotation = startTerrainRotation;
    //        rotation += horRotationChange;
    //        rotation = NormalizeRotation(rotation);

    //        return rotation;
    //    }
    //    else
    //    {
    //        var horPlaneNormal = Vector3.Cross(Vector3.up, startVec);
    //        var projCurrVec = Vector3.ProjectOnPlane(currentVec, Vector3.Cross(Vector3.up, startVec));
    //        var verRotationChange = Quaternion.FromToRotation(startVec, projCurrVec).eulerAngles;

    //        var rotation = startTerrainRotation + verRotationChange;
    //        rotation = NormalizeRotation(rotation);

    //        var boundaryAngle = 10f;
    //        if (Math.Abs(rotation.x) < boundaryAngle && Math.Abs(rotation.z) < boundaryAngle)
    //        {
    //            rotation.x = 0;
    //            rotation.y -= verRotationChange.y;
    //            rotation.z = 0;
    //        }

    //        rotation = Vector3.Max(rotation, new Vector3(-90, rotation.y, -90));
    //        rotation = Vector3.Min(rotation, new Vector3(90, rotation.y, 90));

    //        return rotation;
    //    }
    //}

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

    //public Vector3 offset = new Vector3(0, -2, 0);

    //public Vector3 GetRotation_5(Vector3 startTerrainRotation, Vector3 startPos, Vector3 currentPos)
    //{
    //    var angleVertex = startPos + offset;
    //    var startVec = startPos - angleVertex;
    //    var currentVec = currentPos - angleVertex;

    //    var rotationChange = Quaternion.FromToRotation(startVec, currentVec).eulerAngles;
    //    var rotation = startTerrainRotation;

    //    rotation.x += rotationChange.x;
    //    rotation.z += rotationChange.z;

    //    rotation = NormalizeRotation(rotation);

    //    var boundaryAngle = 10f;
    //    if (Math.Abs(rotation.x) < boundaryAngle && Math.Abs(rotation.z) < boundaryAngle)
    //    {
    //        rotation.x = 0;
    //        rotation.z = 0;
    //    }

    //    rotation = Vector3.Max(rotation, new Vector3(-90, rotation.y, -90));
    //    rotation = Vector3.Min(rotation, new Vector3(90, rotation.y, 90));

    //    return rotation;
    //}
}
