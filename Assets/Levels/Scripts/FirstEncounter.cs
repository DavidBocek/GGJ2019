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


	public GameObject[] slimeSpawners;
	public UnitSpawn[] waveOneSpawns;
	public UnitSpawn[] waveTwoSpawns;

	public Gate[] gatesToOpenOnComplete;

	public GameObject batObj;
	public GameObject eyeObj;
	public GameObject demonObj;

	private GameObject m_player;


	private bool m_hasStarted = false;

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
		List<GameObject> spawnedUnits =  new List<GameObject>();
		foreach (UnitSpawn spawn in waveOneSpawns)
		{
			yield return Timing.WaitForSeconds(spawn.delay);
			for (int i=0; i<spawn.count; i++)
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

		while (!AllUnitsDead(spawnedUnits))
		{
			yield return Timing.WaitForSeconds(0.2f);
		}

		spawnedUnits = new List<GameObject>();
		foreach (UnitSpawn spawn in waveTwoSpawns)
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

		while (!AllUnitsDead(spawnedUnits))
		{
			yield return Timing.WaitForSeconds(0.2f);
		}

		foreach (Gate gate in gatesToOpenOnComplete)
		{
			gate.GateOpen();
		}
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
				spawnedUnit = GameObject.Instantiate(batObj, location.position, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
				break;
			case UnitType.DEMON:
				spawnedUnit = GameObject.Instantiate(demonObj, location.position, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
				break;
			case UnitType.EYE:
				spawnedUnit = GameObject.Instantiate(eyeObj, location.position, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
				break;
		}
		if (!spawnedUnit)
			return null;

		spawnedUnit.SendMessage("OnSpawnFromSpawner", SendMessageOptions.DontRequireReceiver);
		return spawnedUnit;
	}
}
