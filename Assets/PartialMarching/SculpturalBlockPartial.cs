using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.marching2.MarchingCubes.PartialMarching
{

    public class SculpturalBlockPartial : MonoBehaviour
    {
        [SerializeField] private GameObject brush;
        [SerializeField] private GameObject sphere;

        public Material Material;

        public Vector3 Size = new Vector3(32, 32, 32);
        public Vector3 CubeSize = new Vector3(1, 1, 1);

        private IMarching _marching;

        private bool[] _voxels;
        private Vector3Int _voxelCount;
        private Vector3 _meshOffset;
        private List<GameObject> _meshes;

        private bool[] _changes;
        private List<Vector3>[] _cubeVertices;
        private bool _hasChanges = false;


        public GameObject Player;
        public Slider Slider;

        private void Start()
        {
            Slider.value = CubeSize.x;
            Slider.minValue = CubeSize.x;
            Slider.maxValue = Size.x/2;

            Initialize();
            UpdateShape();

            var quadmesh = brush.GetComponent<MeshFilter>().mesh;
            var ver = quadmesh.vertices;
            for (int i = 0; i < ver.Length; i++)
            {
                Debug.Log(i + ": " + ver[i]);
            }

            SetBrushMesh();
        }

        private void SetBrushMesh()
        {
            float value = Slider.value;// / 2;

            Vector3[] newVert = new Vector3[4];
            newVert[0] = new Vector3(-value, -value, 0);
            newVert[1] = new Vector3(value, value, 0);
            newVert[2] = new Vector3(value, -value, 0);
            newVert[3] = new Vector3(-value, value, 0);
            
            brush.GetComponent<MeshFilter>().mesh.vertices = newVert;

            sphere.transform.localScale = new Vector3(2*value, 2*value, 2*value);
        }

        private void Update()
        {
            if (_hasChanges)
            {
                _hasChanges = false;
                UpdateShape();
            }

            if (OVRInput.Get(OVRInput.Button.Two))
            {
                Vector3 loc = Player.transform.Find("TrackingSpace").transform.TransformPoint(OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch));
                SetValue(true, loc, Slider.value);
            }
            else if (OVRInput.Get(OVRInput.Button.One))
            {
                Vector3 loc = Player.transform.Find("TrackingSpace").transform.TransformPoint(OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch));
                SetValue(false, loc, Slider.value);
            }

            if (OVRInput.Get(OVRInput.Button.Four))
            {
                Vector3 loc = Player.transform.Find("TrackingSpace").transform.TransformPoint(OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch));
                SetValue(true, loc, Slider.value);
            }
            else if (OVRInput.Get(OVRInput.Button.Three))
            {
                Vector3 loc = Player.transform.Find("TrackingSpace").transform.TransformPoint(OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch));
                SetValue(false, loc, Slider.value);
            }

            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
            {
                Slider.value += 0.01f;
                SetBrushMesh();

            }
            else if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger))
            {
                Slider.value -= 0.01f;
                SetBrushMesh();
            }
        }


        public void SetValue(bool value, Vector3 position, float radius)
        {
            position = transform.InverseTransformPoint(position);

            for (int x = 0; x < _voxelCount.x; x++)
                for (int y = 0; y < _voxelCount.y; y++)
                    for (int z = 0; z < _voxelCount.z; z++)
                    {
                        if (Vector3.Distance(position, GetPointPosition(x, y, z)) <= radius)
                            SetValueOfIndex(value, x, y, z);
                    }
        }

        public void SetValueWithBounds(bool value, Vector3 position, float radius)
        {
            position = transform.InverseTransformPoint(position);
            var localPosition = (position - _meshOffset);

            var minPosition = localPosition - new Vector3(radius, radius, radius);
            var maxPosition = localPosition + new Vector3(radius, radius, radius);

            var minBoundX = Math.Min(_voxelCount.x - 1, Math.Max(0, Mathf.CeilToInt(minPosition.x / CubeSize.x)));
            var minBoundY = Math.Min(_voxelCount.y - 1, Math.Max(0, Mathf.CeilToInt(minPosition.y / CubeSize.y)));
            var minBoundZ = Math.Min(_voxelCount.z - 1, Math.Max(0, Mathf.CeilToInt(minPosition.z / CubeSize.z)));

            var maxBoundX = Math.Min(_voxelCount.x - 1, Math.Max(0, Mathf.FloorToInt(maxPosition.x / CubeSize.x)));
            var maxBoundY = Math.Min(_voxelCount.y - 1, Math.Max(0, Mathf.FloorToInt(maxPosition.y / CubeSize.y)));
            var maxBoundZ = Math.Min(_voxelCount.z - 1, Math.Max(0, Mathf.FloorToInt(maxPosition.z / CubeSize.z)));

            for (int x = minBoundX; x < maxBoundX; x++)
                for (int y = minBoundY; y < maxBoundY; y++)
                    for (int z = minBoundZ; z < maxBoundZ; z++)
                    {
                        if (Vector3.Distance(position, GetPointPosition(x, y, z)) <= radius)
                            SetValueOfIndex(value, x, y, z);
                    }
        }

        private Vector3 GetPointPosition(int x, int y, int z)
        {
            return _meshOffset + new Vector3(x * CubeSize.x, y * CubeSize.y, z * CubeSize.z);
        }

        public void SetValueOfIndex(bool value, int x, int y, int z)
        {
            if (CheckEdgees(x, y, z) || _voxels[GetIndex(x, y, z)] == value) return;

            _voxels[GetIndex(x, y, z)] = value;
            _changes[GetIndex(x, y, z)] = true;
            _hasChanges = true;
        }
        private int GetIndex(int x, int y, int z)
        {
            return x + y * _voxelCount.x + z * _voxelCount.x * _voxelCount.y;
        }
        private bool CheckEdgees(int x, int y, int z)
        {
            return x == 0 || y == 0 || z == 0 || x == _voxelCount.x - 1 || y == _voxelCount.y - 1 || z == _voxelCount.z - 1;
        }

        private void Initialize()
        {
            _marching = new MarchingCubes();
            _meshes = new List<GameObject>();

            _meshOffset = new Vector3(-Size.x / 2, -Size.y / 2, -Size.z / 2);
            _voxelCount = new Vector3Int(Convert.ToInt32(Size.x / CubeSize.x) + 1, Convert.ToInt32(Size.y / CubeSize.y) + 1, Convert.ToInt32(Size.z / CubeSize.z) + 1);

            _voxels = new bool[_voxelCount.x * _voxelCount.y * _voxelCount.z];
            _changes = Enumerable.Repeat(true, _voxelCount.x * _voxelCount.y * _voxelCount.z).ToArray();

            _cubeVertices = new List<Vector3>[(_voxelCount.x - 1) * (_voxelCount.y - 1) * (_voxelCount.z - 1)];
            for (int i = 0; i < _cubeVertices.Length; i++)
                _cubeVertices[i] = new List<Vector3>(15);


            for (int x = 0; x < _voxelCount.x; x++)
                for (int y = 0; y < _voxelCount.y; y++)
                    for (int z = 0; z < _voxelCount.z; z++)
                            _voxels[GetIndex(x, y, z)] = CheckEdgees(x, y, z);


        }

        private void UpdateShape()
        {
            foreach (var meshObj in _meshes)
                Destroy(meshObj);


            _marching.Generate(_voxels, _changes, _voxelCount, CubeSize, _cubeVertices);

            for (int i = 0; i < _changes.Length; i++)
                _changes[i] = false;


            //A mesh in unity can only be made up of 65000 verts.
            //Need to split the verts between multiple meshes.

            int maxVertsPerMesh = 30000; //must be divisible by 3, ie 3 verts == 1 triangle

            List<Vector3> verts = _cubeVertices.SelectMany(x => x).ToList();
            int numMeshes = verts.Count / maxVertsPerMesh + 1;

            for (int i = 0; i < numMeshes; i++)
            {
                List<Vector3> splitVerts = new List<Vector3>();
                List<int> splitIndices = new List<int>();

                for (int j = 0; j < maxVertsPerMesh; j++)
                {
                    int idx = i * maxVertsPerMesh + j;

                    if (idx < verts.Count)
                    {
                        splitVerts.Add(verts[idx] + _meshOffset);
                        splitIndices.Add(j);
                    }
                }

                if (splitVerts.Count == 0) continue;

                Mesh mesh = new Mesh();
                mesh.SetVertices(splitVerts);
                mesh.SetTriangles(splitIndices, 0);
                mesh.RecalculateBounds();
                mesh.RecalculateNormals();

                GameObject go = new GameObject("Mesh");

                go.transform.parent = transform;
                go.transform.position = new Vector3();
                go.transform.rotation = new Quaternion();
                go.transform.position = transform.position;

                go.AddComponent<MeshFilter>();
                go.AddComponent<MeshRenderer>();
                go.GetComponent<Renderer>().material = Material;
                go.GetComponent<MeshFilter>().mesh = mesh;

                _meshes.Add(go);
            }
        }
    }
}
