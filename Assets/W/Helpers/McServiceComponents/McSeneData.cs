using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;

[Serializable]
public class McSceneData
{
    public Guid TerrainGuid { get; set; }
    public List<McSceneModelData> Models { get; set; }
}

[Serializable]
public class McSceneModelData
{
    public Guid Guid { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 Rotation { get; set; }
    public Vector3 Scale { get; set; }
}



