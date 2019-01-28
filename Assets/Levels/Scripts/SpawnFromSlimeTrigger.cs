using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFromSlimeTrigger : MonoBehaviour
{
	public enum UnitType
	{
		BAT,
		EYE,
		DEMON
	}

	[System.Serializable]
	public class UnitSpawn
	{
		public UnitType type;
		public int count;
		public float delay;
	}

	public GameObject spawner;
	public UnitSpawn spawnToDo;

	public GameObject batObj;
	public GameObject eyeObj;
	public GameObject demonObj;

	private bool m_hasTriggered = false;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player") && !m_hasTriggered)
		{
			m_hasTriggered = true;
			for (int i=0; i<spawnToDo.count; i++)
			{
				SpawnUnit(spawnToDo.type, spawner.transform);
			}
				
		}
	}

	private GameObject SpawnUnit(UnitType type, Transform location)
	{
		GameObject spawnedUnit = null;
		switch (type)
		{
			case UnitType.BAT:
				spawnedUnit = GameObject.Instantiate(batObj, location.position + Vector3.up * (.5f - location.position.y), Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
				break;
			case UnitType.DEMON:
				spawnedUnit = GameObject.Instantiate(demonObj, location.position + Vector3.up * (1f - location.position.y), Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
				break;
			case UnitType.EYE:
				spawnedUnit = GameObject.Instantiate(eyeObj, location.position + Vector3.up * (1f - location.position.y), Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
				break;
		}
		if (!spawnedUnit)
			return null;

		spawnedUnit.SendMessage("OnSpawnFromSpawner", SendMessageOptions.DontRequireReceiver);
		return spawnedUnit;
	}
}
