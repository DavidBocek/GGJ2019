using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SpawnPointController : MonoBehaviour
{
    public bool isLevelStart = false;

	/*private Light m_light;

    // Start is called before the first frame update
    void Start()
    {
		m_light = GetComponentInChildren<Light>();
		Assert.IsTrue(m_light != null);
		m_light.enabled = false;
    }*/

	public void Activate()
	{
		//m_light.enabled = true;
	}

	public void Deactivate()
	{
		//m_light.enabled = false;
	}
}
