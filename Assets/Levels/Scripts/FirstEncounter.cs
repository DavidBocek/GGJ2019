using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using DG.Tweening;

public class FirstEncounter : MonoBehaviour
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

	[System.Serializable]
	public class Wave
	{
		public UnitSpawn[] spawns;
	}

	public AudioClip youDidIt;


	public GameObject[] slimeSpawners;
	public Wave[] waves;

	public Gate[] gatesToOpenOnComplete;

	public GameObject batObj;
	public GameObject eyeObj;
	public GameObject demonObj;

	private GameObject m_player;


	public bool m_hasStarted = false;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player") && !m_hasStarted)
		{
			StartEncounter();
		}

		m_player = GameObject.FindWithTag("Player");
	}

	private void StartEncounter()
	{
		m_hasStarted = true;
		Timing.RunCoroutine(RunEncounter(), gameObject);
	}

	private IEnumerator<float> RunEncounter()
	{
		foreach (Wave wave in waves)
		{
			List<GameObject> spawnedUnits = new List<GameObject>();
			foreach (UnitSpawn spawn in wave.spawns)
			{
				yield return Timing.WaitForSeconds(spawn.delay);
				for (int i = 0; i < spawn.count; i++)
				{
					while (m_player.GetComponent<HealthController>().currHealth <= 0)
					{
						yield return Timing.WaitForSeconds(0.2f);
					}

					GameObject spawner = slimeSpawners[Random.Range(0, slimeSpawners.Length)];
					spawnedUnits.Add(SpawnUnit(spawn.type, spawner.transform));
					yield return Timing.WaitForSeconds(Random.Range(0.1f, 0.15f));
				}
			}

			float emergencyTimeStart = Time.time;
			while (!AllUnitsDead(spawnedUnits))
			{
				yield return Timing.WaitForSeconds(0.2f);
				if (Time.time >= emergencyTimeStart + 25f)
					break;
			}
		}
		
		foreach (Gate gate in gatesToOpenOnComplete)
		{
			AudioSource.PlayClipAtPoint(youDidIt, m_player.transform.position, 2f);
			gate.GateOpen();
		}

		GetComponent<GatesTrigger>().neverTriggerAgain = true;
	}

	private bool AllUnitsDead(List<GameObject> units)
	{
		foreach (GameObject unit in units)
		{
			if (unit != null)
			{
				return false;
			}
		}

		return true;
	}

	private GameObject SpawnUnit(UnitType type, Transform location)
	{
		GameObject spawnedUnit = null;
		switch (type)
		{
			case UnitType.BAT:
				spawnedUnit = GameObject.Instantiate(batObj, location.position + Vector3.up * (1.5f - location.position.y), Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
				break;
			case UnitType.DEMON:
				spawnedUnit = GameObject.Instantiate(demonObj, location.position + Vector3.up * (2f - location.position.y), Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
				break;
			case UnitType.EYE:
				spawnedUnit = GameObject.Instantiate(eyeObj, location.position + Vector3.up * (2f - location.position.y), Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
				break;
		}
		if (!spawnedUnit)
			return null;

		spawnedUnit.SendMessage("OnSpawnFromSpawner", SendMessageOptions.DontRequireReceiver);
		return spawnedUnit;
	}
}
