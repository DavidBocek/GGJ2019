using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using DG.Tweening;
using KinematicCharacterController;
using UnityEngine.Assertions;

public class PlayerController : BaseCharacterController
{
	[Header("Gameplay")]
    public float maxMoveSpeedPerSecond = 1.0f;
    public float fasterWalkScale = 2.0f;
    public int baseAttackDamage = 10;
    public float attackDuration = 0.2f;
    public float attackCooldown = 0.2f;
    public float attackMoveForwardSpeedPerSecond = 1.0f;
    public float dashDuration = 0.5f;
    public float dashCooldown = 0.2f;
    public float dashDistance = 1.0f;
    public GameObject attackCollider;
    public float fasterWalkRequiredDuration = 0.5f;

	[Header("FX")]
	public ParticleSystem footstepsFX;
	public ParticleSystem dashFootstepsFX;
	public ParticleSystem deathFX;
	private SwipeTrail swipeTrail;

    private GameObject pcMainCamera;
    private float nextAttackReadyTime = 0.0f;
    private float attackDeactivateTime = 0.0f;
    private float nextDashReadyTime = 0.0f;
    private float dashEndTime = 0.0f;
    private GameObject currentSpawnPoint = null;
    private Animator animator = null;
    private int legsLayerIndex = -1;
    private float timeSpentWalking = 0.0f;

    private bool debug_drawAttackCollider = false;
    private bool isDead = false;

	void Start()
	{
		pcMainCamera = GameObject.FindGameObjectWithTag( "MainCamera" );
        Assert.AreNotEqual( pcMainCamera, null );
        Assert.AreNotEqual( gameObject.GetComponent<HealthController>(), null );

        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag( "SpawnPoint" );
        for ( int i = 0; i < spawnPoints.Length; ++i )
        {
            // make sure there's only 1 level start per level
            Assert.IsTrue( !spawnPoints[i].GetComponent<SpawnPointController>().isLevelStart || currentSpawnPoint == null );
			if (spawnPoints[i].GetComponent<SpawnPointController>().isLevelStart)
			{
				currentSpawnPoint = spawnPoints[i];
				currentSpawnPoint.GetComponent<SpawnPointController>().Activate();
			}
        }

        if ( spawnPoints.Length == 0 )
        {
            Debug.LogWarning( "No spawn points at all in this level!" );
        }
        if ( currentSpawnPoint == null )
        {
            Debug.LogWarning( "No starting spawn point found in this level!" );
        }

        animator = GetComponentInChildren<Animator>();
        Assert.IsTrue( animator != null );

        legsLayerIndex = animator.GetLayerIndex( "Legs Layer" );

		swipeTrail = GetComponentInChildren<SwipeTrail>();
	}

    void OnTriggerEnter( Collider collider ) 
    {
        HealthController healthController = collider.gameObject.GetComponent<HealthController>();
        if ( healthController == null )
            return;

        healthController.HealthController_TakeDamage( baseAttackDamage );
    }

    private bool PlayerController_GetDirectionFromInput( ref Vector3 outputDirection )
    {
		Vector3 input = new Vector3( Input.GetAxisRaw( "Horizontal" ), 0, Input.GetAxisRaw( "Vertical" ) );
		input.Normalize();
        if ( input == Vector3.zero )
            return false;
        
        Vector3 camForward = pcMainCamera.transform.forward;
        Vector3 camRight = pcMainCamera.transform.right;

        Vector3 camForward2DPlane = new Vector3( camForward.x, 0, camForward.z ).normalized;
        Vector3 camRight2DPlane = new Vector3( camRight.x, 0, camRight.z ).normalized;
		
        outputDirection = input.x * camRight2DPlane + input.z * camForward2DPlane;

        return true;
    }

    public void OnDamage( int damage )
    {
        print( "player took damage " + damage.ToString() );
    }

    public void OnDeath( int damage )
    {
		if ( isDead )
			return;

        isDead = true;
        animator.SetBool( "IsDead", true );
		deathFX.Play();
    }

	public void SetSpawnPoint( GameObject spawnPoint )
	{
		currentSpawnPoint.GetComponent<SpawnPointController>().Deactivate();
		currentSpawnPoint = spawnPoint;
		currentSpawnPoint.GetComponent<SpawnPointController>().Activate();
	}

    private bool PlayerController_CanAct()
    {
        if ( isDead )
            return false;

        if ( attackDeactivateTime != 0.0f )
            return false;

        if ( dashEndTime != 0.0f )
            return false;
        
        return true;
    }

	#region updates
	private void PlayerController_UpdateInput()
    {
        if ( !PlayerController_CanAct() )
            return;
        
        float timeNow = Time.time;
        if ( Input.GetButtonDown( "Attack" ) && timeNow >= nextAttackReadyTime )
        {
            BoxCollider attackColliderComp = attackCollider.GetComponent<BoxCollider>();
            attackColliderComp.enabled = true;
            attackDeactivateTime = timeNow + attackDuration;
            nextAttackReadyTime = attackDeactivateTime + attackCooldown;

			bool swipeLeft = !(animator.GetCurrentAnimatorStateInfo(0).IsName("SkelArmature|Attack1") || animator.GetCurrentAnimatorStateInfo(0).IsName("SkelArmature|Attack3"));
			swipeTrail.Swipe(swipeLeft);

            animator.SetTrigger( "AttackTrigger" );

            animator.SetLayerWeight( legsLayerIndex, 1.0f );
        }
        else if ( Input.GetButtonDown( "Dash" ) && timeNow >= nextDashReadyTime )
        {
            dashEndTime = timeNow + dashDuration;
            nextDashReadyTime = dashEndTime + dashCooldown;
            
            animator.SetTrigger( "DashTrigger" );
			dashFootstepsFX.Play();
			Timing.CallDelayed(dashDuration, delegate { if (dashFootstepsFX.isPlaying) dashFootstepsFX.Stop(); });
        }
    }
    
    void Update()
    {
        // debug updates
        Assert.AreNotEqual( attackCollider, null );
        BoxCollider attackColliderComp = attackCollider.GetComponent<BoxCollider>();

        if ( debug_drawAttackCollider )
        {
            MeshRenderer attackColliderRenderer = attackCollider.GetComponent<MeshRenderer>();
            attackColliderRenderer.enabled = attackColliderComp.enabled;
        }

        if ( Input.GetKeyDown( "p" ))
        {
            gameObject.GetComponent<HealthController>().HealthController_TakeDamage( 50 );
        }

        // input updates
        float timeNow = Time.time;

        if ( isDead )
        {
            if ( Input.GetButtonDown( "Submit" ) )
            {
                if ( currentSpawnPoint == null )
                {
                    Debug.LogWarning( "No spawn point on player death!" );
                }
                else
                {
                    gameObject.GetComponent<KinematicCharacterMotor>().SetPosition( currentSpawnPoint.transform.position );
                }

                gameObject.GetComponent<HealthController>().HealthController_HealToFull();
                
                Transform modelTransform = transform.Find( "skele" );
                
                modelTransform.localScale = Vector3.zero;
                modelTransform.DOScale( 1.0f, 0.6f );

                isDead = false;
                animator.SetBool( "IsDead", false );
            }
        }

        PlayerController_UpdateInput();

        if ( timeNow >= nextAttackReadyTime )
        {
            animator.SetLayerWeight( legsLayerIndex, 0.0f );
        }

        // post input update
        if ( attackDeactivateTime != 0.0f && timeNow >= attackDeactivateTime )
        {
            attackColliderComp.enabled = false;
            attackDeactivateTime = 0.0f;
        }
        if ( dashEndTime != 0.0f && timeNow >= dashEndTime )
        {
            dashEndTime = 0.0f;
        }

    }

	public override void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
	{
        if ( !PlayerController_CanAct() )
            return;

        Vector3 finalDirection = new Vector3();
        if ( !PlayerController_GetDirectionFromInput( ref finalDirection ) )
            return;

        currentRotation.SetLookRotation( finalDirection );
	}

	public override void UpdateVelocity( ref Vector3 currentVelocity, float deltaTime )
	{
        currentVelocity = Vector3.zero;
        bool isWalking = false;

        if ( PlayerController_CanAct() )
        {
            Vector3 finalDirection = new Vector3();
            if ( PlayerController_GetDirectionFromInput( ref finalDirection ) )
            {
                Vector2 finalVelocity = deltaTime * maxMoveSpeedPerSecond * new Vector2( finalDirection.x, finalDirection.z );
                if ( timeSpentWalking >= fasterWalkRequiredDuration )
                {
                    finalVelocity *= fasterWalkScale;
                    animator.SetFloat( "WalkSpeedMultiplier", fasterWalkScale );
                }
                else
                {
                    animator.SetFloat( "WalkSpeedMultiplier", 1.0f );
                }

                isWalking = true;

                currentVelocity.x = finalVelocity.x;
                currentVelocity.z = finalVelocity.y;
            }
        }
        else if ( attackDeactivateTime != 0.0f )
        {
            Vector3 forward2D = new Vector3( gameObject.transform.forward.x, 0, gameObject.transform.forward.z );
            currentVelocity = deltaTime * forward2D * attackMoveForwardSpeedPerSecond;
        }
        else if ( dashEndTime != 0.0f )
        {
            Vector3 forward2D = new Vector3( gameObject.transform.forward.x, 0, gameObject.transform.forward.z );
            currentVelocity = deltaTime * forward2D * dashDistance / dashDuration;
        }

        animator.SetBool( "IsWalking", isWalking );
        if ( isWalking )
        {
            timeSpentWalking += Time.deltaTime;
			if ( !footstepsFX.isPlaying )
			{
				footstepsFX.Play();
			}
        }
        else
        {
            timeSpentWalking = 0.0f;
			if ( footstepsFX.isPlaying )
			{
				footstepsFX.Stop();
			}
        }
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

	public override bool IsColliderValidForCollisions( Collider coll )
	{
        if ( dashEndTime != 0.0f && coll.gameObject.tag == "Enemy" )
            return false;

		return true;
	}

	public override void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
	{
		
	}
	#endregion

}
