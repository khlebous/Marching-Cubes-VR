using UnityEngine;
using UnityEngine.Rendering;

#pragma warning disable 162

using System.Collections.Generic;
using Assets.MarchingCubesGPU.Scripts;
using System;

namespace MarchingCubesGPUProject
{
    public class EditableModel : MonoBehaviour
    {
        public Guid Guid { get; set; }

        const int N = McConsts.ModelN;
        const int meshCount = McConsts.MeshCount;
        const int Size = N * N * N * 3 * 5;

        public Material drawBuffer;
        public ModelBrush brush;

        public ComputeShader brushShader;
        public ComputeShader marchingShader;
        public ComputeShader normalsShader;
        public ComputeShader clearShader;
        public Material material;

        private ComputeBuffer _dataBuffer;
        private ComputeBuffer _dataColorBuffer;
        private ComputeBuffer _meshBuffer;
        private RenderTexture _normalsBuffer;
        private ComputeBuffer _cubeEdgeFlags;
        private ComputeBuffer _triangleConnectionTable;

        private Mesh[] _meshes;
        private McVert[] _verts = new McVert[Size];

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
        }


        private void InitMeshes()
        {
            _meshes = new Mesh[meshCount];

            for (int i = 0; i < meshCount; i++)
            {
                Mesh mesh = new Mesh();
                mesh.bounds = new Bounds(new Vector3(0, N / 2, 0), new Vector3(N, N, N)); //what is it for?
                _meshes[i] = mesh;

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
            _dataBuffer = new ComputeBuffer(N * N * N, sizeof(float));
            var data = new float[N * N * N];

            for (int z = 0; z < N; z++)
                for (int y = 0; y < N; y++)
                    for (int x = 0; x < N; x++)
                    {
                        data[x + y * N + z * N * N] = 0;

                        if (x != 0 && y != 0 && z != 0 && x != N - 1 && y != N - 1 && z != N - 1)
                        {
                            if (y > 0 && y < 2 && x > 1 && x < N && z > 1 && z < N)
                                data[x + y * N + z * N * N] = 0.51f;
                        }
                    }


            _dataBuffer.SetData(data);
        }
        private void InitDataColorBuffer()
        {
            _dataColorBuffer = new ComputeBuffer(N * N * N * 4, 4 * sizeof(float));
            var data = new Vector4[N * N * N];

            for (int z = 0; z < N; z++)
                for (int y = 0; y < N; y++)
                    for (int x = 0; x < N; x++)
                    {
                        data[x + y * N + z * N * N] = new Vector4(0.7f, 0.7f, 0.7f, 1);
                        //if (x > 3)
                        //    data[x + y * N + z * N * N] = new Vector4(1, 0, 0, 1);
                    }

            _dataColorBuffer.SetData(data);
        }
        private void InitNormalsBuffer()
        {
            //Holds the normals of the voxels.
            _normalsBuffer = new RenderTexture(N, N, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
            _normalsBuffer.dimension = TextureDimension.Tex3D;
            _normalsBuffer.enableRandomWrite = true;
            _normalsBuffer.useMipMap = false;
            _normalsBuffer.volumeDepth = N;
            _normalsBuffer.Create();
        }
        private void InitMeshBuffer()
        {
            //m_meshBuffer = new ComputeBuffer(SIZE, sizeof(float) * 7);
            _meshBuffer = new ComputeBuffer(Size, sizeof(float) * 11);
            CleanMeshBuffer();
        }
        private void InitMarchingCubesTablesBuffors()
        {
            //These two buffers are just some settings needed by the marching cubes.
            _cubeEdgeFlags = new ComputeBuffer(256, sizeof(int));
            _cubeEdgeFlags.SetData(MarchingCubesTables.CubeEdgeFlags);
            _triangleConnectionTable = new ComputeBuffer(256 * 16, sizeof(int));
            _triangleConnectionTable.SetData(MarchingCubesTables.TriangleConnectionTable);
        }

        private void Update()
        {
            if (brush.mode == BrushMode.Inactive)
                return;

            CleanMeshBuffer();
            CalculateChanges();
            CalculateNormals();
            CalculateMesh();

            UpdateMeshes();
        }

        private void CleanMeshBuffer()
        {
            clearShader.SetInt("_Width", N);
            clearShader.SetInt("_Height", N);
            clearShader.SetInt("_Depth", N);
            clearShader.SetBuffer(0, "_Buffer", _meshBuffer);

            clearShader.Dispatch(0, N / 8, N / 8, N / 8);
        }
        private void CalculateChanges()
        {
            brushShader.SetInt("_Width", N);
            brushShader.SetInt("_Height", N);
            brushShader.SetInt("_Depth", N);

            brushShader.SetVector("_Scale", transform.lossyScale);
            brushShader.SetBuffer(0, "_Voxels", _dataBuffer);
            brushShader.SetBuffer(0, "_VoxelColors", _dataColorBuffer);

            brushShader.SetVector("_BrushColor", brush.color);
            brushShader.SetInt("_BrushMode", (int)brush.mode);
            brushShader.SetInt("_BrushShape", (int)brush.shape);

            var fromMcToBrushMatrix = brush.GetToBrushMatrix() * GetFromMcMatrix();
            brushShader.SetFloats("_FromMcToBrushMatrix", fromMcToBrushMatrix.ToFloats());
            brushShader.SetVector("_BrushScale", brush.transform.lossyScale);

            brushShader.Dispatch(0, N / 8, N / 8, N / 8);
        }
        private void CalculateNormals()
        {
            normalsShader.SetInt("_Width", N);
            normalsShader.SetInt("_Height", N);
            normalsShader.SetInt("_Depth", N);
            normalsShader.SetBuffer(0, "_Voxels", _dataBuffer);
            normalsShader.SetTexture(0, "_Result", _normalsBuffer);

            normalsShader.Dispatch(0, N / 8, N / 8, N / 8);
        }
        private void CalculateMesh()
        {
            marchingShader.SetInt("_Width", N);
            marchingShader.SetInt("_Height", N);
            marchingShader.SetInt("_Depth", N);
            marchingShader.SetVector("_Scale", this.transform.lossyScale);
            marchingShader.SetVector("_BrushColor", brush.color);
            marchingShader.SetInt("_Border", 0); // strange but works
                                                  //m_marchingCubes.SetInt("_Border", 1);
            marchingShader.SetFloat("_Target", 0.5f);//!!!!! values [0,1]
            marchingShader.SetBuffer(0, "_Voxels", _dataBuffer);
            marchingShader.SetBuffer(0, "_VoxelColors", _dataColorBuffer);
            marchingShader.SetTexture(0, "_Normals", _normalsBuffer);
            marchingShader.SetBuffer(0, "_Buffer", _meshBuffer);
            marchingShader.SetBuffer(0, "_CubeEdgeFlags", _cubeEdgeFlags);
            marchingShader.SetBuffer(0, "_TriangleConnectionTable", _triangleConnectionTable);

            marchingShader.Dispatch(0, N / 8, N / 8, N / 8);
        }

        private void OnDestroy()
        {
            //MUST release buffers.
            _dataBuffer.Release();
            _dataColorBuffer.Release();
            _meshBuffer.Release();
            _cubeEdgeFlags.Release();
            _triangleConnectionTable.Release();
            _normalsBuffer.Release();
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

        private void UpdateMeshes()
        {
            //Get the data out of the buffer.
            _meshBuffer.GetData(_verts);
            //var a = m_meshBuffer.GetNativeBufferPtr();

            var positions = new List<Vector3>();
            var normals = new List<Vector3>();
            var colors = new List<Color>();
            List<int> indexes = new List<int>();

            int meshIdx = 0;
            int idx = 0;
            //int maxTriangles = 65000 / 3;
            int maxVerts = 65000;

            for (int i = 0; i < Size; i++)
            {
                //If the marching cubes generated a vert for this index
                //then the position w value will be not -1.
                if (_verts[i].position.w != -1)
                {
                    positions.Add(_verts[i].position);
                    normals.Add(_verts[i].normal);
                    colors.Add(_verts[i].color);
                    indexes.Add(idx++);
                }

                if (idx >= maxVerts)
                {
                    _meshes[meshIdx].Clear();
                    _meshes[meshIdx].SetVertices(positions);
                    _meshes[meshIdx].SetNormals(normals);
                    _meshes[meshIdx].SetColors(colors);
                    _meshes[meshIdx].SetTriangles(indexes, 0);

                    idx = 0;
                    positions.Clear();
                    normals.Clear();
                    colors.Clear();
                    indexes.Clear();

                    meshIdx++;
                }
            }

            _meshes[meshIdx].Clear();
            _meshes[meshIdx].SetVertices(positions);
            _meshes[meshIdx].SetNormals(normals);
            _meshes[meshIdx].SetColors(colors);
            _meshes[meshIdx].SetTriangles(indexes, 0);
        }

        public void SetData(McData data)
        {
            Guid = data.Guid;
            _dataBuffer.SetData(data.Values);
            _dataColorBuffer.SetData(data.Colors);
        }
        public McData GetData()
        {
            var data = new McData();
            data.Guid = Guid;
            data.Values = new float[N * N * N];
            data.Colors = new Vector4[N * N * N];

            _dataBuffer.GetData(data.Values);
            _dataColorBuffer.GetData(data.Colors);

            return data;
        }
    }
}





























