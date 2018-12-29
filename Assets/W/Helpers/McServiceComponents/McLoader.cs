using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

public class McLoader
{
    private const string _extension = ".bin";

    public void SaveScene(string name, McSceneData data)
    {
        var fileName = GetFilePath(name);
        EnsureDirExists(fileName);

        BinaryFormatter bin = GetBinFormatter();
        using (Stream stream = File.Open(fileName, FileMode.Create))
        {
            bin.Serialize(stream, data);
        }
    }


    public McSceneData LoadScene(string name)
    {
        var fileName = GetFilePath(name);
        EnsureDirExists(fileName);

        BinaryFormatter bin = GetBinFormatter();
        using (Stream stream = File.Open(fileName, FileMode.Open))
        {
            var data = (McSceneData)bin.Deserialize(stream);
            return data;
        }
    }

    public void SaveObj(string name, McData data)
    {
        var fileName = GetFilePath(name);
        EnsureDirExists(fileName);

        BinaryFormatter bin = GetBinFormatter();
        using (Stream stream = File.Open(fileName, FileMode.Create))
        {
            bin.Serialize(stream, data);
        }
    }

    public McData LoadObj(string name)
    {
        var fileName = GetFilePath(name);
        EnsureDirExists(fileName);

        BinaryFormatter bin = GetBinFormatter();
        using (Stream stream = File.Open(fileName, FileMode.Open))
        {
            var data = (McData)bin.Deserialize(stream);
            return data;
        }
    }

    public BinaryFormatter GetBinFormatter()
    {
        var bin = new BinaryFormatter();

        var ss = new SurrogateSelector();
        ss.AddSurrogate(typeof(Vector4),
                        new StreamingContext(StreamingContextStates.All),
                        new Vector4SerializationSurrogate());

        bin.SurrogateSelector = ss;

        return bin;
    }

    public void EnsureDirExists(string path)
    {
        var fileInfo = new FileInfo(path);
        fileInfo.Directory.Create();
    }
    public string GetFilePath(string name)
    {
        var fullPath = Path.Combine(GetRootPath(), name + _extension);

        return fullPath;
    }
    public string GetRootPath()
    {
        var root = Path.Combine(Directory.GetCurrentDirectory(), "saves");
        return root;
    }
}
