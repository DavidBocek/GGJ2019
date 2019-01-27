using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using DG.Tweening;
using KinematicCharacterController;

public class SwarmController : BaseCharacterController
{
	public float moveSpeedTowardPlayer;
	public float addedLungeSpeed;
	public float lungeDuration;
	public int attackDamage;
	public float maxDistCanSeeIdle;
	public float maxDistCanSeeCombat;
	public float attackBeginDistance;
	public float attackTime;
	public float delayBeforeAttack;
	public GameObject attackCollider;

	private enum eSwarmAIState
	{
		IDLE,
		MOVING,
		ATTACKING
	}

	private float m_timeEnteredCurrentState = -1f;
	private eSwarmAIState m_currentState = eSwarmAIState.IDLE;
	private bool m_canRotate = true;
	private bool m_canTranslate = true;
	private bool m_canSeePlayer = false;
	private bool m_doLunge = false;
	private Vector3 m_lastKnownPlayerPos = Vector3.zero;
	private GameObject m_player;


	void Start()
	{
		EnterState( eSwarmAIState.IDLE );
		m_player = GameObject.FindWithTag( "Player" );
	}

	#region updates
	void Update()
	{
		UpdateState();

		Debug.Log( m_doLunge );
	}

	private void UpdateState()
	{
		int layerMask = ( 1 << 2 );
		layerMask = ~layerMask;

		Vector3 dirToPlayer = (m_player.transform.position - transform.position).normalized;
		RaycastHit hit;
		float distToPlayer = float.MaxValue;
		m_canSeePlayer = false;

		float distToCast = m_currentState == eSwarmAIState.IDLE ? maxDistCanSeeIdle : maxDistCanSeeCombat;
		if ( Physics.Raycast( transform.position, dirToPlayer, out hit, distToCast, layerMask ) )
		{
			if ( hit.collider.gameObject.CompareTag( "Player" ) )
			{
				m_canSeePlayer = true;
				m_lastKnownPlayerPos = hit.collider.gameObject.transform.position;
				distToPlayer = hit.distance;
			}
		}

		float timeSinceCurState = Time.time - m_timeEnteredCurrentState;

		switch ( m_currentState )
		{
			case eSwarmAIState.IDLE:
				if ( m_canSeePlayer )
				{
					EnterState( eSwarmAIState.MOVING );
				}
				break;
			case eSwarmAIState.MOVING:
				if ( distToPlayer < attackBeginDistance )
				{
					EnterState( eSwarmAIState.ATTACKING );
				}
				break;
			case eSwarmAIState.ATTACKING:
				if ( timeSinceCurState >= attackTime )
				{
					EnterState( eSwarmAIState.MOVING );
				}
				break;
		}
	}

	private void EnterState( eSwarmAIState newState )
	{
		eSwarmAIState oldState = m_currentState;

		switch ( newState )
		{
			case eSwarmAIState.IDLE:
				m_canRotate = false;
				m_canTranslate = false;
				break;
			case eSwarmAIState.MOVING:
				m_canRotate = true;
				m_canTranslate = true;
				break;
			case eSwarmAIState.ATTACKING:
				m_canRotate = false;
				m_canTranslate = true;
				Timing.RunCoroutineSingleton( Attack(), gameObject, SingletonBehavior.Abort );
				break;
		}

		m_timeEnteredCurrentState = Time.time;
		m_currentState = newState;
	}


	private IEnumerator<float> Attack()
	{
		yield return Timing.WaitForSeconds( delayBeforeAttack );

		m_doLunge = true;
		BoxCollider attackColliderComp = attackCollider.GetComponent<BoxCollider>();
		attackColliderComp.enabled = true;

		yield return Timing.WaitForSeconds( lungeDuration );

		m_doLunge = false;
		attackColliderComp.enabled = false;

		yield break;
	}

	private void OnTriggerEnter( Collider collider )
	{
		HealthController healthController = collider.gameObject.GetComponent<HealthController>();
		if ( healthController == null || !collider.gameObject.CompareTag( "Player" ) )
			return;

		healthController.HealthController_TakeDamage( attackDamage );
	}

	public override void UpdateRotation( ref Quaternion currentRotation, float deltaTime )
	{
		if ( !m_canRotate )
		{
			return;
		}

		Vector3 dirToPlayer = ( m_lastKnownPlayerPos - transform.position );
		dirToPlayer.y = 0;
		dirToPlayer.Normalize();

		currentRotation = Quaternion.LookRotation( dirToPlayer );
	}

	public override void UpdateVelocity( ref Vector3 currentVelocity, float deltaTime )
	{
		if ( !m_canTranslate )
		{
			currentVelocity = Vector3.zero;
			return;
		}
		Vector3 vecToPlayer = m_lastKnownPlayerPos - transform.position;
		float distanceToPlayer = vecToPlayer.magnitude;

		Vector3 localVel = Vector3.zero;// transform.InverseTransformDirection( currentVelocity );

		localVel.z = m_currentState == eSwarmAIState.ATTACKING ? moveSpeedTowardPlayer / 1.5f : moveSpeedTowardPlayer;

		if ( m_doLunge )
		{
			localVel.z += addedLungeSpeed;
		}

		currentVelocity = transform.TransformDirection( localVel );
	}

	public override void AfterCharacterUpdate( float deltaTime )
	{

	}

	public override void BeforeCharacterUpdate( float deltaTime )
	{

	}

	public override void PostGroundingUpdate( float deltaTime )
	{

	}
	#endregion

	#region callbacks

	public void OnDeath( int damage )
	{
		// play death anim

		Destroy( gameObject );
	}
	public override void OnGroundHit( Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport )
	{

	}

	public override void OnMovementHit( Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport )
	{

	}

	public override bool IsColliderValidForCollisions( Collider coll )
	{
		return true;
	}

	public override void ProcessHitStabilityReport( Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport )
	{

	}
	#endregion

}