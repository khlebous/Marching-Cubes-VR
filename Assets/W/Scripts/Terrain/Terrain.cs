using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

#pragma warning disable 162


using System.Collections.Generic;
using System.Linq;
using Assets.MarchingCubesGPU.Scripts;
using System.Runtime.InteropServices;
using System;

namespace MarchingCubesGPUProject
{
    public class Terrain : MonoBehaviour
    {
        //The size of the voxel array for each dimension
        const int N = 32;
        const int meshCount = 20;

        //The size of the buffer that holds the verts.
        //This is the maximum number of verts that the 
        //marching cube can produce, 5 triangles for each voxel.
        const int SIZE = N * N * N * 3 * 5;

        public Material m_drawBuffer;

        public TerrainBrush brush;

        public ComputeShader m_brushColorBuffer;
        public ComputeShader m_brushShapeBuffer;
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

        private Mesh[] meshes;

        private void Start()
        {

            //There are 8 threads run per group so N must be divisible by 8.
            if (N % 8 != 0)
                throw new System.ArgumentException("N must be divisible be 8");

            InitMeshes();

            InitDataBuffer();
            InitDataColorBuffer();
            InitNormalsBuffer();
            InitMeshBuffer();

            InitMarchingCubesTablesBuffors();

            //test
            //StartShaping();
        }

        private void InitMeshes()
        {
            meshes = new Mesh[meshCount];

            for (int i = 0; i < meshCount; i++)
            {
                Mesh mesh = new Mesh();
                mesh.bounds = new Bounds(new Vector3(0, N / 2, 0), new Vector3(N, N, N)); //what is it for?
                meshes[i] = mesh;

                GameObject go = new GameObject("Marching Mesh");
                go.AddComponent<MeshFilter>();
                go.AddComponent<MeshRenderer>();
                go.GetComponent<Renderer>().material = material;
                go.GetComponent<MeshFilter>().mesh = mesh;

                MeshCollider collider = go.AddComponent<MeshCollider>();
                collider.sharedMesh = mesh;

                go.transform.parent = transform;
                go.transform.SetPositionAndRotation(go.transform.parent.position, go.transform.parent.rotation);
            }
        }
        private void InitDataBuffer()
        {
            m_dataBuffer = new ComputeBuffer(N * N, sizeof(float));
            var data = new float[N * N];

            for (int z = 0; z < N; z++)
                for (int x = 0; x < N; x++)
                {
                    data[x + z * N] = 0;

                    if (x != 0 && z != 0 && x != N - 1 && z != N - 1)
                    {
                        if (z < 5)
                            data[x + z * N] = 1f;
                    }
                }

            m_dataBuffer.SetData(data);
        }
        private void InitDataColorBuffer()
        {
            m_dataColorBuffer = new ComputeBuffer(N * N * 4, 4 * sizeof(float));
            var data = new Vector4[N * N];

            for (int x = 0; x < N; x++)
                for (int z = 0; z < N; z++)
                {
                    //data[x + z * N] = new Vector4(0, 0, 1, 1);
                    //if (x > 3)
                    //    data[x + z * N] = new Vector4(1, 0, 0, 1);

                    data[x + z * N] = new Vector4(1, 1, 1, 1);

                }

            m_dataColorBuffer.SetData(data);

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
            //m_meshBuffer = new ComputeBuffer(SIZE, sizeof(float) * 7);
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

        //shaping
        public bool Shaping;
        public Vector3 StartShapingBrushPosition;
        public Vector3 LastShapingBrushPosition;

        public void StartShaping()
        {
            Shaping = true;
            StartShapingBrushPosition = brush.transform.position;
            LastShapingBrushPosition = brush.transform.position;
        }
        public void FinishShaping()
        {
            Shaping = false;
        }
        private float GetShapingHeight()
        {
            Vector3 diff = brush.transform.position - LastShapingBrushPosition;
            var mcUp = transform.rotation * Vector3.up;
            var changeVector = Vector3.Project(diff, mcUp);
            var changeHeight = changeVector.y;

            return changeHeight / this.transform.lossyScale.y;
        }

        private void Update()
        {
            if (brush.mode != TerrainBrushMode.Inactive)
            {

                UpdateBrushRotation();
                CleanMeshBuffer();
                CalculateChanges();
                CalculateNormals();
                CalculateMesh();

                UpdateMeshes();
            }

            if (Shaping)
                LastShapingBrushPosition = brush.transform.position;
        }

        private void UpdateBrushRotation()
        {
            brush.transform.rotation = this.transform.rotation;
        }
        private void CleanMeshBuffer()
        {
            m_clearBuffer.SetInt("_Width", N);
            m_clearBuffer.SetInt("_Height", N);
            m_clearBuffer.SetInt("_Depth", N);
            m_clearBuffer.SetBuffer(0, "_Buffer", m_meshBuffer);

            m_clearBuffer.Dispatch(0, N / 8, N / 8, N / 8);
        }

        private void CalculateChanges()
        {
            if (brush.mode == TerrainBrushMode.Color)
                CalculateColoring();
            else
                CalculateShaping();
        }
        private void CalculateColoring()
        {
            m_brushColorBuffer.SetInt("_Width", N);
            m_brushColorBuffer.SetInt("_Height", N);
            m_brushColorBuffer.SetInt("_Depth", N);

            m_brushColorBuffer.SetVector("_Scale", transform.lossyScale);
            m_brushColorBuffer.SetBuffer(0, "_VoxelColors", m_dataColorBuffer);

            var fromMcToBrushMatrix = brush.GetToBrushMatrix(brush.transform.position) * GetFromMcMatrix();
            m_brushColorBuffer.SetFloats("_FromMcToBrushMatrix", fromMcToBrushMatrix.ToFloats());

            m_brushColorBuffer.SetInt("_BrushShape", (int)brush.shape);
            m_brushColorBuffer.SetVector("_BrushColor", brush.color);

            m_brushColorBuffer.Dispatch(0, N / 8, 1, N / 8);
        }
        private void CalculateShaping()
        {
            m_brushShapeBuffer.SetInt("_Width", N);
            m_brushShapeBuffer.SetInt("_Height", N);
            m_brushShapeBuffer.SetInt("_Depth", N);

            m_brushShapeBuffer.SetVector("_Scale", transform.lossyScale);
            m_brushShapeBuffer.SetBuffer(0, "_Voxels", m_dataBuffer);

            var fromMcToBrushMatrix = brush.GetToBrushMatrix(StartShapingBrushPosition) * GetFromMcMatrix();
            m_brushShapeBuffer.SetFloats("_FromMcToBrushMatrix", fromMcToBrushMatrix.ToFloats());

            m_brushShapeBuffer.SetInt("_BrushShape", (int)brush.shape);
            m_brushShapeBuffer.SetFloat("_HeightChange", GetShapingHeight());

            m_brushShapeBuffer.Dispatch(0, N / 8, 1, N / 8);
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
        private void CalculateMesh()
        {
            m_marchingCubes.SetInt("_Width", N);
            m_marchingCubes.SetInt("_Height", N);
            m_marchingCubes.SetInt("_Depth", N);
            m_marchingCubes.SetVector("_Scale", this.transform.lossyScale);
            m_marchingCubes.SetVector("_BrushColor", brush.color);
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

        private void OnDestroy()
        {
            //MUST release buffers.
            m_dataBuffer.Release();
            m_dataColorBuffer.Release();
            m_meshBuffer.Release();
            m_cubeEdgeFlags.Release();
            m_triangleConnectionTable.Release();
            m_normalsBuffer.Release();
        }

        //private Matrix4x4 GetToMcMatrix()
        //{
        //    var mcPosition = Matrix4x4.Translate(-this.transform.position);
        //    var mcRotation = Matrix4x4.Rotate(Quaternion.Inverse(this.transform.rotation));
        //    var mcOffsetTranslation = Matrix4x4.Translate(new Vector3(N - 1, 0, N - 1) / 2); // N-1 is triangle number
        //    var mcScale = Matrix4x4.Scale(this.transform.lossyScale).inverse;

        //    var result = mcScale * mcOffsetTranslation * mcRotation * mcPosition;
        //    return result;
        //}
        private Matrix4x4 GetFromMcMatrix()
        {
            var mcOffsetTranslation = Matrix4x4.Translate(new Vector3(-(N - 1), 0, -(N - 1)) / 2);// N-1 is triangle number
            var mcScale = Matrix4x4.Scale(this.transform.lossyScale);
            var mcRotation = Matrix4x4.Rotate(this.transform.rotation);
            var mcPosition = Matrix4x4.Translate(this.transform.position);

            var result = mcPosition * mcRotation * mcScale * mcOffsetTranslation;
            return result;
        }

        struct Vert
        {
            public Vector4 position;
            public Vector3 normal;
            public Vector4 color;
        };

        /// <summary>
        /// Reads back the mesh data from the GPU and turns it into a standard unity mesh.
        /// </summary>
        /// <returns></returns>
        /// 

        private Vert[] verts = new Vert[SIZE];
        private void UpdateMeshes()
        {
            //Get the data out of the buffer.
            m_meshBuffer.GetData(verts);
            //var a = m_meshBuffer.GetNativeBufferPtr();

            var positions = new List<Vector3>();
            var normals = new List<Vector3>();
            var colors = new List<Color>();
            List<int> indexes = new List<int>();

            int meshIdx = 0;
            int idx = 0;
            //int maxTriangles = 65000 / 3;
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
                    meshes[meshIdx].Clear();
                    meshes[meshIdx].SetVertices(positions);
                    meshes[meshIdx].SetNormals(normals);
                    meshes[meshIdx].SetColors(colors);
                    meshes[meshIdx].SetTriangles(indexes, 0);

                    idx = 0;
                    positions.Clear();
                    normals.Clear();
                    colors.Clear();
                    indexes.Clear();

                    meshIdx++;
                }
            }

            meshes[meshIdx].Clear();
            meshes[meshIdx].SetVertices(positions);
            meshes[meshIdx].SetNormals(normals);
            meshes[meshIdx].SetColors(colors);
            meshes[meshIdx].SetTriangles(indexes, 0);
        }
    }
}





























