﻿using System.Collections;
using UnityEngine;

public class ScaleWithOculusTouch : MonoBehaviour
{
    [Header("Oculus Touch input")]
    [Tooltip("Button to scale")]
    [SerializeField] private OVRInput.Button buttonY = OVRInput.Button.Three;
    [Tooltip("Controller to follow")]
    private Transform controllerToFollow;

    [Header("Scale multipliers")]
    [SerializeField] private float multiplier = 0.05f;

    private new Transform transform;
    private Vector3 startControllerPosition;

    private Coroutine button_down;
    private Coroutine button_up;

    private void Start()
    {
        transform = GetComponent<Transform>();
        startControllerPosition = Vector3.zero;
    }

    private void StartListening()
    {
        button_down = StartCoroutine(WaitForButton_Down());
    }

    private void StopListening()
    {
        if (null != button_down)
            StopCoroutine(button_down);

        if (null != button_up)
            StopCoroutine(button_up);
    }

    private IEnumerator WaitForButton_Down()
    {
        while (true)
        {
            if (OVRInput.GetDown(buttonY))
            {
                startControllerPosition = controllerToFollow.position;
                StopCoroutine(button_down);
                button_up = StartCoroutine(WaitForButton_Up());
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator WaitForButton_Up()
    {
        while (true)
        {
            var startDist = Vector3.Distance(transform.position, startControllerPosition);
            var currtDist = Vector3.Distance(transform.position, controllerToFollow.transform.position);

            transform.localPosition += Vector3.one * multiplier * (currtDist - startDist);


            if (OVRInput.GetUp(buttonY))
            {
                StopCoroutine(button_up);

                button_down = StartCoroutine(WaitForButton_Down());
            }

            yield return new WaitForEndOfFrame();
        }
    }

    public void SetControllerToFollow(Transform toFollow)
    {
        controllerToFollow = toFollow;
    }

    private void OnEnable()
    {
        StartListening();
    }

    private void OnDisable()
    {
        StopListening();
    }
}