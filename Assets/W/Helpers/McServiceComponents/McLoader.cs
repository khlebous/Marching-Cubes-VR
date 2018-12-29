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

    public void SaveScene(string path, McSceneData data)
    {
        var fileName = GetFilePath(path);
        EnsureDirForFileExists(fileName);

        BinaryFormatter bin = GetBinFormatter();
        using (Stream stream = File.Open(fileName, FileMode.Create))
        {
            bin.Serialize(stream, data);
        }
    }
    public McSceneData LoadScene(string path)
    {
        var fileName = GetFilePath(path);
        EnsureDirForFileExists(fileName);

        BinaryFormatter bin = GetBinFormatter();
        using (Stream stream = File.Open(fileName, FileMode.Open))
        {
            var data = (McSceneData)bin.Deserialize(stream);
            return data;
        }
    }

    public void SaveObj(string path, McData data)
    {
        var fileName = GetFilePath(path);
        EnsureDirForFileExists(fileName);

        BinaryFormatter bin = GetBinFormatter();
        using (Stream stream = File.Open(fileName, FileMode.Create))
        {
            bin.Serialize(stream, data);
        }
    }
    public McData LoadObj(string path)
    {
        var fileName = GetFilePath(path);
        EnsureDirForFileExists(fileName);

        BinaryFormatter bin = GetBinFormatter();
        using (Stream stream = File.Open(fileName, FileMode.Open))
        {
            var data = (McData)bin.Deserialize(stream);
            return data;
        }
    }

    public List<Guid> GetAllObjGuids(string path)
    {
        var dirPath = Path.Combine(GetRootPath(), path);
        var dirInfo = new DirectoryInfo(GetFilePath(path));

        var guids = new List<Guid>();
        if (dirInfo.Exists)
        {
            foreach (var file in dirInfo.GetFiles())
            {
                if (file.Extension != _extension)
                    continue;

                try
                {
                    var guid = new Guid(Path.GetFileNameWithoutExtension(file.Name));
                    guids.Add(guid);
                }
                catch (Exception)
                {
                }
            }
        }

        return guids;
    }

    private BinaryFormatter GetBinFormatter()
    {
        var bin = new BinaryFormatter();

        var ss = new SurrogateSelector();
        ss.AddSurrogate(typeof(Vector4),
                        new StreamingContext(StreamingContextStates.All),
                        new Vector4SerializationSurrogate());

        bin.SurrogateSelector = ss;

        return bin;
    }

    private void EnsureDirForFileExists(string path)
    {
        var fileInfo = new FileInfo(path);
        fileInfo.Directory.Create();
    }
    private string GetFilePath(string path)
    {
        var fullPath = Path.Combine(GetRootPath(), path + _extension);

        return fullPath;
    }
    private string GetRootPath()
    {
        var root = Path.Combine(Directory.GetCurrentDirectory(), "saves");
        return root;
    }
}
