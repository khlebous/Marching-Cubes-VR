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
		obj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		obj.transform.localPosition = Vector3.zero;
		ModelsOnTerrain.Add(new McObject(guid, obj));

		obj.AddComponent<ObjectController>();
		obj.AddComponent<MovementWithOculusTouch>();
		obj.GetComponent<MovementWithOculusTouch>().enabled = false;
		obj.AddComponent<RotationWithOculusTouch>();
		obj.GetComponent<RotationWithOculusTouch>().enabled = false;

		obj.tag = Constants.OBJECT_TAG;
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
		var toUpdate = ModelsOnTerrain.Where(x => x.ModelGuid == modelGuid).ToList();
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

    public void LoadModelsOnScene(List<McSceneModelData> ModelsToLoad)
    {
        ModelsOnTerrain = new List<McObject>();
        foreach (var modelSceneData in ModelsToLoad)
        {
            var modelObj = GameObject.Instantiate(Models[modelSceneData.Guid].GameObject);
            modelObj.name = McConsts.ObjectPrefix + modelSceneData.Guid.ToString();

            modelObj.transform.parent = transform;
            modelObj.transform.position = modelSceneData.Position;
            modelObj.transform.rotation = Quaternion.Euler(modelSceneData.Rotation);
            modelObj.transform.localScale = modelSceneData.Scale;

			modelObj.AddComponent<ObjectController>();
			modelObj.AddComponent<MovementWithOculusTouch>();
			modelObj.GetComponent<MovementWithOculusTouch>().enabled = false;
			modelObj.AddComponent<RotationWithOculusTouch>();
			modelObj.GetComponent<RotationWithOculusTouch>().enabled = false;

			modelObj.tag = Constants.OBJECT_TAG;

			modelObj.SetActive(true);

            ModelsOnTerrain.Add(new McObject(modelSceneData.Guid, modelObj));
        }
    }

	public McSceneData GetData()
	{
		var data = new McSceneData();
		data.Guid = Guid;
		data.TerrainGuid = Terrain.Data.Guid;
		data.Models = ModelsOnTerrain.Select(x => new McSceneModelData()
		{
			Guid = x.ModelGuid,
			Position = x.GameObject.transform.localPosition,
			Rotation = x.GameObject.transform.localRotation.eulerAngles,
			Scale = x.GameObject.transform.localScale,
		}).ToList();

		return data;
	}

    public void Destroy()
    {
        GameObject.Destroy(gameObject);
    }
}
