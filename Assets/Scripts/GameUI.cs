using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("General UI")]
    public Image fadePlane;
    public GameObject gameOverUI;
    public GameObject pauseMenuUI;

    public RectTransform newWaveBanner;
    public Text newWaveTitle;
    public Text newWaveEnemyCount;
    public Text scoreUI;
    public Text comboUI;
    public Text enemyCountUI;
    public Text gameOverScoreUI;
    public GameObject dashButton;
    public GameObject pauseButton;
    public GameObject leftJoystick;
    public GameObject rightJoystick;
    public RectTransform healthBar;
    public RectTransform staminaBar;
    public AudioClip bossTheme;

    [Header("Monster Info UI")]
    public GameObject monsterInfoUI;
    public Text monsterName;
    public Image monsterImage;
    public Text monsterDescription;
    public Image monsterCalorieBar;
    public Image monsterSaltBar;
    public Image monsterDiseaseBar;
    public Monster[] monstersInfo;

    EnemySpawner spawner;
    Player player;
    Monster currentMonster;

    public static bool gameIsPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        player.OnDeath += OnGameOver;
    }

    private void Awake()
    {
        spawner = FindObjectOfType<EnemySpawner>();
        spawner.OnNewWave += onNewWave;
    }

    private void Update()
    {
        
        scoreUI.text = ScoreKeeper.score.ToString("D6");
        comboUI.text = ScoreKeeper.streakCount.ToString("D2");

        int enemyCount = spawner.enemiesRemainingAlive;

        enemyCountUI.text = enemyCount.ToString("D2");

        float healthPercent = 0;
        float staminaPercent = 0;
        if (player != null)
        {
            healthPercent = player.health / player.startingHealth;
            staminaPercent = player.dashLimit / player.maxDashLimit;
        }
        healthBar.localScale = new Vector3(healthPercent, 1, 1);
        staminaBar.localScale = new Vector3(staminaPercent, 1, 1);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            if (gameIsPaused)
            {
                ResumeGame();
            }
            else
            {
                AudioManager.instance.PlaySound("Pause", player.transform.position);
                PauseGame();
            }
        }
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
        Cursor.visible = false;
    }

    public void PauseGame()
    {
        Cursor.visible = true;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    void onNewWave(int waveNumber)
    {
        //Debug.Log(waveNumber);
        string[] numbers = { "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Bonus"};
        newWaveTitle.text = "- Level " + numbers[waveNumber - 1] + " -";
        if (waveNumber == 11)
        {
            newWaveTitle.text = "- Final Boss -";
            AudioManager.instance.PlayMusic(bossTheme, 5);
        }
        string enemyCountString = ((spawner.waves[waveNumber-1].infinite)?"Infinite":spawner.waves[waveNumber-1].enemyCount +"");
        newWaveEnemyCount.text = "Enemies: " + enemyCountString;

        updateMonsterInfo(waveNumber - 1);
        monsterInfoUI.SetActive(true);
        Cursor.visible = true;
    }

    void updateMonsterInfo(int currentWave)
    {
        int maxRate = 5;

        currentMonster = monstersInfo[currentWave];
        monsterName.text = currentMonster.monsterName;
        monsterImage.gameObject.GetComponent<Image>().sprite = currentMonster.monsterImage;
        monsterDescription.gameObject.GetComponent<Text>().text = currentMonster.monsterDescription;
        monsterCalorieBar.gameObject.GetComponent<Image>().fillAmount = currentMonster.monsterCalorieRate/maxRate;
        monsterSaltBar.gameObject.GetComponent<Image>().fillAmount = currentMonster.monsterSaltRate/maxRate;
        monsterDiseaseBar.gameObject.GetComponent<Image>().fillAmount = currentMonster.monsterDiseaseRate/maxRate;

    }

    void OnGameOver()
    {
        Cursor.visible = true;
        StartCoroutine(Fade(Color.clear, new Color(0,0,0,.95f), 1));
        gameOverScoreUI.text = scoreUI.text;
        pauseButton.gameObject.SetActive(false);
        dashButton.gameObject.SetActive(false);
        leftJoystick.gameObject.SetActive(false);
        rightJoystick.gameObject.SetActive(false);
        scoreUI.gameObject.SetActive(false);
        gameOverUI.SetActive(true);
    }

    IEnumerator AnimateNewWaveBanner()
    {
        float delayTime = 1.5f;
        float speed = 3f;
        float animatePercent = 0;
        int dir = 1;

        float endDelayTime = Time.time + 1 / speed + delayTime;

        while(animatePercent >= 0)
        {
            animatePercent += Time.deltaTime * speed * dir;

            if(animatePercent >= 1)
            {
                animatePercent = 1;
                if(Time.time > endDelayTime)
                {
                    dir = -1;
                }
            }

            newWaveBanner.anchoredPosition = Vector2.down * Mathf.Lerp(150, 375, animatePercent);

            yield return null;
        }

    }

    IEnumerator Fade(Color from, Color to, float time)
    {
        float speed = 1 / time;
        float percent = 0;

        while(percent < 1)
        {
            percent += Time.deltaTime * speed;
            fadePlane.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }

    // UI Input
    public void StartNewGame()
    {
        if (Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
        }
        SceneManager.LoadScene("LoadingScreen");
    }

    public void ReturnToMainMenu()
    {
        if (Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
        }

        SceneManager.LoadScene("Menu");
    }

    public void MonsterInfoContinue()
    {
        Cursor.visible = false;
        monsterInfoUI.gameObject.SetActive(false);
        StopCoroutine("AnimateNewWaveBanner");
        StartCoroutine("AnimateNewWaveBanner");
    }

    [System.Serializable]
    public class Monster
    {
        public string monsterName;
        public Sprite monsterImage;

        [Range(0,5)]
        public float monsterCalorieRate;
        [Range(0, 5)]
        public float monsterSaltRate;
        [Range(0, 5)]
        public float monsterDiseaseRate;

        [TextArea]
        public string monsterDescription;
    }
}
