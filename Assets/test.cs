using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var euler = transform.rotation.eulerAngles;
        Debug.Log(euler);
        Debug.Log(transform.position);

        transform.rotation = Quaternion.Euler(new Vector3(euler.x, 0, euler.z));
        Debug.Log(euler);
    }
}
