using Assets.MarchingCubesGPU.Scripts;
using MarchingCubesGPUProject;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class McManager : MonoBehaviour
{
    public ModelSchaders ModelShaders;
    public TerrainShaders TerrainShaders;

    public TerrainBrush TerrainBrush;
    public ModelBrush ModelBrush;

    public Material material;

    public McLoader Loader { get; set; }
    public McTerrainGenerator TerrainGenerator { get; set; }
    public McModelGenerator ModelGenerator { get; set; }

    public void Awake()
    {
        Loader = new McLoader();
        TerrainGenerator = new McTerrainGenerator(TerrainShaders, material);
        ModelGenerator = new McModelGenerator(ModelShaders, material);
    }

    public void OnDestroy()
    {
        TerrainGenerator.ReleaseBuffers();
        ModelGenerator.ReleaseBuffers();
    }

    public void Save(EditableTerrain terrain, Guid sceneGuid)
    {
        var data = terrain.GetData();
        var path = GetTerrainPath(data.Guid, sceneGuid);
        Loader.SaveObj(path, data);
    }
    public void Save(EditableModel model, Guid sceneGuid)
    {
        var data = model.GetData();
        var path = GetModelPath(data.Guid, sceneGuid);
        Loader.SaveObj(path, data);
    }
    public void Save(EditableScene scene)
    {
        var data = scene.GetData();
        Loader.SaveScene(GetScenePath(scene.Guid), data);

        var terrinPath = GetTerrainPath(scene.Terrain.Data.Guid, scene.Guid);
        if (!Loader.ObjectExists(terrinPath))
            Loader.SaveObj(terrinPath, scene.Terrain.Data);
    }

    public void DeleteModel(Guid modelGuid, Guid SceneGuid)
    {
        Loader.DeleteFile(GetModelPath(modelGuid, SceneGuid));
    }

    public EditableModel LoadModel(McData data)
    {
        var modelObj = new GameObject();
        modelObj.name = McConsts.EditableModelPrefix + data.Guid.ToString();
        var model = modelObj.AddComponent<EditableModel>();
        model.Shaders = ModelShaders;
        model.material = material;
        model.brush = ModelBrush;
        model.SetData(data);

        return model;
    }
    public EditableModel LoadModel(Guid modelGuid, Guid sceneGuid)
    {
        var path = GetModelPath(modelGuid, sceneGuid);
        var data = Loader.LoadObj(path);

        return LoadModel(data);
    }
    public EditableTerrain LoadTerrain(McData data)
    {
        var terainObj = new GameObject();
        terainObj.name = McConsts.EditableTerrainPrefix + data.Guid.ToString();
        var terrain = terainObj.AddComponent<EditableTerrain>();
        terrain.Shaders = TerrainShaders;
        terrain.material = material;
        terrain.brush = TerrainBrush;
        terrain.SetData(data);

        TerrainBrush.terrain = terrain;

        return terrain;
    }
    public EditableTerrain LoadTerrain(Guid terrainGuid, Guid sceneGuid)
    {
        var path = GetTerrainPath(terrainGuid, sceneGuid);
        var data = Loader.LoadObj(path);

        return LoadTerrain(data);
    }
    public EditableScene LoadScene(Guid sceneGuid)
    {
        if (sceneGuid == Guid.Empty)
            return CreateScene();

        var sceneObj = new GameObject();
        var scene = sceneObj.AddComponent<EditableScene>();
        scene.Guid = sceneGuid;
        sceneObj.name = McConsts.EditableScenePrefix + scene.Guid.ToString();
        scene.Models = LoadModelList(sceneGuid, scene.transform);

        var sceneData = Loader.LoadScene(GetScenePath(sceneGuid));
        scene.LoadModelsOnScene(sceneData.Models);

        scene.SetOrUpdateTerrain(LoadTerrainMeshes(sceneData.TerrainGuid, sceneGuid));

        return scene;
    }

    public McData CreateModel()
    {
        return ModelGenerator.GetDefaultData();
    }
    public EditableScene CreateScene()
    {
        var sceneObj = new GameObject();
        var scene = sceneObj.AddComponent<EditableScene>();
        scene.Guid = Guid.NewGuid();
        sceneObj.name = McConsts.EditableScenePrefix + scene.Guid.ToString();

        scene.Models = new Dictionary<Guid, McGameObjData>();
        scene.ModelsOnTerrain = new List<McObject>();

        scene.SetOrUpdateTerrain(CreateTerrain());

        return scene;
    }

    public List<Guid> GetAllSceneGuids()
    {
        return Loader.GetAllDirGuids("");
    }

    public GameObject LoadTerrainMeshes(McData data)
    {
        var gameObject = TerrainGenerator.GenerateMeshes(data);
        gameObject.name = McConsts.TerrainPrefix + data.Guid.ToString();

        return gameObject;
    }
    public GameObject LoadModelMeshes(McData data)
    {
        var gameObject = ModelGenerator.GenerateMeshes(data);
        gameObject.name = McConsts.ModelPrefix + data.Guid.ToString();
        gameObject.SetActive(false);

        return gameObject;
    }

    private McGameObjData CreateTerrain()
    {
        var god = new McGameObjData();
        god.Data = TerrainGenerator.GetDefaultData();
        god.GameObject = LoadTerrainMeshes(god.Data);

        return god;
    }
    private McGameObjData LoadTerrainMeshes(Guid terrainGuid, Guid sceneGuid)
    {
        var god = new McGameObjData();
        var path = GetTerrainPath(terrainGuid, sceneGuid);
        god.Data = Loader.LoadObj(path);
        god.GameObject = LoadTerrainMeshes(god.Data);

        return god;
    }
    private Dictionary<Guid, McGameObjData> LoadModelList(Guid sceneGuid, Transform sceneTransform)
    {
        var dirPath = GetModelDirPath(sceneGuid);
        var guids = Loader.GetAllObjGuids(dirPath);

        var models = new Dictionary<Guid, McGameObjData>();
        foreach (var guid in guids)
        {
            var god = new McGameObjData();

            god.Data = Loader.LoadObj(GetModelPath(guid, sceneGuid));
            god.GameObject = ModelGenerator.GenerateMeshes(god.Data);
            god.GameObject.name = McConsts.ModelPrefix + guid.ToString();
            god.GameObject.transform.parent = sceneTransform;
            god.GameObject.SetActive(false);

            models.Add(guid, god);
        }

        return models;
    }

    private void LoadModelsOnScene(EditableScene scene, List<McSceneModelData> Models)
    {
        scene.ModelsOnTerrain = new List<McObject>();
        foreach (var modelSceneData in Models)
        {
            var modelObj = GameObject.Instantiate(scene.Models[modelSceneData.Guid].GameObject);
            modelObj.name = McConsts.ObjectPrefix + modelSceneData.Guid.ToString();

            modelObj.transform.parent = scene.transform;
            modelObj.transform.localPosition= modelSceneData.Position;
            modelObj.transform.localRotation = Quaternion.Euler(modelSceneData.Rotation);
            modelObj.transform.localScale = modelSceneData.Scale;

            modelObj.SetActive(true);

            scene.ModelsOnTerrain.Add(new McObject(modelSceneData.Guid, modelObj));
        }
    }

    private string GetModelDirPath(Guid sceneGuid)
    {
        var path = Path.Combine(sceneGuid.ToString(), "models");
        return path;
    }
    private string GetModelPath(Guid modelGuid, Guid sceneGuid)
    {
        var path = Path.Combine(GetModelDirPath(sceneGuid), modelGuid.ToString());
        return path;
    }
    private string GetTerrainPath(Guid terrainGuid, Guid sceneGuid)
    {
        var path = Path.Combine(sceneGuid.ToString(), terrainGuid.ToString());
        return path;
    }
    private string GetScenePath(Guid sceneGuid)
    {
        var path = Path.Combine(sceneGuid.ToString(), sceneGuid.ToString());
        return path;
    }

}
