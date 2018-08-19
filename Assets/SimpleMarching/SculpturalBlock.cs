using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.marching2.MarchingCubes.SimpleMarching
{

    public class SculpturalBlock : MonoBehaviour
    {
        public Material Material;
        public int Seed = 0;

        public Vector3 Size = new Vector3(32, 32, 32);
        public Vector3 CubeSize = new Vector3(1, 1, 1);

        private IMarching _marching;
        private List<GameObject> _meshes;
        private bool[] _voxels;
        private Vector3Int _voxelCount;
        private Vector3 _meshOffset;
        private bool _hasChanges = false;

        public GameObject Player;

        private void Start()
        {
            Initialize();
            UpdateShape();
        }

        private void Update()
        {
            if (_hasChanges)
            {
                _hasChanges = false;
                UpdateShape();
            }

            if (OVRInput.Get(OVRInput.Button.One))
            {
                //Debug.Log(OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch));
                //SetValue(true, OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch), 1);
                Vector3 loc = Player.transform.Find("TrackingSpace").transform.TransformPoint(OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch));
                Debug.Log(loc);
                SetValue(true, loc, 0.05f);
            }
        }

        public void SetValue(bool value, Vector3 position, float radius)
        {
            for (int x = 0; x < _voxelCount.x; x++)
                for (int y = 0; y < _voxelCount.y; y++)
                    for (int z = 0; z < _voxelCount.z; z++)
                    {
                        if (Vector3.Distance(position, GetPointPosition(x, y, z)) <= radius)
                            SetValueOfIndex(value, x, y, z);
                    }
        }


        private Vector3 GetPointPosition(int x, int y, int z)
        {
            return transform.position + _meshOffset + new Vector3(x * CubeSize.x, y * CubeSize.y, z * CubeSize.z);
        }
        public void SetValueOfIndex(bool value, int x, int y, int z)
        {
            if (CheckEdgees(x, y, z) || _voxels[GetIndex(x, y, z)] == value) return;

            _voxels[GetIndex(x, y, z)] = value;
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
            _meshOffset = new Vector3(-Size.x / 2, -Size.y / 2, -Size.z / 2);
            _voxelCount = new Vector3Int(Convert.ToInt32(Size.x / CubeSize.x) + 1, Convert.ToInt32(Size.y / CubeSize.y) + 1, Convert.ToInt32(Size.z / CubeSize.z) + 1);
            _voxels = new bool[_voxelCount.x * _voxelCount.y * _voxelCount.z];
            _meshes = new List<GameObject>();

            for (int x = 0; x < _voxelCount.x; x++)
                for (int y = 0; y < _voxelCount.y; y++)
                    for (int z = 0; z < _voxelCount.z; z++)
                        _voxels[GetIndex(x, y, z)] = CheckEdgees(x, y, z);

        }

        private void UpdateShape()
        {
            foreach (var meshObj in _meshes)
                Destroy(meshObj);

            List<Vector3> verts = new List<Vector3>();
            List<int> indices = new List<int>();

            _marching.Generate(_voxels, _voxelCount, CubeSize, verts, indices);

            //A mesh in unity can only be made up of 65000 verts.
            //Need to split the verts between multiple meshes.

            int maxVertsPerMesh = 30000; //must be divisible by 3, ie 3 verts == 1 triangle
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
                go.transform.position = transform.position;
                go.AddComponent<MeshFilter>();
                go.AddComponent<MeshRenderer>();
                go.GetComponent<Renderer>().material = Material;
                go.GetComponent<MeshFilter>().mesh = mesh;
                //go.transform.localPosition = new Vector3(-Size.x / 2, -Size.y / 2, -Size.z / 2);

                _meshes.Add(go);
            }
        }
    }
}
