using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public bool devMode;
    public bool infinite;

    public GameObject monsterInfoUI;
    public ParticleSystem spawnEffect;
    public GameObject enemySpawner;
    public Item[] item;

    MapGenerator map;
    int enemyLeft;

    public int itemsRemainingToSpawn = 5;
    public float timeBetweenSpawnMin = 1f;
    public float timeBetweenSpawnMax = 10f;
    public int itemsRemaining;
    float nextSpawnTime;
    public int maxItems;

    private void Start()
    {        
        map = FindObjectOfType<MapGenerator>();
        
        enemyLeft = enemySpawner.GetComponent<EnemySpawner>().enemiesRemainingAlive;
    }

    void Update()
    {
        itemsRemaining = FindObjectsOfType<Item>().Length;

        //Debug.Log("Camping: "+isCamping);
        if ((itemsRemainingToSpawn > 0 || infinite) && Time.time > nextSpawnTime && monsterInfoUI.gameObject.activeSelf != true && itemsRemaining < 3)
        {
            itemsRemainingToSpawn--;
            nextSpawnTime = Time.time + Random.Range(timeBetweenSpawnMin,timeBetweenSpawnMax);
            //Debug.Log(nextSpawnTime);

            StartCoroutine("SpawnItem");
        }
        
    }

    IEnumerator SpawnItem()
    {
        float spawnDelay = 2f;

        Transform spawnTile = map.GetRandomOpenTile();
        
        float spawnTimer = 0;

        while (spawnTimer < spawnDelay)
        {
            ParticleSystem spawnFx = spawnEffect.GetComponent<ParticleSystem>();
            ParticleSystem.MainModule spawnFxMain = spawnFx.main;

            spawnFxMain.startColor = new Color(0, 255, 0, 1);
            if(spawnTile != null) Destroy(Instantiate(spawnEffect.gameObject, spawnTile.transform), .5f);

            spawnTimer += .5f + Time.deltaTime;
            yield return null;
        }
        
        if(spawnTile != null)
        {
            AudioManager.instance.PlaySound("Item Spawn", spawnTile.position);
            Item spawnedItem = Instantiate(item[Random.Range(0, item.Length)], spawnTile.position, Quaternion.identity) as Item;
        }

    }
}
