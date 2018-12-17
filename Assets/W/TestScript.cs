using Assets.MarchingCubesGPU.Scripts;
using MarchingCubesGPUProject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public Transform startCube;
    public Transform currentCube;

    private Vector3 startRotation;

    void Start()
    {
        startRotation = transform.rotation.eulerAngles;
    }

    void Update()
    {
        var result = new Rotator().GetRotation_5(transform.position,
                                                    startRotation,
                                                    startCube.transform.position,
                                                    currentCube.transform.position);
        transform.rotation = Quaternion.Euler(result);

    }
}
