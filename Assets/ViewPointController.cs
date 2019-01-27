using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using KinematicCharacterController;

public class ViewPointController : MonoBehaviour
{
    public Transform cameraTransform;
    public float viewLagSpeed = 3.0f;
    public float forwardPointOffset = 2.0f;

    private GameObject playerObject = null;

    // Start is called before the first frame update
    void Start()
    {
        cameraTransform.LookAt( gameObject.transform.position );
    }

    // Update is called once per frame
    void Update()
    {
        if ( playerObject == null )
            playerObject = GameObject.FindWithTag( "Player" );

        Vector3 playerVelVector = playerObject.GetComponent<KinematicCharacterMotor>().Velocity;
        float playerVelVectorSqMagnitude = playerVelVector.sqrMagnitude;
        float forwardPointOffsetScale = 1.0f;
        if ( playerVelVectorSqMagnitude < 1.0f )
            forwardPointOffsetScale = 0.0f;

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
