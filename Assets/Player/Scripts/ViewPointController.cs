﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using KinematicCharacterController;
using UnityEngine.Assertions;

public class ViewPointController : MonoBehaviour
{
    public Transform cameraTransform;
    public float viewLagSpeed = 3.0f;
    public float forwardPointOffset = 2.0f;
    public float forwardOffsetDuration = 2.0f;

    private GameObject playerObject = null;
    private float forwardOffsetRevertTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        playerObject = GameObject.FindWithTag( "Player" );
        Assert.AreNotEqual( playerObject, null );
        gameObject.transform.position = playerObject.transform.position;

        cameraTransform.LookAt( gameObject.transform.position );
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerVelVector = playerObject.GetComponent<KinematicCharacterMotor>().Velocity;
        float playerVelVectorSqMagnitude = playerVelVector.sqrMagnitude;
        float forwardPointOffsetScale = 0.0f;
        if ( playerVelVectorSqMagnitude > 5.0f )
        {
            forwardOffsetRevertTime = Time.time + forwardOffsetDuration;
            forwardPointOffsetScale = 1.0f;
        }

        if ( forwardOffsetRevertTime >= Time.time )
            forwardPointOffsetScale = 1.0f;
        else
            forwardOffsetRevertTime = 0.0f;

        Vector3 playerPos = playerObject.transform.position;
        Vector3 targetPos = playerPos + playerObject.transform.forward * forwardPointOffset * forwardPointOffsetScale;
        Vector3 usToTargetPoint = targetPos - transform.position;

        if ( usToTargetPoint.magnitude <= 0.01f )
        {
            transform.position = targetPos;
        }
        else
        {
           transform.position += usToTargetPoint * viewLagSpeed * Time.deltaTime;
        }

    }
}
