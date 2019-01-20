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

        public TerrainBrush brush;

        public TerrainShaders Shaders;
        public Material material;

        private ComputeBuffer _extremeValueBuffer;
        private GPURenderer _renderer;

        private Transform[] _meshObjTransforms;
        private Mesh[] _meshes;
        private McVert[] _verts = new McVert[Size];

        private void Start()
        {
            //There are 8 threads run per group so N must be divisible by 8.
            if (N % 8 != 0)
                throw new System.ArgumentException("N must be divisible be 8");

            InitMeshes();
            if (_renderer == null)
                _renderer = new GPURenderer(Shaders, N, N * N);
            InitExtremeValueBuffer();

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
        private void InitExtremeValueBuffer()
        {
            _extremeValueBuffer = new ComputeBuffer(1, sizeof(float));
        }

        //shaping
        public bool Shaping = false;
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
            EnsureBrushRotation();
            EnsureProperBrushMesh();
            EnsureProperBrushPosition();

            if (brush.mode != TerrainBrushMode.Inactive)
            {
                _renderer.CleanMeshBuffer();
                CalculateChanges();
                _renderer.CalculateMesh(this.transform);
                _renderer.CalculateNormals();
                UpdateMeshes();
            }

            LastShapingBrushPosition = brush.transform.position;

            //test
            StartShaping();
        }

        private void EnsureBrushRotation()
        {
            brush.transform.rotation = this.transform.rotation;
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
            Shaders.brushColorShader.SetInt("_Width", N);
            Shaders.brushColorShader.SetInt("_Height", N);
            Shaders.brushColorShader.SetInt("_Depth", N);

            Shaders.brushColorShader.SetVector("_Scale", transform.lossyScale);
            Shaders.brushColorShader.SetBuffer(0, "_VoxelColors", _renderer.dataColorBuffer);

            Shaders.brushColorShader.SetFloats("_FromMcToBrushMatrix", GetFromMcToBrushMatrix(brush.transform.position).ToFloats());

            Shaders.brushColorShader.SetInt("_BrushShape", (int)brush.shape);
            Shaders.brushColorShader.SetVector("_BrushColor", brush.color);

            Shaders.brushColorShader.Dispatch(0, N / 8, 1, N / 8);
        }
        private void CalculateShaping()
        {
            Shaders.brushShapeShader.SetInt("_Width", N);
            Shaders.brushShapeShader.SetInt("_Height", N);
            Shaders.brushShapeShader.SetInt("_Depth", N);

            Shaders.brushShapeShader.SetVector("_Scale", transform.lossyScale);
            Shaders.brushShapeShader.SetBuffer(0, "_Voxels", _renderer.dataBuffer);

            Shaders.brushShapeShader.SetFloats("_FromMcToBrushMatrix", GetFromMcToBrushMatrix(StartShapingBrushPosition).ToFloats());

            Shaders.brushShapeShader.SetInt("_BrushShape", (int)brush.shape);
            Shaders.brushShapeShader.SetInt("_BrushMode", (int)brush.mode);
            Shaders.brushShapeShader.SetFloat("_HeightChange", GetShapingHeight());

            if (brush.mode == TerrainBrushMode.ExtremeChange)
            {
                CalculateExtremeValue();
                Shaders.brushShapeShader.SetBuffer(0, "_ExtremeValue", _extremeValueBuffer);
            }
            Shaders.brushShapeShader.Dispatch(0, N / 8, 1, N / 8);
        }
        private void CalculateExtremeValue()
        {
            Shaders.ExtremeValueShader.SetInt("_Width", N);
            Shaders.ExtremeValueShader.SetInt("_Height", N);
            Shaders.ExtremeValueShader.SetInt("_Depth", N);

            Shaders.ExtremeValueShader.SetBuffer(0, "_Voxels", _renderer.dataBuffer);

            Shaders.ExtremeValueShader.SetFloats("_FromMcToBrushMatrix", GetFromMcToBrushMatrix(StartShapingBrushPosition).ToFloats());

            Shaders.ExtremeValueShader.SetInt("_BrushShape", (int)brush.shape);
            Shaders.ExtremeValueShader.SetFloat("_HeightChange", GetShapingHeight());
            Shaders.ExtremeValueShader.SetBuffer(0, "_ExtremeValue", _extremeValueBuffer);

            Shaders.ExtremeValueShader.Dispatch(0, 1, 1, 1);
        }


        private Matrix4x4 GetFromMcToBrushMatrix(Vector3 brushPosition)
        {
            var adjustBrushScale = Matrix4x4.Scale(transform.lossyScale).inverse;
            var fromMcToBrushMatrix = adjustBrushScale * brush.GetToBrushMatrix(brushPosition) * GetFromMcMatrix();
            return fromMcToBrushMatrix;
        }

        private void OnDestroy()
        {
            _renderer.ReleaseBuffers();
            _extremeValueBuffer.Release();
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
        private void EnsureProperBrushMesh()
        {
            var toMcMatrix = Matrix4x4.Rotate(Quaternion.Inverse(this.transform.rotation)) * Matrix4x4.Translate(-this.transform.position);
            var fromMcMatrix = Matrix4x4.Translate(this.transform.position) * Matrix4x4.Rotate(this.transform.rotation);

            var brushPoint = toMcMatrix * brush.transform.position.ToVector4();
            if (brushPoint.y >= this.transform.lossyScale.y)
                brushPoint.y = this.transform.lossyScale.y;
            else
                brushPoint.y += this.transform.lossyScale.y / 100;
            brushPoint = fromMcMatrix * brushPoint;

            var unscaledDist = Vector3.Distance(brush.transform.position, brushPoint.ToVector3());
            var dist = unscaledDist / brush.transform.localScale.y;

            brush.cylinderMesh.transform.localPosition = new Vector3(0, -dist / 2f, 0);
            brush.cylinderMesh.transform.localScale = new Vector3(this.transform.lossyScale.x, dist / 2f, this.transform.lossyScale.z);

            brush.cubeMesh.transform.localPosition = new Vector3(0, -dist / 2f, 0);
            brush.cubeMesh.transform.localScale = new Vector3(this.transform.lossyScale.x, dist, this.transform.lossyScale.z);
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

        public bool IsInTestVersion;
        private void EnsureProperBrushPosition()
        {
            if (IsInTestVersion)
                return;

            var offset = 0.1f;
            var terrainFactor = Math.Max(this.transform.lossyScale.z, this.transform.lossyScale.x);
            var brushFactor = Math.Max(brush.transform.lossyScale.z, brush.transform.lossyScale.x);
            var dist = offset + terrainFactor * brushFactor / 2;
            brush.transform.localPosition = new Vector3(0, 0, dist);
        }

        public void SetData(McData data)
        {
            Guid = data.Guid;

            if (Shaders == null)
                throw new ArgumentException("renderer");

            if (_renderer == null)
                _renderer = new GPURenderer(Shaders, N, N * N);

            _renderer.dataBuffer.SetData(data.Values);
            _renderer.dataColorBuffer.SetData(data.Colors);
        }
        public McData GetData()
        {
            var data = new McData();
            data.Guid = Guid;
            data.Values = new float[N * N];
            data.Colors = new Vector4[N * N];

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


