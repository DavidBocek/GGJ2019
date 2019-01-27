using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBoxBehaviour : MonoBehaviour
{
	public int damagePerTick;
	public float secondsBetweenTicks;
	public float lifeTime;
	public BoxCollider boxCollider;

	private float m_lastTickTime;
	private float m_deathTime;
	private DemonAttackParticleSpawner particles;
	private TraceBoxWithLine lineDrawer;
    // Start is called before the first frame update
    void Start()
    {
		m_lastTickTime = Time.time;
		particles = GetComponent<DemonAttackParticleSpawner>();
		particles.SpawnParticles( lifeTime );
		lineDrawer = GetComponent<TraceBoxWithLine>();
		lineDrawer.DrawLineFX( lifeTime );
		m_deathTime = Time.time + lifeTime;
	}

    // Update is called once per frame
    void Update()
    {
        if ( Time.time > m_deathTime )
		{
			Destroy( gameObject );
		}
    }

	private void OnTriggerStay( Collider other )
	{
		if ( other.gameObject.CompareTag( "Player" ) )
		{
			if ( Time.time - m_lastTickTime > secondsBetweenTicks )
			{
				HealthController healthController = other.gameObject.GetComponent<HealthController>();

				if ( healthController != null)
				{
					m_lastTickTime = Time.time;
					healthController.HealthController_TakeDamage( damagePerTick );
				}
			}
		}
	}
}
