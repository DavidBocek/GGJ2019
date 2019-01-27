using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MEC;

public class SwipeTrail : MonoBehaviour
{
	public float swipeAngle;
	public float swipeDuration;

	private Vector3 m_initialEulers;
	private float m_leftYAngle;
	private float m_rightYAngle;
	private bool m_canSwipe = true;
	private TrailRenderer m_trailRenderer;

	private const float BUFFER_TIME = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
		m_initialEulers = transform.localEulerAngles;
		m_rightYAngle = transform.localEulerAngles.y;
		m_leftYAngle = transform.localEulerAngles.y - swipeAngle + 360f;
		m_trailRenderer = GetComponentInChildren<TrailRenderer>();
		m_trailRenderer.enabled = false;
	}

    // Update is called once per frame
    void Update()
    {
		
    }

	public void Swipe(bool goLeft)
	{
		if (m_canSwipe)
		{
			Timing.RunCoroutine(_Swipe(goLeft));
		}	
	}

	private IEnumerator<float> _Swipe(bool goLeft)
	{
		m_canSwipe = false;

		m_trailRenderer.enabled = true;

		float startY = goLeft ? m_rightYAngle : m_leftYAngle;
		float endY = goLeft ? m_leftYAngle : m_rightYAngle;

		transform.localEulerAngles = new Vector3(m_initialEulers.x, startY, m_initialEulers.z);
		transform.DOLocalRotate(new Vector3(m_initialEulers.x, endY, m_initialEulers.z), swipeDuration, RotateMode.Fast).SetEase(Ease.InQuad);

		yield return Timing.WaitForSeconds(swipeDuration + BUFFER_TIME);

		m_trailRenderer.enabled = false;

		m_canSwipe = true;
	}
}
