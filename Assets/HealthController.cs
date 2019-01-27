using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

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

    private int currHealth;

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
            gameObject.SendMessage( "OnDeath", damage, SendMessageOptions.DontRequireReceiver );
            return HealthControllerDamageResult.eDead;
        }

        gameObject.SendMessage( "OnDamage", damage, SendMessageOptions.DontRequireReceiver );
        return HealthControllerDamageResult.eDamaged;
    }

    public void HealthController_Heal( int healAmount )
    {
        currHealth = Mathf.Min( maxHealth, currHealth + healAmount );
    }
}
