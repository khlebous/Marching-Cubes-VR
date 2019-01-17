using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class McLoader
{
    public void SaveScene(string path, McSceneData data)
    {
        Save(path, data);
    }
    public void SaveObj(string path, McData data)
    {
        Save(path, data);
    }
    private void Save(string path, object data)
    {
        PathHelper.EnsureDirForFileExists(path);

        BinaryFormatter bin = GetBinFormatter();
        using (Stream stream = File.Open(path, FileMode.Create))
        {
            bin.Serialize(stream, data);
        }
    }

    public McSceneData LoadScene(string path)
    {
        return Load<McSceneData>(path);
    }
    public McData LoadObj(string path)
    {
        return Load<McData>(path);
    }
    private T Load<T>(string path)
    {
		PathHelper.EnsureDirForFileExists(path);

        BinaryFormatter bin = GetBinFormatter();
        using (Stream stream = File.Open(path, FileMode.Open))
        {
            var data = (T)bin.Deserialize(stream);
            return data;
        }
    }

    private BinaryFormatter GetBinFormatter()
    {
        var bin = new BinaryFormatter();

        var ss = new SurrogateSelector();
        ss.AddSurrogate(typeof(Vector4),
                        new StreamingContext(StreamingContextStates.All),
                        new Vector4SerializationSurrogate());
        ss.AddSurrogate(typeof(Vector3),
                        new StreamingContext(StreamingContextStates.All),
                        new Vector3SerializationSurrogate());

        bin.SurrogateSelector = ss;

        return bin;
    }
}
