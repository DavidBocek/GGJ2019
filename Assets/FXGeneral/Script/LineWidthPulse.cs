using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineWidthPulse : MonoBehaviour
{
	public float minWidth;
	public float maxWidth;
	public float frequency;

	private float m_randomOffset;
	private LineRenderer m_line;

    // Start is called before the first frame update
    void Start()
    {
		m_randomOffset = Random.Range(0f, 10f);
		m_line = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
		float width = (maxWidth - minWidth) * Mathf.Sin(frequency * Time.time + m_randomOffset) + ((maxWidth - minWidth) / 2f + minWidth);
        m_line.startWidth = width;
		m_line.endWidth = width;
    }
}
