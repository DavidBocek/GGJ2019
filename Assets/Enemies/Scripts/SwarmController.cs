﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using DG.Tweening;
using KinematicCharacterController;

public class SwarmController : BaseCharacterController
{
	[Header("Gameplay")]
	public float moveSpeedTowardPlayer;
	public float addedLungeSpeed;
	public float lungeDuration;
	public float lungeMinActivationDist;
	public int attackDamage;
	public float maxDistCanSeeIdle;
	public float maxDistCanSeeCombat;
	public float attackBeginDistance;
	public float attackTime;
	public float delayBeforeAttack;
	public GameObject attackCollider;

	[Header("FX")]
	public GameObject deathFX;

    [Header("Sound")]
    public float flapDelay = 0.3f;

	private Animator m_animator;

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
    private float m_nextFlapTime = 0.0f;
    private AudioSource m_flapSource = null;

	void Start()
	{
		EnterState( eSwarmAIState.IDLE );
		m_player = GameObject.FindWithTag( "Player" );
		m_animator = GetComponentInChildren<Animator>();

        m_nextFlapTime = Time.time + flapDelay;
	}

	#region updates
	void Update()
	{
		UpdateState();
        //UpdateSound();
	}

    private void UpdateSound()
    {
        if ( Time.time > m_nextFlapTime )
        {
            m_nextFlapTime = Time.time + flapDelay;
            if ( m_flapSource != null )
                m_flapSource.Stop();

            m_flapSource = gameObject.GetComponent<RandomAudioPlayer>().PlayRandomSound( "batFlap", true );
        }
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
				m_animator.SetBool("IsMoving", false);
				break;
			case eSwarmAIState.MOVING:
				if ( distToPlayer < attackBeginDistance )
				{
					EnterState( eSwarmAIState.ATTACKING );
				}
				m_animator.SetBool("IsMoving", true);
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

		if ( Vector3.Distance(m_player.transform.position, transform.position) > lungeMinActivationDist)
			m_doLunge = true;
		BoxCollider attackColliderComp = attackCollider.GetComponent<BoxCollider>();
		attackColliderComp.enabled = true;

        GetComponent<RandomAudioPlayer>().PlayRandomSound( "batAttack", true );

		m_animator.SetTrigger("Attack");

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
		//oops this should be in health but oh well
		GameObject deathFXInst = GameObject.Instantiate(deathFX, transform.position, Quaternion.identity);
		Destroy(deathFXInst, 2f);


		Timing.KillCoroutines( gameObject );
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