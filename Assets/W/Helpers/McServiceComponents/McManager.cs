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

    public void Save(string name, EditableTerrain terrain, Guid sceneGuid)
    {
        var path = GetTerrainPath(name, sceneGuid);
        _loader.SaveObj(path, terrain.GetData());
    }
    public void Save(string name, EditableModel model, Guid sceneGuid)
    {
        var path = GetModelPath(name, sceneGuid);
        _loader.SaveObj(path, model.GetData());
    }
    public void Load(string name, EditableTerrain model, Guid sceneGuid)
    {
        var path = GetModelPath(name, sceneGuid);
        var data = _loader.LoadObj(path);
        model.SetData(data);
    }
    public void Load(string name, EditableModel model, Guid sceneGuid)
    {
        var path = GetModelPath(name, sceneGuid);
        var data = _loader.LoadObj(path);
        model.SetData(data);
    }
    public GameObject LoadTerrainMeshes(string name, Guid sceneGuid)
    {
        var path = GetTerrainPath(name, sceneGuid);
        var data = _loader.LoadObj(path);
        var meshes = _terrainGenerator.GetMeshes(data);
        return meshes;
    }
    public GameObject LoadModelMeshes(string name, Guid sceneGuid)
    {
        var path = GetModelPath(name, sceneGuid);
        var data = _loader.LoadObj(path);
        var meshes = _modelGenerator.GetMeshes(data);
        return meshes;
    }

    private string GetModelPath(string name,  Guid sceneGuid)
    {
        var path = Path.Combine(sceneGuid.ToString(), Path.Combine("models", name));
        return path;
    }
    private string GetTerrainPath(string name, Guid sceneGuid)
    {
        var path = Path.Combine(sceneGuid.ToString(), name);
        return path;
    }

}
