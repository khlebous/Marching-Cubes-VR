using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EditableScene : MonoBehaviour
{
    public Guid SceneGuid;

    public GameObject Terrain;
    public Guid TerrainGuid;

    public List<KeyValuePair<Guid, GameObject>> Models;

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
        data.Models = Models.Select(x => new McSceneModelData()
        {
            Guid = x.Key,
            Position = x.Value.transform.position,
            Rotation = x.Value.transform.rotation.eulerAngles,
            Scale = x.Value.transform.localScale, // local or lossy??

        }).ToList();

        return data;
    }
}
