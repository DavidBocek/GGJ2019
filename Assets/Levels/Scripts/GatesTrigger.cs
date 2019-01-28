using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatesTrigger : MonoBehaviour
{
	public Gate[] gatesToOpen;
	public Gate[] gatesToClose;
	public bool onlyFireOnce;
	public bool onlyFireOncePerRespawn;

	private bool m_hasFired = false;
	public bool hasFiredThisRespawn = false;
	public bool neverTriggerAgain = false;


	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			if (neverTriggerAgain)
				return;

			if (onlyFireOnce && m_hasFired)
				return;

			if (onlyFireOncePerRespawn && hasFiredThisRespawn)
				return;

			if (!m_hasFired)
				m_hasFired = true;

			if (!hasFiredThisRespawn)
				hasFiredThisRespawn = true;

			foreach (Gate gate in gatesToOpen)
			{
				gate.GateOpen();
			}

			foreach (Gate gate in gatesToClose)
			{
				gate.GateClose();
			}
		}
	}
}
