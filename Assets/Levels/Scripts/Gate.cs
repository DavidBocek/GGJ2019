using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Gate : MonoBehaviour
{
	public float yChange;
	public float moveTime;
	private bool m_isGateOpen = true;
    
	public void GateClose()
	{
		if (!m_isGateOpen)
			return;

		m_isGateOpen = false;
		transform.DOLocalMoveY(yChange, moveTime).SetRelative(true).SetEase(Ease.InQuad);
	}

	public void GateOpen()
	{
		if (m_isGateOpen)
			return;

		m_isGateOpen = true;
		transform.DOLocalMoveY(-yChange, moveTime).SetRelative(true).SetEase(Ease.OutQuad);
	}
}
