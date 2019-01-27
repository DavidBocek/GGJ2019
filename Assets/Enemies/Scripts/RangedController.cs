using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using DG.Tweening;
using KinematicCharacterController;

public class RangedController : BaseCharacterController
{
	[Header("Gameplay")]
	public float targetDistFromPlayer;
	public float moveSpeedMax;
	public float maxDistCanSeeIdle;
	public float maxDistCanSeeCombat;
	public float moveTime;
	public float aimTime;
	public float fireTime;
	public float delayBeforeFire;
	public GameObject projectile;
	public Transform muzzlePoint;
	public AnimationCurve speedCurve = new AnimationCurve(
		new Keyframe( 0f, 0f ),
		new Keyframe( 0.25f, 1f ),
		new Keyframe( 0.75f, 1f ),
		new Keyframe( 1f, 0f ) );

	[Header("FX")]
	public ParticleSystem[] eyeChargeFXs;
	public Light eyeChargeLight;
	public LineRenderer eyeAimWarning;
	public LineRenderer eyeChargeWarning;
	public LayerMask warningLaserMask;
	public ParticleSystem eyeBlastMuzzleFX;
	public GameObject deathFX;

	private Animator animator;

	private enum eRangedAIState
	{
		IDLE,
		MOVING,
		AIMING,
		FIRING
	}

	private float m_timeEnteredCurrentState = -1f;
	private float m_speedEvalT = 0f;
	private eRangedAIState m_currentState = eRangedAIState.IDLE;
	private bool m_canRotate = true;
	private bool m_canTranslate = true;
	private bool m_canSeePlayer = false;
	private bool m_movingForward = true;
	private Vector3 m_lastKnownPlayerPos = Vector3.zero;
	private GameObject m_player;


	void Start()
	{
		EnterState( eRangedAIState.IDLE );
		m_player = GameObject.FindWithTag( "Player" );

		animator = GetComponentInChildren<Animator>();

		eyeAimWarning.enabled = false;
		eyeChargeWarning.enabled = false;
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
		m_canSeePlayer = false;

		float distToCast = m_currentState == eRangedAIState.IDLE ? maxDistCanSeeIdle : maxDistCanSeeCombat;
		if ( Physics.Raycast( transform.position, dirToPlayer, out hit, distToCast, layerMask ) )
		{
			if ( hit.collider.gameObject.CompareTag( "Player" ) )
			if ( hit.collider.gameObject.CompareTag( "Player" ) )
			{
				m_canSeePlayer = true;
				Vector3 playerPos = hit.collider.gameObject.transform.position;
				playerPos.y = muzzlePoint.transform.position.y;
				m_lastKnownPlayerPos = playerPos;
			}
		}

		float timeSinceCurState = Time.time - m_timeEnteredCurrentState;

		switch ( m_currentState )
		{
			case eRangedAIState.IDLE:
				if ( m_canSeePlayer )
				{
					EnterState( eRangedAIState.AIMING );
				}
				break;
			case eRangedAIState.AIMING:
				if ( timeSinceCurState >= aimTime )
				{
					EnterState( eRangedAIState.FIRING );
				}
				break;
			case eRangedAIState.FIRING:
				if ( timeSinceCurState >= fireTime )
				{
					EnterState( eRangedAIState.MOVING );
				}
				break;
			case eRangedAIState.MOVING:
				if ( timeSinceCurState >= moveTime )
				{
					if ( m_canSeePlayer )
					{
						EnterState( eRangedAIState.AIMING );
					}
					else
					{
						EnterState( eRangedAIState.IDLE );
					}
				}
				break;
		}
	}

	private void EnterState( eRangedAIState newState )
	{
		eRangedAIState oldState = m_currentState;

		switch ( newState )
		{
			case eRangedAIState.IDLE:
				m_canRotate = false;
				m_canTranslate = false;
				eyeChargeWarning.enabled = false;
				eyeAimWarning.enabled = false;
				break;
			case eRangedAIState.MOVING:
				m_canRotate = true;
				m_canTranslate = true;
				Vector3 vecToPlayer = m_lastKnownPlayerPos - transform.position;
				float distanceToPlayer = vecToPlayer.magnitude;
				m_movingForward = distanceToPlayer > targetDistFromPlayer;
				eyeChargeWarning.enabled = false;
				eyeAimWarning.enabled = false;
				break;
			case eRangedAIState.AIMING:
				m_canRotate = true;
				m_canTranslate = false;
				animator.SetTrigger("BeginAim");
				eyeAimWarning.enabled = true;
				Timing.RunCoroutine( AimThread(), gameObject );
				break;
			case eRangedAIState.FIRING:
				m_canRotate = false;
				m_canTranslate = false;
				animator.SetTrigger("BeginCharge");
				eyeChargeWarning.enabled = true;
				foreach (ParticleSystem system in eyeChargeFXs)
				{
					system.Play();
				}
				eyeChargeLight.enabled = true;
				eyeAimWarning.enabled = false;
				eyeChargeWarning.enabled = true;
				Timing.CallDelayed( delayBeforeFire, Attack, gameObject );
				break;
		}

		m_timeEnteredCurrentState = Time.time;
		m_currentState = newState;
	}

	private IEnumerator<float> AimThread()
	{
		yield return Timing.WaitForOneFrame;

		while ( m_currentState == eRangedAIState.AIMING || m_currentState == eRangedAIState.FIRING )
		{
			if ( m_currentState == eRangedAIState.AIMING )
			{
				RaycastHit hit = new RaycastHit();
				Vector3 endPos;
				if (Physics.Raycast(muzzlePoint.transform.position, (m_lastKnownPlayerPos - muzzlePoint.transform.position).normalized, out hit, 500f, warningLaserMask, QueryTriggerInteraction.Ignore))
				{
					endPos = hit.point;
				}
				else
				{
					endPos = muzzlePoint.transform.position + (m_lastKnownPlayerPos - muzzlePoint.transform.position).normalized * 500f;
				}
				eyeAimWarning.SetPosition(0, muzzlePoint.transform.position);
				eyeAimWarning.SetPosition(1, endPos);
			}
			else
			{
				RaycastHit hit = new RaycastHit();
				Vector3 endPos;
				if (Physics.Raycast(muzzlePoint.transform.position, muzzlePoint.transform.forward, out hit, 500f, warningLaserMask, QueryTriggerInteraction.Ignore))
				{
					endPos = hit.point;
				}
				else
				{
					endPos = muzzlePoint.transform.position + muzzlePoint.transform.forward * 500f;
				}
				eyeChargeWarning.SetPosition(0, muzzlePoint.transform.position);
				eyeChargeWarning.SetPosition(1, endPos);
			}
			yield return Timing.WaitForOneFrame;
		}

		yield break;
	}

	private void Attack()
	{
		animator.SetTrigger("Attack");
		foreach (ParticleSystem system in eyeChargeFXs)
		{
			system.Stop();
			system.Clear();
		}
		eyeChargeLight.enabled = false;
		eyeChargeWarning.enabled = false;
		eyeBlastMuzzleFX.Play();
		Instantiate( projectile, muzzlePoint.position, muzzlePoint.rotation );
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
			m_speedEvalT = 0;
			return;
		}
		Vector3 vecToPlayer = m_lastKnownPlayerPos - transform.position;
		float distanceToPlayer = vecToPlayer.magnitude;

		Vector3 localVel = Vector3.zero;// transform.InverseTransformDirection( currentVelocity );
		m_speedEvalT = Mathf.Min( m_speedEvalT, 1f );

		if ( m_movingForward )
		{
			localVel.z = speedCurve.Evaluate( m_speedEvalT ) * moveSpeedMax;

			if ( distanceToPlayer < ( targetDistFromPlayer / 2f ) )
			{
				m_speedEvalT += ( 2f * deltaTime ) / moveTime;
			}
		}
		else
		{
			localVel.z = speedCurve.Evaluate( m_speedEvalT ) * moveSpeedMax * -1;
		}

		m_speedEvalT += deltaTime / moveTime;

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