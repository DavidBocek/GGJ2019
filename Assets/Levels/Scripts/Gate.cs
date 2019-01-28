using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MEC;

public class Gate : MonoBehaviour
{
	public float yChange;
	public float moveTime;
	private bool m_isGateOpen = true;

	private void Start()
	{
		Vector3 originPos = transform.position;
		originPos.y -= yChange;
		transform.position = originPos;
	}

	private void Update()
	{
		/*if (Input.GetKeyDown(KeyCode.O))
		{
			if (m_isGateOpen)
				GateClose();
			else
				GateOpen();
		}*/
	}

	public void GateClose()
	{
		if (!m_isGateOpen)
			return;
        
        gameObject.GetComponent<RandomAudioPlayer>().PlayRandomSound( "gateMove", false );

		m_isGateOpen = false;
		transform.DOLocalMoveY(yChange, moveTime).SetRelative(true).SetEase(Ease.OutBounce);
	}

	public void GateOpen()
	{
		if (m_isGateOpen)
			return;
        
        gameObject.GetComponent<RandomAudioPlayer>().PlayRandomSound( "gateMove", false );

		m_isGateOpen = true;
		transform.DOLocalMoveY(-yChange, moveTime).SetRelative(true).SetEase(Ease.InQuart);
	}
}
