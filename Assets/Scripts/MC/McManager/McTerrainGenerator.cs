using System;
using System.Linq;
using UnityEngine;

public class McTerrainGenerator : McBaseGenerator
{
    public McTerrainGenerator(BaseShaders shaders, Material material)
        : base(shaders, McConsts.TerrN, McConsts.TerrN * McConsts.TerrN, material)
    {
    }

    public override McData GetDefaultData()
    {
        var data = new McData();
        data.Guid = Guid.NewGuid();
        data.Colors = Enumerable.Repeat(new Vector4(0.8f, 0.6f, 0.35f, 1), DesiredBufferSize).ToArray();

        data.Values = new float[DesiredBufferSize];
        for (int z = 0; z < N; z++)
            for (int x = 0; x < N; x++)
            {
                data.Values[x + z * N] = 0;

                if (x != 0 && z != 0 && x != N - 1 && z != N - 1)
                {
                    data.Values[x + z * N] = 11;
                }
            }

        return data;
    }
}
