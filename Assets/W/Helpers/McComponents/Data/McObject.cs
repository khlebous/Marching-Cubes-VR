using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;

public class McObject
{
    public Guid Guid { get; set; }
    public GameObject GameObject { get; set; }

    public McObject()
    {
    }

    public McObject(Guid guid, GameObject gameObject)
    {
        Guid = guid;
        GameObject = gameObject;
    }
}


