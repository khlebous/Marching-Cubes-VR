using Assets.MarchingCubesGPU.Scripts;
using MarchingCubesGPUProject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UniRx;
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

    public void Start()
    {
        Loader = new McLoader();
        TerrainGenerator = new McTerrainGenerator(TerrainShaders, material);
        ModelGenerator = new McModelGenerator(ModelShaders, material);
        LoadModel(new Guid("35d4e59c-510c-4c30-94da-010e26df1896"), new Guid("bf8e21d5-a4ca-4425-9ef1-265d8d885ebc"));
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

    public EditableModel LoadModel(McData data)
    {
        var a = data.Values.Where(x => x != 0).Take(100).ToList();
        var modelObj = new GameObject();
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
        var terrain = new EditableTerrain();
        terrain.Shaders = TerrainShaders;
        terrain.SetData(data);

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
        var scene = new EditableScene();
        scene.Guid = sceneGuid;
        scene.Models = LoadModelList(sceneGuid);

        var sceneData = Loader.LoadScene(sceneGuid.ToString());
        LoadModelsOnScene(scene, sceneData.Models);

        scene.Terrain = LoadTerrainMeshes(sceneData.TerrainGuid, sceneGuid);
        scene.Terrain.GameObject.transform.parent = scene.transform;

        return scene;
    }

    public McGameObjData CreateModel()
    {
        var god = new McGameObjData();
        god.Data = ModelGenerator.GetNewEmptyData();
        god.GameObject = ModelGenerator.GenerateMeshes(god.Data);
        god.GameObject.name = McConsts.ModelPrefix + god.Data.Guid.ToString();

        return god;
    }
    public McGameObjData CreateTerrain()
    {
        var god = new McGameObjData();
        god.Data = TerrainGenerator.GetNewEmptyData();
        god.GameObject = TerrainGenerator.GenerateMeshes(god.Data);
        god.GameObject.name = McConsts.TerrainPrefix + god.Data.Guid.ToString();

        return god;
    }
    public EditableScene CreateScene()
    {
        var scene = new EditableScene();
        scene.Guid = new Guid();

        scene.Models = new Dictionary<Guid, McGameObjData>();
        scene.ModelsOnTerrain = new List<McObject>();

        scene.Terrain = CreateTerrain();
        scene.Terrain.GameObject.transform.parent = scene.transform;

        return scene;
    }

    private McGameObjData LoadTerrainMeshes(Guid terrainGuid, Guid sceneGuid)
    {
        var god = new McGameObjData();
        var path = GetTerrainPath(terrainGuid, sceneGuid);
        god.Data = Loader.LoadObj(path);
        god.GameObject = TerrainGenerator.GenerateMeshes(god.Data);
        god.GameObject.name = McConsts.TerrainPrefix + terrainGuid.ToString();

        return god;
    }
    private Dictionary<Guid, McGameObjData> LoadModelList(Guid sceneGuid)
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
            modelObj.transform.position = modelSceneData.Position;
            modelObj.transform.rotation = Quaternion.Euler(modelSceneData.Rotation);
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
