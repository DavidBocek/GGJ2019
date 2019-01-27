using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MEC;

public class SecretTextBehaviour : MonoBehaviour
{
	public string maxSecrets;

	private Text m_textWidget;
	private int m_secretCount = 0;

	private const string SECRET_FOUND = "Secret Found!\n";
    // Start is called before the first frame update
    void Start()
    {
		m_textWidget = GetComponent<Text>();
		Color tempColor = m_textWidget.color;
		tempColor.a = 0.0f;

		m_textWidget.color = tempColor;
	}

	// Update is called once per frame
	void Update()
	{
    }

	public void Announce()
	{
		Timing.RunCoroutineSingleton( AnnounceThread(), gameObject, SingletonBehavior.Abort );
	}

	private IEnumerator<float> AnnounceThread()
	{
		m_secretCount++;
		m_textWidget.text = SECRET_FOUND + m_secretCount.ToString() + " / " + maxSecrets;

		Color tempColor = m_textWidget.color;
		tempColor.a = 1.0f;
		m_textWidget.color = tempColor;

		yield return Timing.WaitForSeconds( 3f );

		for ( float t=1; t>0; t-= Timing.DeltaTime )
		{
			tempColor.a = t;
			m_textWidget.color = tempColor;
			yield return Timing.WaitForOneFrame;
		}

		tempColor.a = 0f;
		m_textWidget.color = tempColor;
	}
}
