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
        obj.transform.parent = this.transform;
        ModelsOnTerrain.Add(new McObject(guid, obj));
        obj.SetActive(true);

        return obj;
    }
    public void SetOrUpdateTerrain(McGameObjData data)
    {
        var newTransform = data.GameObject.transform;
        newTransform.parent = this.transform;

        if (Terrain != null)
        {
            var currTransform = Terrain.GameObject.transform;

            newTransform.position = currTransform.position;
            newTransform.rotation = currTransform.rotation;
            newTransform.localScale = currTransform.localScale;

            GameObject.Destroy(Terrain.GameObject);
        }

        Terrain = data;
    }
    public void SetOrUpdateModel(McGameObjData data)
    {
        var guid = data.Data.Guid;

        if (!Models.ContainsKey(guid))
        {
            Models[guid] = data;
        }
        else
        {
            GameObject.Destroy(Models[guid].GameObject);

            Models[guid] = data;
            UpdateModelsOnTerrain(guid);
        }
    }
    public void UpdateModelsOnTerrain(Guid modelGuid)
    {
        var toUpdate = ModelsOnTerrain.Where(x => x.ModelGuid == modelGuid);
        foreach (var model in toUpdate)
        {
            var updatedObj = InstantiateModel(modelGuid);
            updatedObj.transform.position = model.GameObject.transform.position;
            updatedObj.transform.rotation = model.GameObject.transform.rotation;
            updatedObj.transform.localScale = model.GameObject.transform.localScale;

            GameObject.Destroy(model.GameObject);
            model.GameObject = updatedObj;
        }
    }
	public void DeleteModel(Guid modelGuid)
	{
		var toDelete = ModelsOnTerrain.Where(x => x.ModelGuid == modelGuid);
		foreach (var model in toDelete)
		{
			GameObject.Destroy(model.GameObject);
		}

		Models.Remove(modelGuid);
	}
    public void DeleteModelFromTerrain(GameObject gameObject)
    {
        var toDelete = ModelsOnTerrain.First(x => x.GameObject == gameObject);
        GameObject.Destroy(toDelete.GameObject);
        ModelsOnTerrain.Remove(toDelete);
    }

	public McSceneData GetData()
    {
        var data = new McSceneData();
        data.Guid = Guid;
        data.TerrainGuid = Terrain.Data.Guid;
        data.Models = ModelsOnTerrain.Select(x => new McSceneModelData()
        {
            Guid = x.ModelGuid,
            Position = x.GameObject.transform.position,
            Rotation = x.GameObject.transform.rotation.eulerAngles,
            Scale = x.GameObject.transform.localScale,
        }).ToList();

        return data;
    }

	public void Destroy()
	{
		GameObject.Destroy(gameObject);
	}
}
