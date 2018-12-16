using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public Transform startCube;
    public Transform currentCube;

    private Vector3 startRotation = new Vector3(30, 45, 60);

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var rotation = (Quaternion.Euler(new Vector3(90, 0, 0)) * Quaternion.Euler(new Vector3(0, 90, 0))).eulerAngles;

        Debug.Log(rotation);

        transform.rotation = Quaternion.Euler(new Rotator().GetRotation_5(transform.position,
                                                                            startRotation,
                                                                            startCube.transform.position,
                                                                            currentCube.transform.position));
    }
}
