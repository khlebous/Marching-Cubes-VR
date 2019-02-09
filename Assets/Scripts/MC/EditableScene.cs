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
        obj.name = McConsts.ObjectPrefix + guid.ToString();
        obj.transform.parent = this.transform;
        obj.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        obj.transform.localPosition = Vector3.zero;
        ModelsOnTerrain.Add(new McObject(guid, obj));

        obj.AddComponent<ObjectController>();
        obj.AddComponent<MovementWithOculusTouch>();
        obj.GetComponent<MovementWithOculusTouch>().enabled = false;
        obj.AddComponent<FullRotationWithOculusTouch>();
        obj.GetComponent<FullRotationWithOculusTouch>().enabled = false;
        obj.AddComponent<ScaleWithOculusTouch>();
        obj.GetComponent<ScaleWithOculusTouch>().enabled = false;

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
        data.GameObject.transform.parent = this.transform;

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
        var objToDelete = ModelsOnTerrain.Where(x => x.ModelGuid == modelGuid).ToList();
        foreach (var obj in objToDelete)
        {
            GameObject.Destroy(obj.GameObject);
            ModelsOnTerrain.Remove(obj);
        }

        GameObject.Destroy(Models[modelGuid].GameObject);
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
            var modelObj = InstantiateModel(modelSceneData.Guid);

            modelObj.transform.position = modelSceneData.Position;
            modelObj.transform.rotation = Quaternion.Euler(modelSceneData.Rotation);
            modelObj.transform.localScale = modelSceneData.Scale;
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
