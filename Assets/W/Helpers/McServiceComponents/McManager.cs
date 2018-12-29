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

    public void Save(Guid terrainGuid, EditableTerrain terrain, Guid sceneGuid)
    {
        var path = GetTerrainPath(terrainGuid, sceneGuid);
        _loader.SaveObj(path, terrain.GetData());
    }
    public void Save(Guid modelGuid, EditableModel model, Guid sceneGuid)
    {
        var path = GetModelPath(modelGuid, sceneGuid);
        _loader.SaveObj(path, model.GetData());
    }
    public void Save(EditableScene scene)
    {
        var data = scene.GetData();
        _loader.SaveScene(scene.SceneGuid.ToString(), data);
    }

    public void Load(Guid terrainGuid, EditableTerrain terrain, Guid sceneGuid)
    {
        var path = GetTerrainPath(terrainGuid, sceneGuid);
        var data = _loader.LoadObj(path);
        terrain.SetData(data);
    }
    public void Load(Guid modelGuid, EditableModel model, Guid sceneGuid)
    {
        var path = GetModelPath(modelGuid, sceneGuid);
        var data = _loader.LoadObj(path);
        model.SetData(data);
    }
    public void Load(EditableScene scene)
    {
        var data = _loader.LoadScene(scene.SceneGuid.ToString());
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
    }

    public GameObject LoadTerrainMeshes(Guid terrainGuid, Guid sceneGuid)
    {
        var path = GetTerrainPath(terrainGuid, sceneGuid);
        var data = _loader.LoadObj(path);
        var meshes = _terrainGenerator.GetMeshes(data);
        return meshes;
    }
    public GameObject LoadModelMeshes(Guid modelGuid, Guid sceneGuid)
    {
        var path = GetModelPath(modelGuid, sceneGuid);
        var data = _loader.LoadObj(path);
        var meshes = _modelGenerator.GetMeshes(data);
        return meshes;
    }

    private string GetModelPath(Guid modelGuid, Guid sceneGuid)
    {
        var path = Path.Combine(sceneGuid.ToString(), Path.Combine("models", name.ToString()));
        return path;
    }
    private string GetTerrainPath(Guid terrainGuid, Guid sceneGuid)
    {
        var path = Path.Combine(sceneGuid.ToString(), name.ToString());
        return path;
    }

}
