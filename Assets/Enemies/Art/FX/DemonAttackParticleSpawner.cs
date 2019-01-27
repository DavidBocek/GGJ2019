using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonAttackParticleSpawner : MonoBehaviour
{
	public BoxCollider box;
	public GameObject fireFX;
	public int numFiresToSpawn;
	public float fireBurnDuration;

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.O))
			SpawnParticles();
	}

	public void SpawnParticles()
	{
		float boxX = box.size.x / 2f * box.transform.localScale.x;
		float boxY = box.size.y / 2f * box.transform.localScale.y;
		float boxZ = box.size.z / 2f * box.transform.localScale.z;
		
		for (int i=0; i<numFiresToSpawn; i++)
		{
			
			float randX = Random.Range(-boxX, boxX);
			float randZ = Random.Range(-boxZ, boxZ);
			Debug.Log(" x: " + randX + " z: " + randZ);
			Vector3 spawnPoint = box.transform.position + box.center + box.transform.right * randX - box.transform.up * boxY + box.transform.forward * randZ;
			GameObject fireFXInst = GameObject.Instantiate(fireFX, spawnPoint, Quaternion.LookRotation(Vector3.up));
			ParticleSystem ps = fireFXInst.GetComponent<ParticleSystem>();
			var noise = ps.noise;
			noise.scrollSpeedMultiplier = Random.Range(.8f, 1.2f);
			noise.frequency *= Random.Range(0.8f, 1.4f);
			var main = ps.main;
			main.duration = fireBurnDuration;
			Destroy(fireFXInst, fireBurnDuration + 1f);
		}
	}
}
