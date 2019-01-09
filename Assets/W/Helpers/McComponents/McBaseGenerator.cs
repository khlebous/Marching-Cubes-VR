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

public abstract class McBaseGenerator
{
    protected abstract int N { get; }
    protected abstract int DesiredBufferSize { get; }

    protected int Size
    { get { return N * N * N * 3 * 5; } }


    private McVert[] _verts;

    public BaseShaders _shaders;
    public Material material;

    private ComputeBuffer dataBuffer;
    private ComputeBuffer dataColorBuffer;
    private ComputeBuffer meshBuffer;
    private RenderTexture normalsBuffer;
    private ComputeBuffer cubeEdgeFlags;
    private ComputeBuffer triangleConnectionTable;

    public McBaseGenerator(BaseShaders shaders, Material material)
    {
        _shaders = shaders;
        this.material = material;

        //There are 8 threads run per group so N must be divisible by 8.
        if (N % 8 != 0)
            throw new System.ArgumentException("N must be divisible be 8");

        _verts = new McVert[Size];

        dataBuffer = new ComputeBuffer(DesiredBufferSize, sizeof(float));
        dataColorBuffer = new ComputeBuffer(4 * DesiredBufferSize, 4 * sizeof(float));

        InitNormalsBuffer();
        InitMeshBuffer();
        InitMarchingCubesTablesBuffors();
    }
    public abstract McData GetDefaultData();
    public GameObject GenerateMeshes(McData data)
    {
        if (data.Values.Length != DesiredBufferSize)
            throw new System.ArgumentException("Values array should have " + DesiredBufferSize + " elements");

        if (data.Colors.Length != DesiredBufferSize)
            throw new System.ArgumentException("Colors array should have " + DesiredBufferSize + " elements");

        dataBuffer.SetData(data.Values);
        dataColorBuffer.SetData(data.Colors);

        GameObject meshesObject = new GameObject("Marching Meshes");

        CleanMeshBuffer();
        CalculateNormals();
        CalculateMesh(meshesObject.transform);

        CreateMeshes(meshesObject.transform);

        return meshesObject;
    }
    public void ReleaseBuffers()
    {
        dataBuffer.Release();
        dataColorBuffer.Release();
        meshBuffer.Release();
        normalsBuffer.Release();
        cubeEdgeFlags.Release();
        triangleConnectionTable.Release();
    }

    private void InitNormalsBuffer()
    {
        //Holds the normals of the voxels.
        normalsBuffer = new RenderTexture(N, N, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
        normalsBuffer.dimension = TextureDimension.Tex3D;
        normalsBuffer.enableRandomWrite = true;
        normalsBuffer.useMipMap = false;
        normalsBuffer.volumeDepth = N;
        normalsBuffer.Create();
    }
    private void InitMeshBuffer()
    {
        meshBuffer = new ComputeBuffer(Size, sizeof(float) * 11);
        CleanMeshBuffer();
    }
    private void InitMarchingCubesTablesBuffors()
    {
        //These two buffers are just some settings needed by the marching cubes.
        cubeEdgeFlags = new ComputeBuffer(256, sizeof(int));
        cubeEdgeFlags.SetData(MarchingCubesTables.CubeEdgeFlags);
        triangleConnectionTable = new ComputeBuffer(256 * 16, sizeof(int));
        triangleConnectionTable.SetData(MarchingCubesTables.TriangleConnectionTable);
    }

    private void CleanMeshBuffer()
    {
        _shaders.clearShader.SetInt("_Width", N);
        _shaders.clearShader.SetInt("_Height", N);
        _shaders.clearShader.SetInt("_Depth", N);
        _shaders.clearShader.SetBuffer(0, "_Buffer", meshBuffer);

        _shaders.clearShader.Dispatch(0, N / 8, N / 8, N / 8);
    }
    private void CalculateNormals()
    {
        _shaders.normalsShader.SetInt("_Width", N);
        _shaders.normalsShader.SetInt("_Height", N);
        _shaders.normalsShader.SetInt("_Depth", N);
        _shaders.normalsShader.SetBuffer(0, "_Voxels", dataBuffer);
        _shaders.normalsShader.SetTexture(0, "_Result", normalsBuffer);

        _shaders.normalsShader.Dispatch(0, N / 8, N / 8, N / 8);
    }
    private void CalculateMesh(Transform parent)
    {
        _shaders.marchingShader.SetInt("_Width", N);
        _shaders.marchingShader.SetInt("_Height", N);
        _shaders.marchingShader.SetInt("_Depth", N);
        _shaders.marchingShader.SetVector("_Scale", parent.transform.lossyScale);
        _shaders.marchingShader.SetInt("_Border", 0); // strange but works
                                              //m_marchingCubes.SetInt("_Border", 1);
        _shaders.marchingShader.SetFloat("_Target", 0.5f);//!!!!! values [0,1]
        _shaders.marchingShader.SetBuffer(0, "_Voxels", dataBuffer);
        _shaders.marchingShader.SetBuffer(0, "_VoxelColors", dataColorBuffer);
        _shaders.marchingShader.SetTexture(0, "_Normals", normalsBuffer);
        _shaders.marchingShader.SetBuffer(0, "_Buffer", meshBuffer);
        _shaders.marchingShader.SetBuffer(0, "_CubeEdgeFlags", cubeEdgeFlags);
        _shaders.marchingShader.SetBuffer(0, "_TriangleConnectionTable", triangleConnectionTable);

        _shaders.marchingShader.Dispatch(0, N / 8, N / 8, N / 8);
    }

    private Mesh InitMesh(Transform parent)
    {
        Mesh mesh = new Mesh();
        mesh.bounds = new Bounds(new Vector3(0, N / 2, 0), new Vector3(N, N, N)); //what is it for?
        
        GameObject go = new GameObject("Marching Mesh");
        go.AddComponent<MeshFilter>();
        go.AddComponent<MeshRenderer>();
        go.GetComponent<Renderer>().material = material;
        go.GetComponent<MeshFilter>().mesh = mesh;

        MeshCollider collider = go.AddComponent<MeshCollider>();
        collider.sharedMesh = mesh;

        go.transform.parent = parent;
        go.transform.SetPositionAndRotation(go.transform.parent.position, go.transform.parent.rotation);


        return mesh;
    }
    private void CreateMeshes(Transform parent)
    {
        //Get the data out of the buffer.
        meshBuffer.GetData(_verts);

        var positions = new List<Vector3>();
        var normals = new List<Vector3>();
        var colors = new List<Color>();
        List<int> indexes = new List<int>();

        int meshIdx = 0;
        int idx = 0;
        int maxVerts = 65000;

        for (int i = 0; i < Size; i++)
        {
            //If the marching cubes generated a vert for this index
            //then the position w value will be 1, not -1.
            if (_verts[i].position.w != -1)
            {
                positions.Add(_verts[i].position);
                normals.Add(_verts[i].normal);
                colors.Add(_verts[i].color);
                indexes.Add(idx++);
            }

            if (idx >= maxVerts)
            {
                Mesh mesh = InitMesh(parent);
                mesh.Clear();
                mesh.SetVertices(positions);
                mesh.SetNormals(normals);
                mesh.SetColors(colors);
                mesh.SetTriangles(indexes, 0);

                idx = 0;
                positions.Clear();
                normals.Clear();
                colors.Clear();
                indexes.Clear();

                meshIdx++;
            }
        }

        Mesh lastMesh = InitMesh(parent);
        lastMesh.Clear();
        lastMesh.SetVertices(positions);
        lastMesh.SetNormals(normals);
        lastMesh.SetColors(colors);
        lastMesh.SetTriangles(indexes, 0);

    }
}
