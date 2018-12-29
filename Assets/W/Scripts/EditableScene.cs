using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class EditableScene : MonoBehaviour
{
    public Guid Guid { get; set; }

    public Guid TerrainGuid { get; set; }
    public McData TerrainData { get; set; }
    public GameObject Terrain { get; set; }

    public List<McObject> ModelsOnTerrain { get; set; }

    public Dictionary<Guid, McData> ModelsData { get; set; }
    public Dictionary<Guid, GameObject> Models { get; set; }

    public GameObject InstantiateModel(Guid guid)
    {
        var obj = GameObject.Instantiate(Models[guid]);
        ModelsOnTerrain.Add(new McObject(guid, obj));
        obj.SetActive(true);

        return obj;
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
            Guid = x.Guid,
            Position = x.GameObject.transform.position,
            Rotation = x.GameObject.transform.rotation.eulerAngles,
            Scale = x.GameObject.transform.localScale,
        }).ToList();

        return data;
    }
}
