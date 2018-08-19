using System.Collections.Generic;
using UnityEngine;

namespace Assets.marching2.MarchingCubes.PartialMarching
{
    public interface IMarching
    {
        void Generate(IList<bool> voxels, IList<bool> changed, Vector3Int voxelCount, Vector3 cubeSize,
            List<Vector3>[] cubeVertices);
    }
}