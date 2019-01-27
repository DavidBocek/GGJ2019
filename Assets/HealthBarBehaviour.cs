using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarBehaviour : MonoBehaviour
{
	public Image fill;

	private Slider m_slider;
	private HealthController health;
    // Start is called before the first frame update
    void Start()
    {
		m_slider = GetComponent<Slider>();
		health = GameObject.FindWithTag( "Player" ).GetComponent<HealthController>();
		m_slider.maxValue = health.maxHealth;
		m_slider.value = health.currHealth;
		m_slider.minValue = 0f;
    }

    // Update is called once per frame
    void Update()
    {
		if ( health.currHealth >= health.maxHealth*0.5f )
		{
			fill.color = Color.green;
		}
		else if ( health.currHealth >= health.maxHealth*0.25f )
		{
			fill.color = Color.yellow;
		}
		else
		{
			fill.color = Color.red;
		}
		m_slider.value = health.currHealth;
    }
}
