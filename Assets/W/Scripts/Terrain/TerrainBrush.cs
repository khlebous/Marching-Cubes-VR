using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.MarchingCubesGPU.Scripts
{
    public enum TerrainBrushMode
    {
        Color = 3,
        Change = 2,
        Inactive = 0
    }

    public enum TerrainBrushShape
    {
        Wheel = 1,
        Rectangle = 2
    }

    public class TerrainBrush : MonoBehaviour
    {
        //public float range;

        //public float width;//x
        //public float length; //z 

        public Color color;

        public TerrainBrushMode mode = TerrainBrushMode.Change;
        public TerrainBrushShape shape = TerrainBrushShape.Wheel;

        void Start()
        {
        }

        void Update()
        {
        }

        public Matrix4x4 GetToBrushMatrix(Vector3 position)
        {
            var brushPosition = Matrix4x4.Translate(-position);
            var brushRotation = Matrix4x4.Rotate(Quaternion.Inverse(this.transform.rotation));
            var scale = Matrix4x4.Scale(this.transform.lossyScale).inverse;

            var result = scale * brushRotation * brushPosition;
            return result;
        }
        public Matrix4x4 GetFromBrushMatrix(Vector3 position)
        {
            var scale = Matrix4x4.Scale(this.transform.lossyScale);
            var brushRotation = Matrix4x4.Rotate(this.transform.rotation);
            var brushPosition = Matrix4x4.Translate(position);

            var result = brushPosition * brushRotation * scale;
            return result;
        }
    }   
}
