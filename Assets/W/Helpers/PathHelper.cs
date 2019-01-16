using Assets.MarchingCubesGPU.Scripts;
using MarchingCubesGPUProject;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class PathHelper
{
    public const string _extension = ".bin";

    public static void EnsureDirForFileExists(string path)
    {
        var fileInfo = new FileInfo(path);
        fileInfo.Directory.Create();
    }

    public static string GetModelPath(Guid modelGuid, Guid sceneGuid)
    {
        var path = Path.Combine(GetModelDirPath(sceneGuid), modelGuid.ToString() + _extension);
        return path;
    }
    public static string GetTerrainPath(Guid terrainGuid, Guid sceneGuid)
    {
        return Path.Combine(GetSceneDirPath(sceneGuid), terrainGuid.ToString() + _extension);
    }
    public static string GetScenePath(Guid sceneGuid)
    {
        return Path.Combine(GetSceneDirPath(sceneGuid), sceneGuid.ToString() + _extension);
    }

    public static string GetModelDirPath(Guid sceneGuid)
    {

        return Path.Combine(GetSceneDirPath(sceneGuid), "models");
    }
    public static string GetSceneDirPath(Guid sceneGuid)
    {
        return Path.Combine(GetRootPath(), sceneGuid.ToString());
    }

    public static string GetRootPath()
    {
        return Path.Combine(Directory.GetCurrentDirectory(), "saves");
    }
}
