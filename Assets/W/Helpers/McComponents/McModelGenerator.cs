using MarchingCubesGPUProject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class McModelGenerator : McBaseGenerator
{
    protected override int N
    { get { return McConsts.ModelN; } }

    protected override int DesiredBufferSize
    { get { return N * N * N; } }


    public McModelGenerator(BaseShaders shaders, Material material)
        : base(shaders, material)
    {
    }

    public override McData GetDefaultData()
    {
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
}
