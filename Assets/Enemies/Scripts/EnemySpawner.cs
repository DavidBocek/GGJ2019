using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Assertions;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnDelay = 1.0f;
    public int maxEnemyCountOnField = 3;
    
    private float nextSpawnTime = 0.0f;
    private int enemyCountOnField = 0;

    public void SpawnEnemy()
    {
        nextSpawnTime = 0.0f;

        GameObject enemy = Instantiate( enemyPrefab, transform.position, transform.rotation );
        enemy.transform.SetParent( gameObject.transform );
        
        enemy.SendMessage( "OnSpawnFromSpawner", SendMessageOptions.DontRequireReceiver );

        ++enemyCountOnField;
        if ( enemyCountOnField < maxEnemyCountOnField )
            nextSpawnTime = Time.time + spawnDelay;
    }

    // Start is called before the first frame update
    void Start()
    {
        SpawnEnemy();
    }

    // Update is called once per frame
    void Update()
    {
        if ( nextSpawnTime != 0.0f && Time.time >= nextSpawnTime )
        {
            SpawnEnemy();
        }
    }

    void OnDeath( int damage )
    {
        --enemyCountOnField;
        nextSpawnTime = Time.time + spawnDelay;
    }
}
