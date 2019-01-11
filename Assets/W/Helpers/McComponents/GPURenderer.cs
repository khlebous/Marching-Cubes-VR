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

public class GPURenderer
{
    public int N;
    public int DesiredBufferSize;

    public int Size
    { get { return N * N * N * 3 * 5; } }

    public McVert[] _verts;

    public BaseShaders shaders;

    public ComputeBuffer dataBuffer;
    public ComputeBuffer dataColorBuffer;
    public ComputeBuffer meshBuffer;
    public ComputeBuffer cubeEdgeFlags;
    public ComputeBuffer triangleConnectionTable;

    public GPURenderer(BaseShaders shaders, int n, int desiredBufferSize)
    {
        this.shaders = shaders;
        this.N = n;
        this.DesiredBufferSize = desiredBufferSize;

        //There are 8 threads run per group so N must be divisible by 8.
        if (N % 8 != 0)
            throw new System.ArgumentException("N must be divisible be 8");

        _verts = new McVert[Size];

        dataBuffer = new ComputeBuffer(DesiredBufferSize, sizeof(float));
        dataColorBuffer = new ComputeBuffer(4 * DesiredBufferSize, 4 * sizeof(float));

        InitMeshBuffer();
        InitMarchingCubesTablesBuffors();
    }
    public void ReleaseBuffers()
    {
        dataBuffer.Release();
        dataColorBuffer.Release();
        meshBuffer.Release();
        cubeEdgeFlags.Release();
        triangleConnectionTable.Release();
    }

    public void InitDataBuffer()
    {
        if (dataBuffer != null)
            return;

        dataBuffer = new ComputeBuffer(DesiredBufferSize, sizeof(float));
        var data = new float[DesiredBufferSize];
        dataBuffer.SetData(data);
    }
    public void InitDataColorBuffer()
    {
        if (dataColorBuffer != null)
            return;

        dataColorBuffer = new ComputeBuffer(DesiredBufferSize * 4, 4 * sizeof(float));
        var data = new Vector4[DesiredBufferSize];
        dataColorBuffer.SetData(data);
    }
    public void InitMeshBuffer()
    {
        meshBuffer = new ComputeBuffer(Size, sizeof(float) * 11);
        CleanMeshBuffer();
    }
    public void InitMarchingCubesTablesBuffors()
    {
        //These two buffers are just some settings needed by the marching cubes.
        cubeEdgeFlags = new ComputeBuffer(256, sizeof(int));
        cubeEdgeFlags.SetData(MarchingCubesTables.CubeEdgeFlags);
        triangleConnectionTable = new ComputeBuffer(256 * 16, sizeof(int));
        triangleConnectionTable.SetData(MarchingCubesTables.TriangleConnectionTable);
    }

    public void CleanMeshBuffer()
    {
        shaders.clearShader.SetInt("_Width", N);
        shaders.clearShader.SetInt("_Height", N);
        shaders.clearShader.SetInt("_Depth", N);
        shaders.clearShader.SetBuffer(0, "_Buffer", meshBuffer);

        shaders.clearShader.Dispatch(0, N / 8, N / 8, N / 8);
    }
    public void CalculateMesh(Transform parent)
    {
        shaders.marchingShader.SetInt("_Width", N);
        shaders.marchingShader.SetInt("_Height", N);
        shaders.marchingShader.SetInt("_Depth", N);
        shaders.marchingShader.SetVector("_Scale", parent.transform.lossyScale);
        shaders.marchingShader.SetInt("_Border", 0); // strange but works
                                              //m_marchingCubes.SetInt("_Border", 1);
        shaders.marchingShader.SetFloat("_Target", 0.5f);//!!!!! values [0,1]
        shaders.marchingShader.SetBuffer(0, "_Voxels", dataBuffer);
        shaders.marchingShader.SetBuffer(0, "_VoxelColors", dataColorBuffer);
        shaders.marchingShader.SetBuffer(0, "_Buffer", meshBuffer);
        shaders.marchingShader.SetBuffer(0, "_CubeEdgeFlags", cubeEdgeFlags);
        shaders.marchingShader.SetBuffer(0, "_TriangleConnectionTable", triangleConnectionTable);

        shaders.marchingShader.Dispatch(0, N / 8, N / 8, N / 8);
    }
    public void CalculateNormals()
    {
        shaders.normalsShader.SetInt("_Width", N);
        shaders.normalsShader.SetInt("_Height", N);
        shaders.normalsShader.SetInt("_Depth", N);
        shaders.normalsShader.SetBuffer(0, "_Buffer", meshBuffer);

        shaders.normalsShader.Dispatch(0, N / 8, N / 8, N / 8);
    }
}
