using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Test_SimpleEnemy : MonoBehaviour
{
    public GameObject healthPickup;

    // Start is called before the first frame update
    void Start()
    {
        Assert.AreNotEqual( gameObject.GetComponent<HealthController>(), null );
        Assert.AreNotEqual( healthPickup, null );
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
        GameObject pickup = Instantiate( healthPickup, transform.position + new Vector3( 0.0f, 0.5f, 0.0f ), transform.rotation );

        GameObject playerObject = GameObject.FindGameObjectWithTag( "Player" );
        Vector3 playerToSelf = transform.position - playerObject.transform.position;
        playerToSelf.y = 0.0f;
        playerToSelf.Normalize();

        Vector3 pickupVel = playerToSelf * 2.0f;
        pickupVel.y = 2.0f;
        pickup.GetComponent<PickupController>().DoSpawnAnimation( pickupVel );
    }
}
