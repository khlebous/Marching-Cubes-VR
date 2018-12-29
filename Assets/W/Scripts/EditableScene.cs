using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EditableScene : MonoBehaviour
{
    public Guid SceneGuid { get; set; }

    public Guid TerrainGuid { get; set; }
    public McData TerrainData { get; set; }
    public GameObject Terrain { get; set; }

    public Dictionary<Guid, GameObject> PlacedModels { get; set; }

    public Dictionary<Guid, McData> ModelsData { get; set; }
    public Dictionary<Guid, GameObject> Models { get; set; }

    public void Start()
    {

    }

    private void Update()
    {

    }

    public McSceneData GetData()
    {
        var data = new McSceneData();
        data.TerrainGuid = TerrainGuid;
        data.Models = PlacedModels.Select(x => new McSceneModelData()
        {
            Guid = x.Key,
            Position = x.Value.transform.position,
            Rotation = x.Value.transform.rotation.eulerAngles,
            Scale = x.Value.transform.localScale, // local or lossy??

        }).ToList();

        return data;
    }
}
