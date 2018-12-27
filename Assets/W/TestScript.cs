using Assets.MarchingCubesGPU.Scripts;
using MarchingCubesGPUProject;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public Transform startCube;
    public Transform currentCube;

    private Vector3 startRotation;
    private Vector3 startPosition;

    void Start()
    {
        startRotation = transform.rotation.eulerAngles;
        startPosition = transform.position;

    }

    void Update()
    {
        //var result = new RotationHelper().GetRotation(transform.position,
        //                                            startRotation,
        //                                            startCube.transform.position,
        //                                            currentCube.transform.position);
        //transform.rotation = Quaternion.Euler(result);

        var result = new MovementService().GetPosition(startPosition, startCube.transform.position, currentCube.transform.position);
        transform.position = result;
    }
}
