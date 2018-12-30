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
}
