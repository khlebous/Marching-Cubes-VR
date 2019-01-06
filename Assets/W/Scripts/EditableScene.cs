using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class EditableScene : MonoBehaviour
{
    public Guid Guid { get; set; }

    public McGameObjData Terrain { get; set; }
    public List<McObject> ModelsOnTerrain { get; set; }
    public Dictionary<Guid, McGameObjData> Models { get; set; }

    public GameObject InstantiateModel(Guid guid)
    {
        var obj = GameObject.Instantiate(Models[guid].GameObject);
        ModelsOnTerrain.Add(new McObject(guid, obj));
        obj.SetActive(true);

        return obj;
    }

    public void SetOrUpdateTerrain(McGameObjData data)
    {
        if (Terrain != null)
        {
            var newTransform = data.GameObject.transform;
            var currentTransform = Terrain.GameObject.transform;

            newTransform.parent = currentTransform.parent;
            newTransform.position = currentTransform.position;
            newTransform.rotation = currentTransform.rotation;
            newTransform.localScale = currentTransform.localScale;

            GameObject.Destroy(Terrain.GameObject);
        }

        Terrain = data;
    }

    public McSceneData GetData()
    {
        var data = new McSceneData();
        data.Guid = Guid;
        data.TerrainGuid = Terrain.Data.Guid;
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
