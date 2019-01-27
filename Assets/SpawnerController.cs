using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SpawnerController : MonoBehaviour
{
    public PickupController.PickupType pickupType = PickupController.PickupType.eCount;
    public GameObject pickupPrefab;
    public float spawnDelay = 1.0f;

    private GameObject pickup;
    private float nextSpawnTime = 0.0f;

    public void SpawnPickup()
    {
        pickup = Instantiate( pickupPrefab, transform.position, transform.rotation );
        pickup.GetComponent<PickupController>().pickupType = pickupType;
    }

    // Start is called before the first frame update
    void Start()
    {
        Assert.AreNotEqual( pickupType, PickupController.PickupType.eCount );
        SpawnPickup();
    }

    // Update is called once per frame
    void Update()
    {
        if ( nextSpawnTime == 0.0f && pickup == null )
        {
            nextSpawnTime = Time.time + spawnDelay;
        }

        if ( nextSpawnTime != 0.0f && Time.time >= nextSpawnTime )
        {
            SpawnPickup();
            nextSpawnTime = 0.0f;
        }
    }
}
