using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using DG.Tweening;
using KinematicCharacterController;

public class MeleeController : BaseCharacterController
{
	[Header("Gameplay")]
	public float moveSpeedTowardPlayer;
	public int attackDamage;
	public float maxDistCanSeeIdle;
	public float maxDistCanSeeCombat;
	public float attackBeginDistance;
	public float attackTime;
	public float delayBeforeAttack;
	public float minMoveTime;
	public GameObject attackCollider;
	public GameObject fireBoxPrefab;

	[Header("FX")]
	public GameObject deathFX;
	public Transform muzzle;
	public GameObject attackFX;

	private Animator m_animator;

	private enum eMeleeAIState
	{
		IDLE,
		MOVING,
		ATTACKING
	}

	private float m_timeEnteredCurrentState = -1f;
	private eMeleeAIState m_currentState = eMeleeAIState.IDLE;
	private bool m_canRotate = true;
	private bool m_canTranslate = true;
	private bool m_canSeePlayer = false;
	private Vector3 m_lastKnownPlayerPos = Vector3.zero;
	private GameObject m_player;


	void Start()
	{
		EnterState( eMeleeAIState.IDLE );
		m_player = GameObject.FindWithTag( "Player" );
		m_animator = GetComponentInChildren<Animator>();
	}

	#region updates
	void Update()
	{
		UpdateState();
	}

	private void UpdateState()
	{
		int layerMask = ( 1 << 2 );
		layerMask = ~layerMask;

		Vector3 dirToPlayer = (m_player.transform.position - transform.position).normalized;
		RaycastHit hit;
		float distToPlayer = float.MaxValue;
		m_canSeePlayer = false;

		float distToCast = m_currentState == eMeleeAIState.IDLE ? maxDistCanSeeIdle : maxDistCanSeeCombat;
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
			case eMeleeAIState.IDLE:
				if ( m_canSeePlayer )
				{
					EnterState( eMeleeAIState.MOVING );
				}
				m_animator.SetBool("IsMoving", false);
				break;
			case eMeleeAIState.MOVING:
				if ( distToPlayer < attackBeginDistance && timeSinceCurState > minMoveTime )
				{
					EnterState( eMeleeAIState.ATTACKING );
				}
				m_animator.SetBool("IsMoving", true);
				break;
			case eMeleeAIState.ATTACKING:
				if ( timeSinceCurState >= attackTime )
				{
					EnterState( eMeleeAIState.MOVING );
				}
				break;
		}
	}

	private void EnterState( eMeleeAIState newState )
	{
		eMeleeAIState oldState = m_currentState;

		switch ( newState )
		{
			case eMeleeAIState.IDLE:
				m_canRotate = false;
				m_canTranslate = false;
				break;
			case eMeleeAIState.MOVING:
				m_canRotate = true;
				m_canTranslate = true;
				break;
			case eMeleeAIState.ATTACKING:
				m_canRotate = false;
				m_canTranslate = false;
				m_animator.SetTrigger("Charge");
				Timing.RunCoroutineSingleton( Attack(), Segment.LateUpdate, gameObject, SingletonBehavior.Abort );
				break;
		}

		m_timeEnteredCurrentState = Time.time;
		m_currentState = newState;
	}


	private IEnumerator<float> Attack()
	{
		Quaternion lastRotation = transform.rotation;
		yield return Timing.WaitForOneFrame;

		while ( lastRotation != transform.rotation )
		{
			lastRotation = transform.rotation;
			yield return Timing.WaitForOneFrame;
		}

		BoxCollider attackColliderComp = attackCollider.GetComponent<BoxCollider>();
		GetComponentInChildren<TraceBoxWithLine>().DrawLineFX( delayBeforeAttack );

		yield return Timing.WaitForSeconds( delayBeforeAttack );

		Instantiate( fireBoxPrefab, attackCollider.transform.position, attackCollider.transform.rotation );
		m_animator.SetTrigger("Attack");
		GameObject attackFXInst = GameObject.Instantiate(attackFX, muzzle.position, muzzle.rotation);
		Destroy(attackFXInst, 3f);

		yield break;
	}

	private void OnTriggerEnter( Collider collider )
	{

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

		Vector3 localVel = Vector3.zero;

		if ( distanceToPlayer > attackBeginDistance + 0.25 )
		{
			localVel.z = moveSpeedTowardPlayer;
		}
		else if ( distanceToPlayer < attackBeginDistance - 0.25 )
		{
			localVel.z = -moveSpeedTowardPlayer;
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
		Destroy( deathFXInst, 2f );

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