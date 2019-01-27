using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSpawnPointTrigger : MonoBehaviour
{
	GameObject spawnPoint;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			other.GetComponent<PlayerController>().SetSpawnPoint(spawnPoint);
		}
	}
}
