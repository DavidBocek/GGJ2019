using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
	public float moveSpeed;

	private const float TIME_TO_LIVE = 3.0f;
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

		if ( Physics.Raycast( transform.position, moveDir, out hit, moveSpeed * Time.fixedDeltaTime ) )
		{
			// check for health component and send damage
			Destroy( gameObject );
		}
		else if ( Time.time > m_spawnTime + TIME_TO_LIVE )
		{
			Destroy( gameObject );
		}
	}
}
