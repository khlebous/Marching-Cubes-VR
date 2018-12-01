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
        public float range;

        public float width;//x
        public float thickness;//y
        public float length; //z 

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
            var brushSize = Matrix4x4.Translate(new Vector3(width, thickness, length) / 2);

            var result = brushSize * brushRotation * brushPosition;
            return result;
        }
        public Matrix4x4 GetFromBrushMatrix()
        {
            var brushSize = Matrix4x4.Translate(new Vector3(-width, -thickness, -length) / 2);
            var brushRotation = Matrix4x4.Rotate(this.transform.rotation);
            var brushPosition = Matrix4x4.Translate(this.transform.position);

            var result = brushPosition * brushRotation * brushSize;
            return result;
        }
    }
}
