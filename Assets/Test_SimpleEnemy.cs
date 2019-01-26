using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Test_SimpleEnemy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Assert.AreNotEqual( gameObject.GetComponent<HealthController>(), null );
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDamage( int damage )
    {
        print( "Took damage: " + damage.ToString() );
    }

    void OnDeath( int damage )
    {
        print( "Dead!" );
        gameObject.GetComponent<HealthController>().HealthController_HealToFull();
    }
}
