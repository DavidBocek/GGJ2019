using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Assertions;

public class PickupController : MonoBehaviour
{
    private int healAmount = 10;
    private float pickupDelay = 1.0f;
    private float pickupActivateTime = 0.0f;

    public enum PickupType
    {
        eHealth,
        eCount
    }

    public PickupType pickupType = PickupType.eCount;

    private void SetTriggerEnabledState( bool enabledState )
    {
        SphereCollider pickupTrigger = GetComponent<SphereCollider>();
        Assert.AreEqual( pickupTrigger.isTrigger, true );
        pickupTrigger.enabled = enabledState;
    }

    public void DoSpawnAnimation( Vector3 velocity )
    {
        gameObject.transform.localScale = new Vector3( 0.0f, 0.0f, 0.0f );

        gameObject.GetComponent<Rigidbody>().velocity = velocity;
        
        Transform transform = gameObject.transform;
        transform.DOScale( 1.0f, 1.0f ).SetEase( Ease.OutQuad );

        SetTriggerEnabledState( false );
        pickupActivateTime = Time.time + pickupDelay;
    }

    // Start is called before the first frame update
    void Start()
    {
        Assert.AreNotEqual( pickupType, PickupType.eCount );
    }

    // Update is called once per frame
    void Update()
    {
        if ( pickupActivateTime != 0.0f && Time.time >= pickupActivateTime )
        {
            SetTriggerEnabledState( true );
            pickupActivateTime = 0.0f;
        }
    }
    
    void OnTriggerEnter( Collider collider )
    {
        if ( collider.gameObject.tag == "Player" )
        {
            collider.gameObject.GetComponent<HealthController>().HealthController_Heal( healAmount );
            Destroy( gameObject );
        }
    }
}
