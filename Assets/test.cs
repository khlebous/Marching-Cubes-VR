using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public Transform startCube;
    public Transform currentCube;

    private Vector3 startRotation;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(new Rotator().GetRotation_5(startRotation,
                                                                            startCube.transform.position,
                                                                            currentCube.transform.position));
    }
}
