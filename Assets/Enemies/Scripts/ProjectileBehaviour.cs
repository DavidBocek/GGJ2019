using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
	public float moveSpeed;
	public GameObject impactFX;

	private const float TIME_TO_LIVE = 3.0f;
	private const int DAMAGE = 50;
	private float m_spawnTime;
    // Start is called before the first frame update
    void Start()
    {
		m_spawnTime = Time.time;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		Vector3 moveDir = transform.TransformDirection( Vector3.forward ).normalized;

		transform.position += ( moveDir * moveSpeed * Time.fixedDeltaTime );
		RaycastHit hit;

		LayerMask layerMask = 1 << 2;
		layerMask = ~layerMask;

		if ( Physics.Raycast( transform.position, moveDir, out hit, moveSpeed * Time.fixedDeltaTime,  layerMask ) )
		{
			HealthController thingToDamage = hit.collider.gameObject.GetComponent<HealthController>();
			if ( thingToDamage != null )
			{
				thingToDamage.HealthController_TakeDamage( DAMAGE );
			}
			GameObject impactFXInst = GameObject.Instantiate( impactFX, hit.point, Quaternion.identity );
			impactFXInst.transform.rotation = Quaternion.Slerp( Quaternion.LookRotation( hit.normal ), Quaternion.LookRotation( Vector3.up, Vector3.back ), .2f );
			Destroy( impactFXInst, 1.0f );
			Destroy( gameObject );
		}
		else if ( Time.time > m_spawnTime + TIME_TO_LIVE )
		{
			Destroy( gameObject );
		}
	}
}
