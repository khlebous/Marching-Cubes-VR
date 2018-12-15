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
        var rotaiton = startTerrainTransform.rotation.eulerAngles + rotationChange;

        return rotaiton;
    }

    public Vector3 GetRotation_2(Transform startTerrainTransform, Vector3 startPos, Vector3 currentPos)
    {
        var startVec = startPos - startTerrainTransform.position;
        var currentVec = currentPos - startTerrainTransform.position;

        var rotationChange = Quaternion.FromToRotation(startVec, currentVec).eulerAngles;
        var rotaiton = startTerrainTransform.rotation.eulerAngles;
        rotaiton.y += rotationChange.y;

        return rotaiton;
    }


    public Vector3 GetRotation_3(Transform startTerrainTransform, Vector3 startPos, Vector3 currentPos)
    {
        var startVec = startPos - startTerrainTransform.position;
        var currentVec = currentPos - startTerrainTransform.position;

        var rotationChange = Quaternion.FromToRotation(startVec, currentVec).eulerAngles;
        var rotaiton = startTerrainTransform.rotation.eulerAngles;

        if (Math.Abs(rotationChange.y) > Math.Abs(rotationChange.x))
            rotaiton.y += rotationChange.y;
        else
            rotaiton.x += rotationChange.x;

        return rotaiton;
    }



    public Vector3 GetRotation_4(Transform startTerrainTransform, Vector3 startPos, Vector3 currentPos)
    {
        var startVec = startPos - startTerrainTransform.position;
        var currentVec = currentPos - startTerrainTransform.position;

        var rotationChange = Quaternion.FromToRotation(startVec, currentVec).eulerAngles;
        var rotaiton = startTerrainTransform.rotation.eulerAngles;

        if (Math.Abs(rotationChange.y) > Math.Abs(rotationChange.x))
            rotaiton.y += rotationChange.y;
        else
            rotaiton.x += rotationChange.x;

        return rotaiton;
    }


    public Vector3 GetRotation_5(Transform startTerrainTransform, Vector3 startPos, Vector3 currentPos)
    {
        var startVec = startPos - startTerrainTransform.position;
        var currentVec = currentPos - startTerrainTransform.position;

        var rotationChange = Quaternion.FromToRotation(startVec, currentVec).eulerAngles;
        var rotaiton = startTerrainTransform.rotation.eulerAngles;


        //sth with max
        if (Math.Abs(rotationChange.y) > Math.Abs(rotationChange.x))
            rotaiton.y += rotationChange.y;
        else
            rotaiton.x += rotationChange.x;

        return rotaiton;
    }
}
