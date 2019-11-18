using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    public bool devMode;

    public Wave[] waves;

    public ParticleSystem spawnEffect;
    public GameObject monsterInfoUI;
    public GameObject itemSpawner;
    public GameObject winningUI;
    public Text winningTextUI;
    public Text winningScoreTextUI;
    public GameObject winningOriginalButton;
    public GameObject winningLastButton;

    LivingEntity playerEntity;
    Transform playerT;

    Wave currentWave;
    public int currentWaveNumber { get; private set; }

    int enemiesRemainingToSpawn;
    int maxItemToSpawn;
    public int enemiesRemainingAlive;
    float nextSpawnTime;

    MapGenerator map;

    float timeBetweenCampingChecks = 2;
    float campThresholdDistance = 1.5f;
    float nextCampCheckTime;
    Vector3 campPositionOld;
    bool isCamping;

    public bool isDisabled;

    public event System.Action<int> OnNewWave;

    private void Start()
    {
        playerEntity = FindObjectOfType<Player>();
        playerT = playerEntity.transform;

        nextCampCheckTime = timeBetweenCampingChecks + Time.time;
        campPositionOld = playerT.position;
        playerEntity.OnDeath += OnPlayerDeath;

        map = FindObjectOfType<MapGenerator>();
        maxItemToSpawn = itemSpawner.GetComponent<ItemSpawner>().itemsRemainingToSpawn;

        NextWave();
    }

    void Update()
    {
        //Debug.Log("Camping: "+isCamping);
        if(!isDisabled)
        {

            if (Time.time > nextCampCheckTime)
            {
                nextCampCheckTime = Time.time + timeBetweenCampingChecks;

                isCamping = (Vector3.Distance(playerT.position, campPositionOld) < campThresholdDistance);
                campPositionOld = playerT.position;
            }

            if ((enemiesRemainingToSpawn > 0 || currentWave.infinite) && Time.time > nextSpawnTime && monsterInfoUI.gameObject.activeSelf != true)
            {
                enemiesRemainingToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawn;

                StartCoroutine("SpawnEnemy");
            }
        }
        
        if(devMode)
        {
            if(Input.GetKeyDown(KeyCode.Return))
            {
                StopCoroutine("SpawnEnemy");
                foreach(Enemy enemy in FindObjectsOfType<Enemy>())
                {
                    GameObject.Destroy(enemy.gameObject);
                }
                playerEntity.GetComponent<GunController>().EquipGun(0);
                NextWave();
            }
        }

        if(enemiesRemainingAlive == 0)
        {
            winningScoreTextUI.GetComponent<Text>().text = ScoreKeeper.score.ToString("D6");
        }
    }

    IEnumerator SpawnEnemy()
    {
        float spawnDelay = 1f;

        Transform spawnTile = map.GetRandomOpenTile();
        if(isCamping)
        {
            spawnTile = map.GetTileFromPosition(playerT.position);
        }
        Material tileMat = spawnTile.GetComponent<Renderer>().material;
        Color initialColour = new Color(.1415f, .1415f, .1415f);
        Color flashColour = Color.red;
        float spawnTimer = 0;

        while (spawnTimer < spawnDelay)
        {
            ParticleSystem spawnFx = spawnEffect.GetComponent<ParticleSystem>();
            ParticleSystem.MainModule spawnFxMain = spawnFx.main;

            spawnFxMain.startColor = new Color(255,0,0,1);
            Destroy(Instantiate(spawnEffect.gameObject, spawnTile.transform), .5f);

            spawnTimer += Time.deltaTime;
            yield return null;
        }

        AudioManager.instance.PlaySound("Enemy Spawn", spawnTile.position);

        Enemy spawnedEnemy = Instantiate(currentWave.enemy[Random.Range(0, currentWave.enemy.Length)], spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
        spawnedEnemy.OnDeath += OnEnemyDeath;
        //spawnedEnemy.SetCharacteristics(currentWave.moveSpeed, currentWave.hitsToKillPlayer, currentWave.enemyHealth, currentWave.skinColour);
    }

    void OnPlayerDeath()
    {
        isDisabled = true;
    }

    void OnEnemyDeath()
    {
        string[] numbers = { "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Bonus" };

        if (!currentWave.infinite)
        {
            enemiesRemainingAlive--;
        }
        
        if(enemiesRemainingAlive == 0 && !currentWave.infinite)
        {
            winningTextUI.GetComponent<Text>().text = "Level " + numbers[currentWaveNumber - 1] + " Completed";
            if(currentWaveNumber == 11)
            {
                winningTextUI.GetComponent<Text>().text = "Game Completed";
                winningOriginalButton.SetActive(false);
                winningLastButton.SetActive(true);
            }
            Cursor.visible = true;
            AudioManager.instance.PlaySound("Level Completed", playerEntity.transform.position);
            winningUI.SetActive(true);
            //NextWave();
        }
    }

    void ResetPlayerPosition()
    {
        playerT.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 2;
    }

    public void NextWave()
    {
        //Time.timeScale = 1f;
        Cursor.visible = false;

        currentWaveNumber ++;
        //print("Wave: " + currentWaveNumber);
        if(currentWaveNumber-1 < waves.Length)
        {
            currentWave = waves[currentWaveNumber - 1];

            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRemainingAlive = enemiesRemainingToSpawn;
            playerEntity.GetComponent<GunController>().EquipGun(0);
            RestockItem();
            CleanBox();
            if(OnNewWave != null)
            {
                OnNewWave(currentWaveNumber);
            }
        }
        winningUI.SetActive(false);
        ResetPlayerPosition();
    }

    void RestockItem()
    {
        foreach (Item item in FindObjectsOfType<Item>())
        {
            GameObject.Destroy(item.gameObject);
        }
        itemSpawner.GetComponent<ItemSpawner>().itemsRemainingToSpawn = maxItemToSpawn;
    }

    void CleanBox()
    {
        foreach (Box box in FindObjectsOfType<Box>())
        {
            GameObject.Destroy(box.gameObject);
        }
    }

    [System.Serializable]
    public class Wave
    {
        public bool infinite;
        public int enemyCount;
        public float timeBetweenSpawn;

        public Enemy[] enemy;
    }

}
