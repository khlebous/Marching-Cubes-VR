using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;

public class McObject
{
    public Guid ModelGuid { get; set; }
    public GameObject GameObject { get; set; }

    public McObject()
    {
    }

    public McObject(Guid modelGuid, GameObject gameObject)
    {
        ModelGuid = modelGuid;
        GameObject = gameObject;
    }
}


