using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.MarchingCubesGPU.Scripts
{
    public enum BrushMode
    {
        Color = 3,
        Remove = 2,
        Create = 1,
        Inactive = 0
    }

    public enum BrushShape
    {
        Sphere = 1,
        Cuboid = 2
    }

    public class Brush : MonoBehaviour
    {

        public Color color;

        public BrushMode mode = BrushMode.Create;
        public BrushShape shape = BrushShape.Sphere;

        void Start()
        {
        }

        void Update()
        {
        }


        public Matrix4x4 GetToBrushMatrix()
        {
            var brushPosition = Matrix4x4.Translate(-this.transform.position);
            var brushRotation = Matrix4x4.Rotate(Quaternion.Inverse(this.transform.rotation));
            var scale = Matrix4x4.Scale(this.transform.lossyScale).inverse;

            var result = scale * brushRotation * brushPosition;
            return result;
        }
        public Matrix4x4 GetFromBrushMatrix()
        {
            var scale = Matrix4x4.Scale(this.transform.lossyScale);
            var brushRotation = Matrix4x4.Rotate(this.transform.rotation);
            var brushPosition = Matrix4x4.Translate(this.transform.position);

            var result = brushPosition * brushRotation * scale;
            return result;
        }
    }
}
