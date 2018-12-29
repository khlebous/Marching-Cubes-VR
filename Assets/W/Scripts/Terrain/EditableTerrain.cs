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
    public class EditableTerrain : MonoBehaviour
    {
        public Guid Guid { get; set; }

        const int N = McConsts.ModelN;
        const int meshCount = McConsts.MeshCount;
        const int Size = N * N * N * 3 * 5;

        public Material drawBuffer;
        public TerrainBrush brush;

        public ComputeShader brushColorShader;
        public ComputeShader brushShapeShader;
        public ComputeShader marchingShader;
        public ComputeShader normalsShader;
        public ComputeShader clearShader;
        public ComputeShader ExtremeValueShader;
        public Material material;

        private ComputeBuffer _dataBuffer;
        private ComputeBuffer _dataColorBuffer;
        private ComputeBuffer _meshBuffer;
        private RenderTexture _normalsBuffer;
        private ComputeBuffer _cubeEdgeFlags;
        private ComputeBuffer _triangleConnectionTable;
        private ComputeBuffer _extremeValueBuffer;

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
            InitExtremeValueBuffer();


            //test
            //StartShaping();
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
            _dataBuffer = new ComputeBuffer(N * N, sizeof(float));
            var data = new float[N * N];

            for (int z = 0; z < N; z++)
                for (int x = 0; x < N; x++)
                {
                    data[x + z * N] = 0;

                    if (x != 0 && z != 0 && x != N - 1 && z != N - 1)
                    {
                        data[x + z * N] = 2;
                    }
                }

            _dataBuffer.SetData(data);
        }
        private void InitDataColorBuffer()
        {
            _dataColorBuffer = new ComputeBuffer(N * N * 4, 4 * sizeof(float));
            var data = new Vector4[N * N];
            
            for (int x = 0; x < N; x++)
                for (int z = 0; z < N; z++)
                {
                    //data[x + z * N] = new Vector4(0, 0, 1, 1);
                    //if (x > 3)
                    //    data[x + z * N] = new Vector4(1, 0, 0, 1);

                    data[x + z * N] = new Vector4(1, 1, 1, 1);

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
        private void InitExtremeValueBuffer()
        {
            _extremeValueBuffer = new ComputeBuffer(1, sizeof(float));
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
            clearShader.SetInt("_Width", N);
            clearShader.SetInt("_Height", N);
            clearShader.SetInt("_Depth", N);
            clearShader.SetBuffer(0, "_Buffer", _meshBuffer);

            clearShader.Dispatch(0, N / 8, N / 8, N / 8);
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
            brushColorShader.SetInt("_Width", N);
            brushColorShader.SetInt("_Height", N);
            brushColorShader.SetInt("_Depth", N);

            brushColorShader.SetVector("_Scale", transform.lossyScale);
            brushColorShader.SetBuffer(0, "_VoxelColors", _dataColorBuffer);

            var fromMcToBrushMatrix = brush.GetToBrushMatrix(brush.transform.position) * GetFromMcMatrix();
            brushColorShader.SetFloats("_FromMcToBrushMatrix", fromMcToBrushMatrix.ToFloats());

            brushColorShader.SetInt("_BrushShape", (int)brush.shape);
            brushColorShader.SetVector("_BrushColor", brush.color);

            brushColorShader.Dispatch(0, N / 8, 1, N / 8);
        }
        private void CalculateShaping()
        {
            brushShapeShader.SetInt("_Width", N);
            brushShapeShader.SetInt("_Height", N);
            brushShapeShader.SetInt("_Depth", N);

            brushShapeShader.SetVector("_Scale", transform.lossyScale);
            brushShapeShader.SetBuffer(0, "_Voxels", _dataBuffer);

            var fromMcToBrushMatrix = brush.GetToBrushMatrix(StartShapingBrushPosition) * GetFromMcMatrix();
            brushShapeShader.SetFloats("_FromMcToBrushMatrix", fromMcToBrushMatrix.ToFloats());

            brushShapeShader.SetInt("_BrushShape", (int)brush.shape);
            brushShapeShader.SetInt("_BrushMode", (int)brush.mode);
            brushShapeShader.SetFloat("_HeightChange", GetShapingHeight());

            if (brush.mode == TerrainBrushMode.ExtremeChange)
            {
                CalculateExtremeValue();
                brushShapeShader.SetBuffer(0, "_ExtremeValue", _extremeValueBuffer);
            }
            brushShapeShader.Dispatch(0, N / 8, 1, N / 8);
        }
        private void CalculateExtremeValue()
        {
            ExtremeValueShader.SetInt("_Width", N);
            ExtremeValueShader.SetInt("_Height", N);
            ExtremeValueShader.SetInt("_Depth", N);

            ExtremeValueShader.SetBuffer(0, "_Voxels", _dataBuffer);

            var fromMcToBrushMatrix = brush.GetToBrushMatrix(StartShapingBrushPosition) * GetFromMcMatrix();
            ExtremeValueShader.SetFloats("_FromMcToBrushMatrix", fromMcToBrushMatrix.ToFloats());

            ExtremeValueShader.SetInt("_BrushShape", (int)brush.shape);
            ExtremeValueShader.SetFloat("_HeightChange", GetShapingHeight());
            ExtremeValueShader.SetBuffer(0, "_ExtremeValue", _extremeValueBuffer);

            ExtremeValueShader.Dispatch(0, 1, 1, 1);
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


        /// <summary>
        /// Reads back the mesh data from the GPU and turns it into a standard unity mesh.
        /// </summary>
        /// <returns></returns>
        /// 

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
            data.Values = new float[N * N];
            data.Colors = new Vector4[N * N];

            _dataBuffer.GetData(data.Values);
            _dataColorBuffer.GetData(data.Colors);

            return data;
        }
    }
}





























