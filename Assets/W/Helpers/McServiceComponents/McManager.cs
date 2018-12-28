using System;
using System.Collections.Generic;
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
}
