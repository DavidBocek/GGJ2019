﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using DG.Tweening;
using KinematicCharacterController;

public class PlayerController : BaseCharacterController
{
    private GameObject pcMainCamera;
    
    public float maxMoveSpeed = 1.0f;

	void Start()
	{
		pcMainCamera = GameObject.FindGameObjectWithTag( "MainCamera" );
	}

    private bool PlayerController_GetDirectionFromInput( ref Vector3 outputDirection )
    {
		Vector3 input = new Vector3( Input.GetAxisRaw( "Horizontal" ), 0, Input.GetAxisRaw( "Vertical" ) );
        if ( input == Vector3.zero )
            return false;
        
        Vector3 camForward = pcMainCamera.transform.forward;
        Vector3 camRight = pcMainCamera.transform.right;

        Vector3 camForward2DPlane = new Vector3( camForward.x, 0, camForward.z ).normalized;
        Vector3 camRight2DPlane = new Vector3( camRight.x, 0, camRight.z ).normalized;
		
        outputDirection = input.x * camRight2DPlane + input.z * camForward2DPlane;

        return true;
    }

	#region updates
	void Update()
	{

    }

	public override void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
	{
        Vector3 finalDirection = new Vector3();
        if ( !PlayerController_GetDirectionFromInput( ref finalDirection ) )
            return;

        currentRotation.SetLookRotation( finalDirection );
	}

	public override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
	{
        currentVelocity = Vector3.zero;

        Vector3 finalDirection = new Vector3();
        if ( !PlayerController_GetDirectionFromInput( ref finalDirection ) )
            return;

        Vector2 finalVelocity = maxMoveSpeed * new Vector2( finalDirection.x, finalDirection.z );

        currentVelocity.x = finalVelocity.x;
        currentVelocity.z = finalVelocity.y;
        
	}

	public override void AfterCharacterUpdate(float deltaTime)
	{
		
	}

	public override void BeforeCharacterUpdate(float deltaTime)
	{
		
	}

	public override void PostGroundingUpdate(float deltaTime)
	{
		
	}
	#endregion

	#region callbacks
	public override void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
	{
		
	}

	public override void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
	{
		
	}

	public override bool IsColliderValidForCollisions(Collider coll)
	{
		return true;
	}

	public override void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
	{
		
	}
	#endregion

}
