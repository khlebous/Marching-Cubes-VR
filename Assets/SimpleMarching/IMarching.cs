using System.Collections.Generic;
using UnityEngine;

namespace Assets.marching2.MarchingCubes.SimpleMarching
{
    public interface IMarching
    {
        void Generate(IList<bool> voxels, Vector3Int voxelCount, Vector3 cubeSize, IList<Vector3> verts,
            IList<int> indices);
    }
}