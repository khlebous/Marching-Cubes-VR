using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class RotationTests
{

    [Test]
    public void SimpleRotationTest()
    {
        var objectPosition = new Vector3(0, 0, 0);
        var objectRotation = new Vector3(0, 0, 0);

        var startPosition = new Vector3(0, 2, 0);
        var currentPosition = new Vector3(0, 2, -2);


        var newRotation = RotationHelper.GetRotation(objectPosition, objectRotation, startPosition, currentPosition);


        Assert.AreEqual(new Vector3(-45, 0, 0), newRotation);
    }

    [Test]
    public void RotateHorizontalIsolatedTest()
    {
        var objectPosition = new Vector3(0, 0, 0);
        var objectRotation = new Vector3(0, 0, 0);

        var startPosition = new Vector3(0, 0, -2);
        var currentPosition = new Vector3(2, 0, -2);


        var newRotation = RotationHelper.GetRotation(objectPosition, objectRotation, startPosition, currentPosition);


        Assert.AreEqual(new Vector3(0, -45, 0), newRotation);
    }

    [Test]
    public void RotateHorizontalTest()
    {
        var objectPosition = new Vector3(0, 0, 0);
        var objectRotation = new Vector3(0, 0, 0);

        var startPosition = new Vector3(0, 0, -2);
        var currentPosition = new Vector3(2, 1, -2);


        var newRotation = RotationHelper.GetRotation(objectPosition, objectRotation, startPosition, currentPosition);


        Assert.AreEqual(new Vector3(0, -45, 0), newRotation);
    }

    [Test]
    public void RotateVerticalTest()
    {
        var objectPosition = new Vector3(0, 0, 0);
        var objectRotation = new Vector3(0, 0, 0);

        var startPosition = new Vector3(0, 0, -2);
        var currentPosition = new Vector3(1, -2, -2);


        var newRotation = RotationHelper.GetRotation(objectPosition, objectRotation, startPosition, currentPosition);


        Assert.AreEqual(new Vector3(-45, 0, 0), newRotation);
    }

    [Test]
    public void RotateVerticalBoundaryTest()
    {
        var objectPosition = new Vector3(0, 0, 0);
        var objectRotation = new Vector3(50, 0, 0);

        var startPosition = new Vector3(0, 2, -2);
        var currentPosition = new Vector3(0, 0, -2);


        var newRotation = RotationHelper.GetRotation(objectPosition, objectRotation, startPosition, currentPosition);


        Assert.AreEqual(new Vector3(0, 0, 0), newRotation);
    }


    //[Test]//max does not work
    //public void RotateVerticalMaxTest()
    //{
    //    var objectPosition = new Vector3(0, 0, 0);
    //    var objectRotation = new Vector3(-80, 0, 0);

    //    var startPosition = new Vector3(0, 2, 0);
    //    var currentPosition = new Vector3(0, 0, -2);


    //    var rotator = new Rotator();
    //    var newRotation = rotator.GetRotation_5(objectPosition, objectRotation, startPosition, currentPosition);


    //    Assert.AreEqual(new Vector3(-90, 0, 0), newRotation);
    //}

}
