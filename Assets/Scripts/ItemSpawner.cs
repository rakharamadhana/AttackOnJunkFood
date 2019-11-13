using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public bool devMode;
    public bool infinite;

    public GameObject monsterInfoUI;

    int currentWaveNumber = 0;

    public GameObject enemySpawner;
    public Item[] item;

    MapGenerator map;
    int enemyLeft;

    public int itemsRemainingToSpawn = 5;
    public float timeBetweenSpawn = 1f;
    public int itemsRemainingAlive;
    float nextSpawnTime;
    int maxItems;

    bool isDisabled;

    public event System.Action<int> OnNewWave;

    private void Start()
    {        
        map = FindObjectOfType<MapGenerator>();
        
        maxItems = itemsRemainingToSpawn;
        enemyLeft = enemySpawner.GetComponent<EnemySpawner>().enemiesRemainingAlive;
    }

    void Update()
    {
        //Debug.Log("Camping: "+isCamping);
        if (!isDisabled)
        {
            if ((itemsRemainingToSpawn > 0 || infinite) && Time.time > nextSpawnTime && monsterInfoUI.gameObject.activeSelf != true)
            {
                itemsRemainingToSpawn--;
                nextSpawnTime = Time.time + timeBetweenSpawn;

                StartCoroutine("SpawnItem");
            }
        }
    }

    IEnumerator SpawnItem()
    {
        float spawnDelay = 2f;

        Transform spawnTile = map.GetRandomOpenTile();
        
        float spawnTimer = 0;

        while (spawnTimer < spawnDelay)
        {
            spawnTimer += Time.deltaTime;
            yield return null;
        }
        
        if(spawnTile != null)
        {
            Item spawnedItem = Instantiate(item[Random.Range(0, item.Length)], spawnTile.position + Vector3.up, Quaternion.identity) as Item;
        }

    }
}
