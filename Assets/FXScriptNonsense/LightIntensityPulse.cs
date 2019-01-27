using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightIntensityPulse : MonoBehaviour
{
	public float intensityMin;
	public float intensityMax;
	public float frequency;
	public float randomAmplitude;
	public float randomScrollSpeed;

	private float verticalOffset;
	private float randomPhase;
	private Light m_light;

    // Start is called before the first frame update
    void Start()
    {
		m_light = GetComponent<Light>();
		randomPhase = Random.Range(0f, 10f);
		verticalOffset = ((intensityMax - intensityMin) / 2f + intensityMin);

	}

    // Update is called once per frame
    void Update()
    {
		m_light.intensity = (intensityMax - intensityMin) * Mathf.Sin(frequency*Time.time + randomPhase) + verticalOffset + randomAmplitude * Mathf.PerlinNoise(Time.time * randomScrollSpeed, 0f);
	}
}
