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

    private McLoader _loader;
    private McTerrainGenerator _terrainGenerator;
    private McModelGenerator _modelGenerator;

    public void Start()
    {
        _loader = new McLoader();
        _terrainGenerator = new McTerrainGenerator(terrainMarching, terrainNormals, clearBuffer, material);
        _terrainGenerator = new McTerrainGenerator(modelMarching, modelNormals, clearBuffer, material);
    }

    public void Update()
    {
    }

    public void OnDestroy()
    {
        _terrainGenerator.ReleaseBuffers();
        _modelGenerator.ReleaseBuffers();
    }


    public void Save(EditableTerrain terrain, Guid sceneGuid)
    {
        var data = terrain.GetData();
        var path = GetTerrainPath(data.Guid, sceneGuid);
        _loader.SaveObj(path, data);
    }
    public void Save(EditableModel model, Guid sceneGuid)
    {
        var data = model.GetData();
        var path = GetModelPath(data.Guid, sceneGuid);
        _loader.SaveObj(path, data);
    }
    public void Save(EditableScene scene)
    {
        var data = scene.GetData();
        _loader.SaveScene(scene.SceneGuid.ToString(), data);
    }

    public EditableTerrain LoadTerrain(Guid terrainGuid, Guid sceneGuid)
    {
        var path = GetTerrainPath(terrainGuid, sceneGuid);
        var data = _loader.LoadObj(path);

        var terrain = new EditableTerrain();
        terrain.SetData(data);

        return terrain;
    }
    public EditableModel LoadModel(Guid modelGuid, Guid sceneGuid)
    {
        var path = GetModelPath(modelGuid, sceneGuid);
        var data = _loader.LoadObj(path);

        var model = new EditableModel();
        model.SetData(data);

        return model;
    }
    public EditableScene LoadScene(Guid sceneGuid)
    {
        var scene = new EditableScene();
        scene.SceneGuid = sceneGuid;
        var data = _loader.LoadScene(sceneGuid.ToString());
        scene.TerrainGuid = data.TerrainGuid;

        var terrain = LoadTerrainMeshes(scene.TerrainGuid, scene.SceneGuid);
        terrain.name = "Terrain_" + scene.TerrainGuid.ToString();
        terrain.transform.parent = scene.transform;

        foreach (var modelData in data.Models)
        {
            var model = LoadModelMeshes(modelData.Guid, scene.SceneGuid);
            terrain.name = "Model_" + modelData.Guid.ToString();

            terrain.transform.parent = scene.transform;
            terrain.transform.position = modelData.Position;
            terrain.transform.rotation = Quaternion.Euler(modelData.Rotation);
            terrain.transform.localScale = modelData.Scale;
        }

        return scene;
    }

    public McData CreateModel()
    {
        var data = _modelGenerator.GetNewEmptyData();
        return data;
    }
    public McData CreateTerrain()
    {
        var data = _terrainGenerator.GetNewEmptyData();
        return data;
    }
    public EditableScene CreateScene()
    {
        var scene = new EditableScene();
        scene.SceneGuid = new Guid();

        var terrainData = CreateTerrain();
        scene.Terrain = _terrainGenerator.GenerateMeshes(terrainData);
        scene.Terrain.name = "Terrain_" + scene.TerrainGuid.ToString();
        scene.Terrain.transform.parent = scene.transform;

        return scene;
    }

    public List<McData> LoadModelList(Guid sceneGuid)
    {
        var dirPath = GetModelDirPath(sceneGuid);
        var guids = _loader.GetAllObjGuids(dirPath);

        var models = new List<McData>();

        foreach (var guid in guids)
        {
            var model = _loader.LoadObj(GetModelPath(guid, sceneGuid));
            models.Add(model);
        }

        return models;
    }

    public GameObject LoadTerrainMeshes(Guid terrainGuid, Guid sceneGuid)
    {
        var path = GetTerrainPath(terrainGuid, sceneGuid);
        var data = _loader.LoadObj(path);
        var meshes = _terrainGenerator.GenerateMeshes(data);
        return meshes;
    }
    public GameObject LoadModelMeshes(Guid modelGuid, Guid sceneGuid)
    {
        var path = GetModelPath(modelGuid, sceneGuid);
        var data = _loader.LoadObj(path);
        var meshes = _modelGenerator.GenerateMeshes(data);
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
