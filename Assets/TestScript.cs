using Assets.MarchingCubesGPU.Scripts;
using MarchingCubesGPUProject;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestScript : MonoBehaviour
{

    private EditableModel model;
    private GameObject sphere;
    private GameObject cuboid;

    // Use this for initialization
    void Start()
    {
        model = this.gameObject.GetComponent<EditableModel>();
        sphere = model.brush.sphereMesh.gameObject;
        cuboid = model.brush.cubeMesh.gameObject;
        model.SetData(GetDefaultData());
    }

    public McData GetDefaultData()
    {
        int N = McConsts.ModelN;
        var DesiredBufferSize = N * N * N;

        var data = new McData();
        data.Guid = Guid.NewGuid();
        data.Colors = Enumerable.Repeat(new Vector4(0, 0.5f, 1, 1), DesiredBufferSize).ToArray();

        data.Values = new float[DesiredBufferSize];
        for (int z = 0; z < N; z++)
            for (int y = 0; y < N; y++)
                for (int x = 0; x < N; x++)
                {
                    data.Values[x + y * N + z * N * N] = 0;

                    if (x != 0 && y != 0 && z != 0 && x != N - 1 && y != N - 1 && z != N - 1)
                    {
                        if ((x <= 1 || x >= N - 2) && (y <= 1 || y >= N - 2) && (z <= 1 || z >= N - 2)
                            || x >= N / 2 && x <= N / 2 + 1 && y >= N / 2 && y <= N / 2 + 1 && z >= N / 2 && z <= N / 2 + 1)
                            //if (y > 0 && y < 2 && x > 1 && x < N && z > 1 && z < N)
                            data.Values[x + y * N + z * N * N] = 1.0f;
                    }
                }


        return data;
    }

    // Update is called once per frame
    void Update()
    {
        var shape = model.brush.shape;

        if (shape == BrushShape.Sphere && !sphere.activeSelf)
        {
            sphere.SetActive(true);
            cuboid.SetActive(false);
        }


        if (shape == BrushShape.Cuboid && !cuboid.activeSelf)
        {
            cuboid.SetActive(true);
            sphere.SetActive(false);
        }
    }
}
