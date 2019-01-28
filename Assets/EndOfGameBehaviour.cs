using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MEC;

public class EndOfGameBehaviour : MonoBehaviour
{
	public Image curtain;
	public Image victoryImage;
	public SecretTextBehaviour secretText;
	public Text textWidget;
    // Start is called before the first frame update
    void Start()
    {
		Color tempColor = curtain.color;
		tempColor.a = 0f;
		curtain.color = tempColor;

		tempColor = textWidget.color;
		tempColor.a = 0f;
		textWidget.color = tempColor;

		tempColor = victoryImage.color;
		tempColor.a = 0f;
		victoryImage.color = tempColor;
	}

    // Update is called once per frame
    void Update()
    {
		
    }

	private void OnTriggerEnter( Collider other )
	{
		if ( other.gameObject.CompareTag( "Player" ) )
		{
			Timing.RunCoroutineSingleton( EndingSequence(), gameObject, SingletonBehavior.Abort );
		}
	}

	private IEnumerator<float> EndingSequence()
	{
		Color tempColor = curtain.color;
		bool youDidIt = false;

		for ( float t = 0; t < 1; t += Timing.DeltaTime/1.5f )
		{
			tempColor.a = t;
			curtain.color = tempColor;
			yield return Timing.WaitForOneFrame;
		}

		if ( (secretText.secretCount).ToString().Equals( secretText.maxSecrets ) )
		{
			textWidget.text = "You found " + secretText.secretCount.ToString() + " / " + secretText.maxSecrets + " secrets! \nWay to go!";
			youDidIt = true;
		}
		else
		{
			textWidget.text = "You found " + secretText.secretCount.ToString() + " / " + secretText.maxSecrets + " secrets! \nTry again?";
		}

		yield return Timing.WaitForSeconds( 0.5f );

		tempColor = textWidget.color;
		tempColor.a = 1.0f;
		textWidget.color = tempColor;

		if ( youDidIt )
		{
			yield return Timing.WaitForSeconds( 3.0f );

			tempColor = victoryImage.color;
			tempColor.a = 1.0f;
			victoryImage.color = tempColor;
		}


		while( true )
		{
			if ( Input.GetButtonDown( "Submit" ) )
			{
				SceneManager.LoadScene( "MainMenu" );
			}
			yield return Timing.WaitForOneFrame;
		}

		
	}
}
