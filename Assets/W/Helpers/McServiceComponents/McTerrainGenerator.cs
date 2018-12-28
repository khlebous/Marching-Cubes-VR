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


public static class Settings
{
    public const int TerrN = 16;
    public const int TerrSize = TerrN * TerrN * TerrN * 3 * 5;
}
public class McTerrainGenerator
{
    //The size of the voxel array for each dimension
    const int N = 16;

    //The size of the buffer that holds the verts.
    //This is the maximum number of verts that the 
    //marching cube can produce, 5 triangles for each voxel.
    const int SIZE = N * N * N * 3 * 5;

    private McVert[] verts = new McVert[SIZE];

    public ComputeShader m_marchingCubes;
    public ComputeShader m_normals;
    public ComputeShader m_clearBuffer;
    public Material material;

    private ComputeBuffer m_dataBuffer;
    private ComputeBuffer m_dataColorBuffer;
    private ComputeBuffer m_meshBuffer;
    private RenderTexture m_normalsBuffer;
    private ComputeBuffer m_cubeEdgeFlags;
    private ComputeBuffer m_triangleConnectionTable;


    public GameObject GetTerrainMeshes(McData data)
    {
        if (data.Values.Length != N * N)
            throw new System.ArgumentException("Values array should have " + N * N + " elements");

        if (data.Colors.Length != N * N)
            throw new System.ArgumentException("Colors array should have " + N * N + " elements");

        m_dataBuffer = new ComputeBuffer(N * N, 4 * sizeof(float));
        m_dataBuffer.SetData(data.Values);

        m_dataColorBuffer = new ComputeBuffer(N * N * 4, 4 * sizeof(float));
        m_dataColorBuffer.SetData(data.Colors);

        GameObject meshesObject = new GameObject("Marching Meshes");

        CleanMeshBuffer();
        CalculateNormals();
        CalculateMesh(meshesObject.transform);

        CreateMeshes(meshesObject.transform);


        return meshesObject;
    }

    public McTerrainGenerator(ComputeShader marchingShader, ComputeShader normalsShader, ComputeShader clearShader, Material material)
    {
        m_marchingCubes = marchingShader;
        m_normals = normalsShader;
        m_clearBuffer = clearShader;
        this.material = material;

    }

    public void Init()
    {
        //There are 8 threads run per group so N must be divisible by 8.
        if (N % 8 != 0)
            throw new System.ArgumentException("N must be divisible be 8");

        InitNormalsBuffer();
        InitMeshBuffer();
        InitMarchingCubesTablesBuffors();
    }

    private void InitNormalsBuffer()
    {
        //Holds the normals of the voxels.
        m_normalsBuffer = new RenderTexture(N, N, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
        m_normalsBuffer.dimension = TextureDimension.Tex3D;
        m_normalsBuffer.enableRandomWrite = true;
        m_normalsBuffer.useMipMap = false;
        m_normalsBuffer.volumeDepth = N;
        m_normalsBuffer.Create();
    }
    private void InitMeshBuffer()
    {
        m_meshBuffer = new ComputeBuffer(SIZE, sizeof(float) * 11);
        CleanMeshBuffer();
    }
    private void InitMarchingCubesTablesBuffors()
    {
        //These two buffers are just some settings needed by the marching cubes.
        m_cubeEdgeFlags = new ComputeBuffer(256, sizeof(int));
        m_cubeEdgeFlags.SetData(MarchingCubesTables.CubeEdgeFlags);
        m_triangleConnectionTable = new ComputeBuffer(256 * 16, sizeof(int));
        m_triangleConnectionTable.SetData(MarchingCubesTables.TriangleConnectionTable);
    }

    private void CleanMeshBuffer()
    {
        m_clearBuffer.SetInt("_Width", N);
        m_clearBuffer.SetInt("_Height", N);
        m_clearBuffer.SetInt("_Depth", N);
        m_clearBuffer.SetBuffer(0, "_Buffer", m_meshBuffer);

        m_clearBuffer.Dispatch(0, N / 8, N / 8, N / 8);
    }
    private void CalculateNormals()
    {
        m_normals.SetInt("_Width", N);
        m_normals.SetInt("_Height", N);
        m_normals.SetInt("_Depth", N);
        m_normals.SetBuffer(0, "_Voxels", m_dataBuffer);
        m_normals.SetTexture(0, "_Result", m_normalsBuffer);

        m_normals.Dispatch(0, N / 8, N / 8, N / 8);
    }
    private void CalculateMesh(Transform parent)
    {
        m_marchingCubes.SetInt("_Width", N);
        m_marchingCubes.SetInt("_Height", N);
        m_marchingCubes.SetInt("_Depth", N);
        m_marchingCubes.SetVector("_Scale", parent.transform.lossyScale);
        m_marchingCubes.SetInt("_Border", 0); // strange but works
                                              //m_marchingCubes.SetInt("_Border", 1);
        m_marchingCubes.SetFloat("_Target", 0.5f);//!!!!! values [0,1]
        m_marchingCubes.SetBuffer(0, "_Voxels", m_dataBuffer);
        m_marchingCubes.SetBuffer(0, "_VoxelColors", m_dataColorBuffer);
        m_marchingCubes.SetTexture(0, "_Normals", m_normalsBuffer);
        m_marchingCubes.SetBuffer(0, "_Buffer", m_meshBuffer);
        m_marchingCubes.SetBuffer(0, "_CubeEdgeFlags", m_cubeEdgeFlags);
        m_marchingCubes.SetBuffer(0, "_TriangleConnectionTable", m_triangleConnectionTable);

        m_marchingCubes.Dispatch(0, N / 8, N / 8, N / 8);
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
        m_meshBuffer.GetData(verts);


        var positions = new List<Vector3>();
        var normals = new List<Vector3>();
        var colors = new List<Color>();
        List<int> indexes = new List<int>();

        int meshIdx = 0;
        int idx = 0;
        int maxVerts = 65000;

        for (int i = 0; i < SIZE; i++)
        {
            //If the marching cubes generated a vert for this index
            //then the position w value will be 1, not -1.
            if (verts[i].position.w != -1)
            {
                positions.Add(verts[i].position);
                normals.Add(verts[i].normal);
                colors.Add(verts[i].color);
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
