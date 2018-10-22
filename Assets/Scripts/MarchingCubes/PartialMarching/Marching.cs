using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.marching2.MarchingCubes.PartialMarching
{
    public abstract class Marching : IMarching
    {
        private bool[] Cube { get; set; }

        protected Marching()
        {
            Cube = new bool[8];
        }

        public virtual void Generate(IList<bool> voxels, IList<bool> changed, Vector3Int voxelCount, Vector3 cubeSize, List<Vector3>[] cubeVertices)
        {
            var cubeCount = voxelCount - Vector3Int.one;

            for (int x = 0; x < voxelCount.x - 1; x++)
            {
                for (int y = 0; y < voxelCount.y - 1; y++)
                {
                    for (int z = 0; z < voxelCount.z - 1; z++)
                    {
                        bool cubeChanged = false;

                        //Get the values in the 8 neighbours which make up a cube
                        for (int i = 0; i < 8; i++)
                        {
                            int ix = x + VertexOffset[i, 0];
                            int iy = y + VertexOffset[i, 1];
                            int iz = z + VertexOffset[i, 2];

                            if (changed[GetIndex(ix, iy, iz, voxelCount)])
                                cubeChanged = true;

                            Cube[i] = voxels[GetIndex(ix, iy, iz, voxelCount)];
                        }

                        if (cubeChanged)
                        {
                            var indices = cubeVertices[GetIndex(x, y, z, cubeCount)];
                            indices.Clear();
                            March(x, y, z, Cube, cubeSize, indices);

                        }
                    }
                }
            }
        }


        private int GetIndex(int x, int y, int z, Vector3Int count)
        {
            return x + y * count.x + z * count.x * count.y;
        }

        /// <summary>
        /// MarchCube performs the Marching algorithm on a single cube
        /// </summary>
        protected abstract void March(float x, float y, float z, bool[] cube, Vector3 cubeSize,
            List<Vector3> indices);

        /// <summary>
        /// VertexOffset lists the positions, relative to vertex0, 
        /// of each of the 8 vertices of a cube.
        /// vertexOffset[8][3]
        /// </summary>
        protected static readonly int[,] VertexOffset = {
            {0, 0, 0},{1, 0, 0},{1, 1, 0},{0, 1, 0},
            {0, 0, 1},{1, 0, 1},{1, 1, 1},{0, 1, 1}
        };
    }
}
