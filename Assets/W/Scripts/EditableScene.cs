using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EditableScene : MonoBehaviour
{
    public Guid Guid { get; set; }

    public Guid TerrainGuid { get; set; }
    public McData TerrainData { get; set; }
    public GameObject Terrain { get; set; }

    public Dictionary<Guid, GameObject> ModelsOnTerrain { get; set; }

    public Dictionary<Guid, McData> ModelsData { get; set; }
    public Dictionary<Guid, GameObject> Models { get; set; }

    public void InstantiateModel(Guid guid)
    {

    }

    public void Start()
    {

    }

    private void Update()
    {

    }

    public McSceneData GetData()
    {
        var data = new McSceneData();
        data.Guid = Guid;
        data.TerrainGuid = TerrainGuid;
        data.Models = ModelsOnTerrain.Select(x => new McSceneModelData()
        {
            Guid = x.Key,
            Position = x.Value.transform.position,
            Rotation = x.Value.transform.rotation.eulerAngles,
            Scale = x.Value.transform.localScale,
        }).ToList();

        return data;
    }
}
