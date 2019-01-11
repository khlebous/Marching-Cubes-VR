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

        public ModelBrush brush;

        public ModelSchaders Shaders;
        public Material material;

        private GPURenderer _renderer;
        
        private Mesh[] _meshes;
        private Transform[] _meshObjTransforms;
        private McVert[] _verts = new McVert[Size];

        private void Start()
        {
            //There are 8 threads run per group so N must be divisible by 8.
            if (N % 8 != 0)
                throw new System.ArgumentException("N must be divisible be 8");

            InitMeshes();
            _renderer = new GPURenderer(Shaders, N, N * N * N);

            //first calculation
            _renderer.CleanMeshBuffer();
            _renderer.CalculateMesh(this.transform);
            _renderer.CalculateNormals();
            UpdateMeshes();
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

        private void Update()
        {
            if (brush.mode == BrushMode.Inactive)
                return;

            _renderer.CleanMeshBuffer();
            CalculateChanges();
            _renderer.CalculateMesh(this.transform);
            _renderer.CalculateNormals();
            UpdateMeshes();
        }

        private void CalculateChanges()
        {
            Shaders.brushShader.SetInt("_Width", N);
            Shaders.brushShader.SetInt("_Height", N);
            Shaders.brushShader.SetInt("_Depth", N);

            Shaders.brushShader.SetVector("_Scale", transform.lossyScale);
            Shaders.brushShader.SetBuffer(0, "_Voxels", _renderer.dataBuffer);
            Shaders.brushShader.SetBuffer(0, "_VoxelColors", _renderer.dataColorBuffer);

            Shaders.brushShader.SetVector("_BrushColor", brush.color);
            Shaders.brushShader.SetInt("_BrushMode", (int)brush.mode);
            Shaders.brushShader.SetInt("_BrushShape", (int)brush.shape);

            Shaders.brushShader.SetFloats("_FromMcToBrushMatrix", GetFromMcToBrushMatrix().ToFloats());
            Shaders.brushShader.SetVector("_BrushScale", brush.transform.lossyScale);

            Shaders.brushShader.Dispatch(0, N / 8, N / 8, N / 8);
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
            _renderer.ReleaseBuffers();
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
            _renderer.meshBuffer.GetData(_verts);
            //var a = mmeshBuffer.GetNativeBufferPtr();

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
            Guid = data.Guid;

            _renderer.InitDataBuffer();
            _renderer.InitDataColorBuffer();

            _renderer.dataBuffer.SetData(data.Values);
            _renderer.dataColorBuffer.SetData(data.Colors);
        }
        public McData GetData()
        {
            var data = new McData();
            data.Guid = Guid;
            data.Values = new float[N * N * N];
            data.Colors = new Vector4[N * N * N];

            _renderer.dataBuffer.GetData(data.Values);
            _renderer.dataColorBuffer.GetData(data.Colors);

            return data;
        }

        public void Destroy()
        {
            GameObject.Destroy(gameObject);
        }
    }
}





























