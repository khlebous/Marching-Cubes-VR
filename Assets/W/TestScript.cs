using Assets.MarchingCubesGPU.Scripts;
using MarchingCubesGPUProject;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public Transform startCube;
    public Transform currentCube;

    private Vector3 startRotation;

    void Start()
    {
        startRotation = transform.rotation.eulerAngles;

        //List<Tuple<Vector3, Vector3>> list = new List<Tuple<Vector3, Vector3>>();


        //var rot11 = new Vector3(91,180,180);

        //var rot22 = Quaternion.Euler(rot11).eulerAngles;

        //for (int i = -179; i < 180; i++)
        //{
        //    for (int j = -179; j < 180; j++)
        //    {
        //        for (int k = -179; k < 180; k++)
        //        {
        //            if (k > -178)
        //                break;
        //            var rot1 = new Vector3(i, j, k);
        //            var rot2 = Quaternion.Euler(rot1).eulerAngles;
        //            list.Add(new Tuple<Vector3, Vector3>(rot1, rot2));
        //            //if (rot1 != rot2)
        //            //    Debug.Log(rot1 + " " + rot2);
        //        }
        //    }
        //}

    }

    void Update()
    {
        var result = new Rotator().GetRotation_5(transform.position,
                                                    startRotation,
                                                    startCube.transform.position,
                                                    currentCube.transform.position);
        transform.rotation = Quaternion.Euler(result);
    }
}
