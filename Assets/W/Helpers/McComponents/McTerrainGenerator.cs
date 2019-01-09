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

public class McTerrainGenerator : McBaseGenerator
{
    protected override int N
    { get { return McConsts.TerrN; } }

    protected override int DesiredBufferSize
    { get { return N * N; } }


    public McTerrainGenerator(BaseShaders shaders, Material material)
        : base(shaders, material)
    {
    }

    public override McData GetDefaultData()
    {
        var data = new McData();
        data.Guid = Guid.NewGuid();
        data.Colors = Enumerable.Repeat(new Vector4(1, 1, 1, 1), DesiredBufferSize).ToArray();
        data.Values = new float[DesiredBufferSize];

        for (int z = 0; z < N; z++)
            for (int x = 0; x < N; x++)
            {
                data.Values[x + z * N] = 0;

                if (x != 0 && z != 0 && x != N - 1 && z != N - 1)
                {
                    data.Values[x + z * N] = 2;
                }
            }

        return data;
    }
}
