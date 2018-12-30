using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;


[System.Serializable]
public class BaseShaders
{
    public ComputeShader marchingShader;
    public ComputeShader normalsShader;
    public ComputeShader clearShader;

}

[System.Serializable]
public class ModelSchaders : BaseShaders
{
    public ComputeShader brushShader;
}

[System.Serializable]
public class TerrainShaders : BaseShaders
{
    public ComputeShader brushColorShader;
    public ComputeShader brushShapeShader;
    public ComputeShader ExtremeValueShader;
}



