using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBoxBehaviour : MonoBehaviour
{
	public int damagePerTick;
	public float secondsBetweenTicks;
	public float lifeTime;
	public BoxCollider boxCollider;
    public float fireSoundDelay = 0.2f;

	private float m_lastTickTime;
	private float m_deathTime;
	private DemonAttackParticleSpawner particles;
	private TraceBoxWithLine lineDrawer;
    private float m_nextFireSoundTime;
    private AudioSource m_fireSource = null;

    // Start is called before the first frame update
    void Start()
    {
		m_lastTickTime = Time.time;
		particles = GetComponent<DemonAttackParticleSpawner>();
		particles.SpawnParticles( lifeTime );
		lineDrawer = GetComponent<TraceBoxWithLine>();
		lineDrawer.DrawLineFX( lifeTime );
		m_deathTime = Time.time + lifeTime;
        m_nextFireSoundTime = Time.time;
	}

    // Update is called once per frame
    void Update()
    {
        if ( Time.time >= m_nextFireSoundTime )
        {
            if ( m_fireSource != null )
                m_fireSource.Stop();

            m_fireSource = GetComponent<RandomAudioPlayer>().PlayRandomSound( "demonFire", true );
            m_nextFireSoundTime = Time.time + fireSoundDelay;
        }

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
