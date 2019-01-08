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

        public ModelSchaders Shaders;
        public Material material;

        public ComputeShader normalsShader2;

        private ComputeBuffer _dataBuffer;
        private ComputeBuffer _dataColorBuffer;
        private ComputeBuffer _meshBuffer;
        private RenderTexture _normalsBuffer;
        private ComputeBuffer _cubeEdgeFlags;
        private ComputeBuffer _triangleConnectionTable;

        private Mesh[] _meshes;
        private Transform[] _meshObjTransforms;
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

            //first calculation
            //CleanMeshBuffer();
            //CalculateNormals();
            //CalculateMesh();
            //UpdateMeshes();
        }


        private void InitMeshes()
        {
            _meshes = new Mesh[meshCount];
            _meshObjTransforms = new Transform[meshCount];

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
                _meshObjTransforms[i] = go.transform;
            }
        }
        private void InitDataBuffer()
        {
            if (_dataBuffer != null)
                return;

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
            if (_dataColorBuffer != null)
                return;

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
            //CalculateNormals();
            CalculateMesh();
            CalculateNormals2();

            UpdateMeshes();
        }

        private void CleanMeshBuffer()
        {
            Shaders.clearShader.SetInt("_Width", N);
            Shaders.clearShader.SetInt("_Height", N);
            Shaders.clearShader.SetInt("_Depth", N);
            Shaders.clearShader.SetBuffer(0, "_Buffer", _meshBuffer);

            Shaders.clearShader.Dispatch(0, N / 8, N / 8, N / 8);
        }
        private void CalculateChanges()
        {
            Shaders.brushShader.SetInt("_Width", N);
            Shaders.brushShader.SetInt("_Height", N);
            Shaders.brushShader.SetInt("_Depth", N);

            Shaders.brushShader.SetVector("_Scale", transform.lossyScale);
            Shaders.brushShader.SetBuffer(0, "_Voxels", _dataBuffer);
            Shaders.brushShader.SetBuffer(0, "_VoxelColors", _dataColorBuffer);

            Shaders.brushShader.SetVector("_BrushColor", brush.color);
            Shaders.brushShader.SetInt("_BrushMode", (int)brush.mode);
            Shaders.brushShader.SetInt("_BrushShape", (int)brush.shape);

           Shaders.brushShader.SetFloats("_FromMcToBrushMatrix", GetFromMcToBrushMatrix().ToFloats());
            Shaders.brushShader.SetVector("_BrushScale", brush.transform.lossyScale);

            Shaders.brushShader.Dispatch(0, N / 8, N / 8, N / 8);
        }
        private void CalculateNormals()
        {
            Shaders.normalsShader.SetInt("_Width", N);
            Shaders.normalsShader.SetInt("_Height", N);
            Shaders.normalsShader.SetInt("_Depth", N);
            Shaders.normalsShader.SetBuffer(0, "_Voxels", _dataBuffer);
            Shaders.normalsShader.SetTexture(0, "_Result", _normalsBuffer);

            Shaders.normalsShader.Dispatch(0, N / 8, N / 8, N / 8);
        }
        private void CalculateNormals2()
        {
            normalsShader2.SetInt("_Width", N);
            normalsShader2.SetInt("_Height", N);
            normalsShader2.SetInt("_Depth", N);
            normalsShader2.SetBuffer(0, "_Buffer", _meshBuffer);

            normalsShader2.Dispatch(0, N / 8, N / 8, N / 8);
        }
        private void CalculateMesh()
        {
            Shaders.marchingShader.SetInt("_Width", N);
            Shaders.marchingShader.SetInt("_Height", N);
            Shaders.marchingShader.SetInt("_Depth", N);
            Shaders.marchingShader.SetVector("_Scale", this.transform.lossyScale);
            Shaders.marchingShader.SetVector("_BrushColor", brush.color);
            Shaders.marchingShader.SetInt("_Border", 0); // strange but works
                                                         //m_marchingCubes.SetInt("_Border", 1);
            Shaders.marchingShader.SetFloat("_Target", 0.5f);//!!!!! values [0,1]
            Shaders.marchingShader.SetBuffer(0, "_Voxels", _dataBuffer);
            Shaders.marchingShader.SetBuffer(0, "_VoxelColors", _dataColorBuffer);
            //Shaders.marchingShader.SetTexture(0, "_Normals", _normalsBuffer);
            Shaders.marchingShader.SetBuffer(0, "_Buffer", _meshBuffer);
            Shaders.marchingShader.SetBuffer(0, "_CubeEdgeFlags", _cubeEdgeFlags);
            Shaders.marchingShader.SetBuffer(0, "_TriangleConnectionTable", _triangleConnectionTable);

            Shaders.marchingShader.Dispatch(0, N / 8, N / 8, N / 8);
        }

        private Matrix4x4 GetFromMcToBrushMatrix()
        {
            //var adjustBrushScale = Matrix4x4.Scale(Vector3.one.Divide(transform.lossyScale));
            var adjustBrushScale = Matrix4x4.Scale(transform.lossyScale).inverse;
            var fromMcToBrushMatrix = adjustBrushScale * brush.GetToBrushMatrix() * GetFromMcMatrix();

            return fromMcToBrushMatrix;
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
            EnsureProperMeshScaling();

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
        private void EnsureProperMeshScaling()
        {
            var scale = this.transform.localScale;
            var meshScale = new Vector3(1 / scale.x, 1 / scale.y, 1 / scale.z);

            foreach (var tran in _meshObjTransforms)
            {
                tran.localScale = meshScale;
            }
        }

        public void SetData(McData data)
        {
            InitDataBuffer();
            InitDataColorBuffer();

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

		public void Destroy()
		{
			GameObject.Destroy(gameObject);
		}
	}
}





























