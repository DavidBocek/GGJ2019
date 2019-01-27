using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Assertions;

public class PickupController : MonoBehaviour
{
	private SecretTextBehaviour m_secretFoundAnnounce;
    private int healAmount = 10;
    private float pickupDelay = 1.0f;
    private float pickupActivateTime = 0.0f;
    private float bounceUpScaleVal = 1.4f;
    private float nothingToBounceUpTime = 0.2f;
    private float bounceUpToNormalTime = 0.2f;

    public enum PickupType
    {
        eHealth,
        eCount
    }

    public PickupType pickupType = PickupType.eCount;

    private void SetTriggerEnabledState( bool enabledState )
    {
        SphereCollider pickupTrigger = GetComponentInChildren<SphereCollider>();
        Assert.AreEqual( pickupTrigger.isTrigger, true );
        pickupTrigger.enabled = enabledState;
    }

    public void DoSpawnAnimation( Vector3 velocity )
    {
        gameObject.transform.localScale = new Vector3( 0.0f, 0.0f, 0.0f );

        gameObject.GetComponent<Rigidbody>().velocity = velocity;
        
        Transform transform = gameObject.transform;
        
        Sequence spawnAnimSequence = DOTween.Sequence();
        spawnAnimSequence.Append( transform.DOScale( bounceUpScaleVal, nothingToBounceUpTime ).SetEase( Ease.OutQuad ) );
        spawnAnimSequence.Append( transform.DOScale( 1.0f, bounceUpToNormalTime ).SetEase( Ease.OutQuad ) );

        SetTriggerEnabledState( false );
        pickupActivateTime = Time.time + pickupDelay;
    }

    // Start is called before the first frame update
    void Start()
    {
        Assert.AreNotEqual( pickupType, PickupType.eCount );
		m_secretFoundAnnounce = GameObject.FindWithTag( "SecretText" ).GetComponent<SecretTextBehaviour>();
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
    
    void OnPickupAnimComplete()
    {
        Destroy( gameObject );
    }

    void OnTriggerEnter( Collider collider )
    {
        if ( collider.gameObject.tag == "Player" )
        {
			//collider.gameObject.GetComponent<HealthController>().HealthController_Heal( healAmount );

			m_secretFoundAnnounce.Announce();

            Sequence pickupAnimSequence = DOTween.Sequence();
            pickupAnimSequence.Append( transform.DOScale( bounceUpScaleVal, bounceUpToNormalTime ).SetEase( Ease.InQuad ) );
            pickupAnimSequence.Append( transform.DOScale( 0.0f, nothingToBounceUpTime ).SetEase( Ease.OutQuad ) );
            pickupAnimSequence.OnComplete( OnPickupAnimComplete );
        }
    }
}
