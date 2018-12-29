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


    public ComputeShader m_marchingCubes;
    public ComputeShader m_normals;
    public ComputeShader m_clearBuffer;
    public Material material;

    void Start()
    {
        startRotation = transform.rotation.eulerAngles;
        startPosition = transform.position;

        int N = McConsts.TerrN;

        var generator = new McTerrainGenerator(m_marchingCubes, m_normals, m_clearBuffer, material);

        var values = new float[N * N];
        for (int z = 0; z < N; z++)
            for (int x = 0; x < N; x++)
            {
                values[x + z * N] = 0;

                if (x != 0 && z != 0 && x != N - 1 && z != N - 1)
                {
                    values[x + z * N] = 2;
                }
            }

        var Colordata = new Vector4[N * N];

        for (int x = 0; x < N; x++)
            for (int z = 0; z < N; z++)
            {
                Colordata[x + z * N] = new Vector4(1, 1, 1, 1);
            }

        var data = new McData();
        data.Colors = Colordata;
        data.Values = values;

        var obj = generator.GetMeshes(data);
        obj.transform.parent = this.transform;
    }

    void Update()
    {

        //var result = new RotationHelper().GetRotation(transform.position,
        //                                            startRotation,
        //                                            startCube.transform.position,
        //                                            currentCube.transform.position);
        //transform.rotation = Quaternion.Euler(result);

        var result = new OperationService().GetPosition(startPosition, startCube.transform.position, currentCube.transform.position);
        transform.position = result;
    }
}
