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

        var loader = new McLoader();
        var data = new McData();
        data.Values = new Vector4[] { new Vector4(1, 2, 3, 4) };
        data.Colors = new Vector4[] { new Vector4(1, 1, 1, 1) };

        loader.SaveObj("qweTest", data);
        var loadedData = loader.LoadObj("qweTest");
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
