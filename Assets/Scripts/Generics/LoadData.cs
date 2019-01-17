using System;

public struct LoadData
{
	public McData data;
	public Guid sceneGuid;

	public LoadData(Guid sceneGuid, McData data)
	{
		this.sceneGuid = sceneGuid;
		this.data = data;
	}
}