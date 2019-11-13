using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    public bool devMode;
    public bool infinite;

    public GameObject monsterInfoUI;
    public ParticleSystem spawnEffect;
    public GameObject enemySpawner;
    public Box[] box;

    MapGenerator map;
    int enemyLeft;

    public int boxesRemainingToSpawn = 5;
    public float timeBetweenSpawnMin = 1f;
    public float timeBetweenSpawnMax = 10f;
    public int boxesRemaining;
    float nextSpawnTime;
    int maxBoxes;

    private void Start()
    {
        map = FindObjectOfType<MapGenerator>();

        maxBoxes = boxesRemainingToSpawn;
        enemyLeft = enemySpawner.GetComponent<EnemySpawner>().enemiesRemainingAlive;
    }

    void Update()
    {
        boxesRemaining = FindObjectsOfType<Box>().Length;

        //Debug.Log("Camping: "+isCamping);
        if ((boxesRemainingToSpawn > 0 || infinite) && Time.time > nextSpawnTime && monsterInfoUI.gameObject.activeSelf != true && boxesRemaining < 3)
        {
            boxesRemainingToSpawn--;
            nextSpawnTime = Time.time + Random.Range(timeBetweenSpawnMin, timeBetweenSpawnMax);
            //Debug.Log(nextSpawnTime);

            StartCoroutine("SpawnBox");
        }

    }

    IEnumerator SpawnBox()
    {
        float spawnDelay = 2f;

        Transform spawnTile = map.GetRandomOpenTile();

        float spawnTimer = 0;

        while (spawnTimer < spawnDelay)
        {
            ParticleSystem spawnFx = spawnEffect.GetComponent<ParticleSystem>();
            ParticleSystem.MainModule spawnFxMain = spawnFx.main;

            spawnFxMain.startColor = new Color(0, 0, 255, 1);
            if (spawnTile != null) Destroy(Instantiate(spawnEffect.gameObject, spawnTile.transform), .5f);

            spawnTimer += 1 + Time.deltaTime;
            yield return null;
        }

        if (spawnTile != null)
        {
            AudioManager.instance.PlaySound("Item Spawn", spawnTile.position);
            Box spawnedBox = Instantiate(box[Random.Range(0, box.Length)], spawnTile.position, Quaternion.identity) as Box;
        }

    }
}
