using Assets.MarchingCubesGPU.Scripts;
using MarchingCubesGPUProject;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class PathHelper
{
    public const string ExtensionBin = ".bin";
	public const string ExtensionPng = ".png";

	public static void EnsureDirForFileExists(string path)
    {
        var fileInfo = new FileInfo(path);
        fileInfo.Directory.Create();
    }

    public static string GetModelBinPath(Guid modelGuid, Guid sceneGuid)
    {
        return Path.Combine(GetModelDirPath(sceneGuid), modelGuid.ToString() + ExtensionBin);
    }
    public static string GetTerrainBinPath(Guid terrainGuid, Guid sceneGuid)
    {
        return Path.Combine(GetSceneDirPath(sceneGuid), terrainGuid.ToString() + ExtensionBin);
    }
    public static string GetSceneBinPath(Guid sceneGuid)
    {
        return Path.Combine(GetSceneDirPath(sceneGuid), sceneGuid.ToString() + ExtensionBin);
    }

	public static string GetNoImagePath()
	{
		return Path.Combine(Directory.GetCurrentDirectory(), "Assets/Resources/0.png");
	}
	public static string GetNewScenePath()
	{
		return Path.Combine(Directory.GetCurrentDirectory(), "Assets/Resources/NewScene.png");
	}
	public static string GetEmptyModelsListPath()
	{
		return Path.Combine(Directory.GetCurrentDirectory(), "Assets/Resources/EmptyModelsList.png");
	}
	public static string GetModelPngPath(Guid modelGuid, Guid sceneGuid)
	{
		return Path.Combine(GetModelDirPath(sceneGuid), modelGuid.ToString() + ExtensionPng);
	}
	public static string GetModelTmpPngPath(Guid sceneGuid)
	{
		return Path.Combine(GetModelDirPath(sceneGuid), "tmp" + ExtensionPng);
	}
	public static string GetScenePngPath(Guid sceneGuid)
	{
		return Path.Combine(GetSceneDirPath(sceneGuid), sceneGuid.ToString() + ExtensionPng);
	}
	public static string GetSceneTmpPngPath(Guid sceneGuid)
	{
		return Path.Combine(GetSceneDirPath(sceneGuid), "tmp" + ExtensionPng);
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
