using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class RotationTests
{

    //[Test]
    //public void SimpleRotationTest()
    //{
    //    var objectRotation = new Vector3(0, 0, 0);
    //    var startPosition = new Vector3(0, 2, 0);
    //    var currentPosition = new Vector3(0, 2, -2);


    //    var rotator = new Rotator();
    //    var newRotation = rotator.GetRotation_5(objectRotation, startPosition, currentPosition);


    //    Assert.AreEqual(new Vector3(-45, 0, 0), newRotation);
    //}

    //[Test]
    //public void RotateBothAxiesTest()
    //{
    //    var objectRotation = new Vector3(0, 0, 0);
    //    var startPosition = new Vector3(0, 2, 0);
    //    var currentPosition = new Vector3(2, 0, -2);


    //    var rotator = new Rotator();
    //    var newRotation = rotator.GetRotation_5(objectRotation, startPosition, currentPosition);


    //    Assert.AreEqual(new Vector3(-45, 0, -45), newRotation);
    //}


    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    [UnityTest]
    public IEnumerator NewEditModeTestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // yield to skip a frame
        yield return null;
    }
}
