using MarchingCubesGPUProject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class McManager : MonoBehaviour
{
    public ComputeShader terrainMarching;
    public ComputeShader terrainNormals;
    public ComputeShader modelMarching;
    public ComputeShader modelNormals;

    public ComputeShader clearBuffer;
    public Material material;

    public McLoader Loader { get; set; }
    public McTerrainGenerator TerrainGenerator { get; set; }
    public McModelGenerator ModelGenerator { get; set; }

    public void Start()
    {
        Loader = new McLoader();
        TerrainGenerator = new McTerrainGenerator(terrainMarching, terrainNormals, clearBuffer, material);
        TerrainGenerator = new McTerrainGenerator(modelMarching, modelNormals, clearBuffer, material);
    }

    public void Update()
    {
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
        Loader.SaveScene(scene.Guid.ToString(), data);
    }

    public EditableTerrain LoadTerrain(Guid terrainGuid, Guid sceneGuid)
    {
        var path = GetTerrainPath(terrainGuid, sceneGuid);
        var data = Loader.LoadObj(path);

        var terrain = new EditableTerrain();
        terrain.SetData(data);

        return terrain;
    }
    public EditableModel LoadModel(Guid modelGuid, Guid sceneGuid)
    {
        var path = GetModelPath(modelGuid, sceneGuid);
        var data = Loader.LoadObj(path);

        var model = new EditableModel();
        model.SetData(data);

        return model;
    }
    public EditableScene LoadScene(Guid sceneGuid)
    {
        var scene = new EditableScene();
        scene.Guid = sceneGuid;
        LoadModelList(scene);
        
        var data = Loader.LoadScene(sceneGuid.ToString());
        LoadTerrainOnScene(scene, data.TerrainGuid);
        LoadModelsOnScene(scene, data.Models);

        return scene;
    }

    private void LoadModelList(EditableScene scene)
    {
        scene.Models = new Dictionary<Guid, GameObject>();
        scene.ModelsData = new Dictionary<Guid, McData>();

        foreach (var modelData in LoadModelDataList(scene.Guid))
        {
            scene.ModelsData.Add(modelData.Guid, modelData);
            var modelObj = ModelGenerator.GenerateMeshes(modelData);
            modelObj.name = McConsts.ModelPrefix + modelData.Guid;
            modelObj.SetActive(false);
            scene.Models.Add(modelData.Guid, modelObj);
        }
    }
    private List<McData> LoadModelDataList(Guid sceneGuid)
    {
        var dirPath = GetModelDirPath(sceneGuid);
        var guids = Loader.GetAllObjGuids(dirPath);

        var models = new List<McData>();

        foreach (var guid in guids)
        {
            var model = Loader.LoadObj(GetModelPath(guid, sceneGuid));
            models.Add(model);
        }

        return models;
    }
    private void LoadTerrainOnScene(EditableScene scene, Guid terrainGuid)
    {
        scene.TerrainGuid = terrainGuid;
        var terrain = LoadTerrainMeshes(scene.TerrainGuid, scene.Guid);
        terrain.name = McConsts.TerrainPrefix + scene.TerrainGuid.ToString();
        terrain.transform.parent = scene.transform;
    }
    private void LoadModelsOnScene(EditableScene scene, List<McSceneModelData> Models)
    {
        scene.ModelsOnTerrain = new Dictionary<Guid, GameObject>();
        foreach (var modelSceneData in Models)
        {
            var modelObj = GameObject.Instantiate(scene.Models[modelSceneData.Guid]);
            modelObj.name = McConsts.ObjectPrefix + modelSceneData.Guid.ToString();

            modelObj.transform.parent = scene.transform;
            modelObj.transform.position = modelSceneData.Position;
            modelObj.transform.rotation = Quaternion.Euler(modelSceneData.Rotation);
            modelObj.transform.localScale = modelSceneData.Scale;

            modelObj.SetActive(true);

            scene.ModelsOnTerrain.Add(modelSceneData.Guid, modelObj);
        }
    }

    public McData CreateModel()
    {
        var data = ModelGenerator.GetNewEmptyData();
        return data;
    }
    public McData CreateTerrain()
    {
        var data = TerrainGenerator.GetNewEmptyData();
        return data;
    }
    public EditableScene CreateScene()
    {
        var scene = new EditableScene();
        scene.Guid = new Guid();

        scene.Models = new Dictionary<Guid, GameObject>();
        scene.ModelsData = new Dictionary<Guid, McData>();
        scene.ModelsOnTerrain = new Dictionary<Guid, GameObject>();

        var terrainData = CreateTerrain();
        scene.Terrain = TerrainGenerator.GenerateMeshes(terrainData);
        scene.Terrain.name = McConsts.TerrainPrefix + scene.TerrainGuid.ToString();
        scene.Terrain.transform.parent = scene.transform;

        return scene;
    }

    public GameObject LoadTerrainMeshes(Guid terrainGuid, Guid sceneGuid)
    {
        var path = GetTerrainPath(terrainGuid, sceneGuid);
        var data = Loader.LoadObj(path);
        var meshes = TerrainGenerator.GenerateMeshes(data);

        return meshes;
    }
    public GameObject LoadModelMeshes(Guid modelGuid, Guid sceneGuid)
    {
        var path = GetModelPath(modelGuid, sceneGuid);
        var data = Loader.LoadObj(path);
        var meshes = ModelGenerator.GenerateMeshes(data);

        return meshes;
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

}
