using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using DG.Tweening;
using KinematicCharacterController;

public class PlayerController : BaseCharacterController
{
    private GameObject pcMainCamera;

	void Start()
	{
		pcMainCamera = GameObject.FindGameObjectWithTag( "MainCamera" );
	}

	#region updates
	void Update()
	{

	}

	public override void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
	{
		
	}

	public override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
	{
        Vector3 camForward = pcMainCamera.transform.forward;
        Vector3 camRight = pcMainCamera.transform.right;

        Vector2 camForward2DPlane = new Vector2( camForward.x, camForward.z );
        Vector2 camRight2DPlane = new Vector2( camRight.x, camRight.z );

		Vector2 input = new Vector2( Input.GetAxis( "Horizontal" ), Input.GetAxis( "Vertical" ) );
        Vector2 finalVelocity = input.x * camRight2DPlane + input.y * camForward2DPlane;

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
