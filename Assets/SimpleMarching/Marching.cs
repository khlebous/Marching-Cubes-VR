using System.Collections.Generic;
using UnityEngine;

namespace Assets.marching2.MarchingCubes.SimpleMarching
{
    public abstract class Marching : IMarching
    {
        private bool[] Cube { get; set; }

        protected Marching()
        {
            Cube = new bool[8];
        }

        public virtual void Generate(IList<bool> voxels, Vector3Int voxelCount, Vector3 cubeSize, IList<Vector3> verts, IList<int> indices)
        {

            for (int x = 0; x < voxelCount.x - 1; x++)
            {
                for (int y = 0; y < voxelCount.y - 1; y++)
                {
                    for (int z = 0; z < voxelCount.z - 1; z++)
                    {
                        //Get the values in the 8 neighbours which make up a cube
                        for (int i = 0; i < 8; i++)
                        {
                            int ix = x + VertexOffset[i, 0];
                            int iy = y + VertexOffset[i, 1];
                            int iz = z + VertexOffset[i, 2];

                            Cube[i] = voxels[ix + iy * voxelCount.x + iz * voxelCount.x * voxelCount.y];
                        }

                        March(x, y, z, Cube, cubeSize, verts, indices);
                    }
                }
            }
        }


        /// <summary>
        /// MarchCube performs the Marching algorithm on a single cube
        /// </summary>
        protected abstract void March(float x, float y, float z, bool[] cube, Vector3 cubeSize, IList<Vector3> vertList, IList<int> indexList);

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
