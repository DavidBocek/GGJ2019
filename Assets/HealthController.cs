﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using DG.Tweening;
using MEC;

public class HealthController : MonoBehaviour
{
    public enum HealthControllerDamageResult
    {
        eNoDamage,
        eDamaged,
        eDead,
        eCount
    }

    public int maxHealth;

	public Renderer flashRenderer;
	public float flashDuration = 0.15f;

    public int currHealth;

    // Start is called before the first frame update
    void Start()
    {
        Assert.AreNotEqual( maxHealth, 0 );
        currHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if ( gameObject.CompareTag( "Player" ) )
		{
			Debug.Log( currHealth );
		}
    }
    
    public void HealthController_HealToFull()
    {
        currHealth = maxHealth;
    }
    
    public HealthControllerDamageResult HealthController_TakeDamage( int damage )
    {
        currHealth -= damage;

        if ( currHealth <= 0 )
        {
            if ( flashRenderer != null )
			    flashRenderer.material.SetFloat("_FlashAmount", 0f);

            gameObject.SendMessageUpwards( "OnDeath", damage, SendMessageOptions.DontRequireReceiver ); // send upwards for spawners
            return HealthControllerDamageResult.eDead;
        }
        
        if ( flashRenderer != null )
        {
            flashRenderer.material.SetFloat("_FlashAmount", 0f);
		    flashRenderer.material.DOFloat(1f, "_FlashAmount", flashDuration / 2f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InQuad);
			Timing.RunCoroutineSingleton(HackGoToZeroFlash(), "hack", SingletonBehavior.Overwrite);
        }
		
        gameObject.SendMessageUpwards( "OnDamage", damage, SendMessageOptions.DontRequireReceiver );
        return HealthControllerDamageResult.eDamaged;
    }

	private IEnumerator<float> HackGoToZeroFlash()
	{
		yield return Timing.WaitForSeconds(0.15f);
		flashRenderer.material.SetFloat("_FlashAmount", 0f);
	}

    public void HealthController_Heal( int healAmount )
    {
        currHealth = Mathf.Min( maxHealth, currHealth + healAmount );
    }

    public bool HealthController_CanHeal()
    {
        return currHealth < maxHealth;
    }
}
