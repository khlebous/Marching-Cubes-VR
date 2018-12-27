using Assets.MarchingCubesGPU.Scripts;
using MarchingCubesGPUProject;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementService
{
    public Vector3 GetPosition(Vector3 objPos, Vector3 startConPos, Vector3 currentConPos)
    {
        var positionChange = currentConPos - startConPos;

        return objPos + positionChange;
    }
}
