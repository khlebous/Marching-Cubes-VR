using System;

public struct TerrainLoadData
{
	public McData data;
	public Guid sceneGuid;

	public TerrainLoadData(Guid sceneGuid, McData data)
	{
		this.sceneGuid = sceneGuid;
		this.data = data;
	}
}

public struct ModelLoaddata
{

}
