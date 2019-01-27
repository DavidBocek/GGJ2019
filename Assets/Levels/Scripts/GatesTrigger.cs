using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatesTrigger : MonoBehaviour
{
	public Gate[] gatesToOpen;
	public Gate[] gatesToClose;
	public bool onlyFireOnce;

	private bool m_hasFired = false;


	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			if (onlyFireOnce && m_hasFired)
				return;

			if (!m_hasFired)
				m_hasFired = true;

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
