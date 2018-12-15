using Assets.MarchingCubesGPU.Scripts;
using MarchingCubesGPUProject;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoatationTest
{
    public Vector3 GetRotation_1(Transform startTerrainTransform, Vector3 startPos, Vector3 currentPos)
    {
        var startVec = startPos - startTerrainTransform.position;
        var currentVec = currentPos - startTerrainTransform.position;

        var rotationChange = Quaternion.FromToRotation(startVec, currentVec).eulerAngles;
        var rotation = startTerrainTransform.rotation.eulerAngles + rotationChange;

        return rotation;
    }

    public Vector3 GetRotation_2(Transform startTerrainTransform, Vector3 startPos, Vector3 currentPos)
    {
        var startVec = startPos - startTerrainTransform.position;
        var currentVec = currentPos - startTerrainTransform.position;

        var rotationChange = Quaternion.FromToRotation(startVec, currentVec).eulerAngles;
        var rotation = startTerrainTransform.rotation.eulerAngles;
        rotation.y += rotationChange.y;

        return rotation;
    }


    public Vector3 GetRotation_3(Transform startTerrainTransform, Vector3 startPos, Vector3 currentPos)
    {
        var startVec = startPos - startTerrainTransform.position;
        var currentVec = currentPos - startTerrainTransform.position;

        var rotationChange = Quaternion.FromToRotation(startVec, currentVec).eulerAngles;
        var rotation = startTerrainTransform.rotation.eulerAngles;

        if (Math.Abs(rotationChange.y) > Math.Abs(rotationChange.x)
            && Math.Abs(rotationChange.y) > Math.Abs(rotationChange.z))
        {
            rotation.y += rotationChange.y;
        }
        else
        {
            rotation.x += rotationChange.x;
            rotation.z += rotationChange.z;
        }

        return rotation;
    }



    public Vector3 GetRotation_4(Transform startTerrainTransform, Vector3 startPos, Vector3 currentPos)
    {
        var startVec = startPos - startTerrainTransform.position;
        var currentVec = currentPos - startTerrainTransform.position;

        var rotationChange = Quaternion.FromToRotation(startVec, currentVec).eulerAngles;
        var rotation = startTerrainTransform.rotation.eulerAngles;

        if (Math.Abs(rotationChange.y) > Math.Abs(rotationChange.x)
            && Math.Abs(rotationChange.y) > Math.Abs(rotationChange.z))
        {
            rotation.y += rotationChange.y;
        }
        else
        {
            rotation.x += rotationChange.x;
            rotation.z += rotationChange.z;
        }

        var boundaryAngle = 10f;

        if (Math.Abs(rotation.x) < boundaryAngle && Math.Abs(rotation.z) < boundaryAngle)
        {
            rotation.x = 0;
            rotation.z = 0;
        }

        return rotation;
    }


    public Vector3 GetRotation_5(Transform startTerrainTransform, Vector3 startPos, Vector3 currentPos)
    {
        var startVec = startPos - startTerrainTransform.position;
        var currentVec = currentPos - startTerrainTransform.position;

        var rotationChange = Quaternion.FromToRotation(startVec, currentVec).eulerAngles;
        var rotation = startTerrainTransform.rotation.eulerAngles;


        if (Math.Abs(rotationChange.y) > Math.Abs(rotationChange.x)
            && Math.Abs(rotationChange.y) > Math.Abs(rotationChange.z))
        {
            rotation.y += rotationChange.y;
        }
        else
        {
            rotation.x += rotationChange.x;
            rotation.z += rotationChange.z;
        }

        var boundaryAngle = 10f;

        if (Math.Abs(rotation.x) < boundaryAngle && Math.Abs(rotation.z) < boundaryAngle)
        {
            rotation.x = 0;
            rotation.z = 0;
        }

        rotation = Vector3.Max(rotation, new Vector3(-90, rotation.x, -90));
        rotation = Vector3.Min(rotation, new Vector3(90, rotation.x, 90));

        return rotation;
    }
}
