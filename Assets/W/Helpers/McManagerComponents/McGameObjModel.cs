using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;

public class McGameObjData
{
    public McData Data { get; set; }
    public GameObject GameObject { get; set; }

    public McGameObjData()
    {
    }

    public McGameObjData(McData data, GameObject gameObject)
    {
        Data = data;
        GameObject = gameObject;
    }
}


