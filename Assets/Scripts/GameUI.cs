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
    
    public RectTransform newWaveBanner;
    public Text newWaveTitle;
    public Text newWaveEnemyCount;
    public Text scoreUI;
    public Text comboUI;
    public Text enemyCountUI;
    public Text gameOverScoreUI;
    public GameObject dashButton;
    public GameObject leftJoystick;
    public GameObject rightJoystick;
    public RectTransform healthBar;
    public RectTransform staminaBar;

    [Header("Monster Info UI")]
    public GameObject monsterInfoUI;
    public Text monsterName;
    public Image monsterImage;
    public Text monsterDescription;
    public Monster[] monstersInfo;

    Spawner spawner;
    Player player;
    Monster currentMonster;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        player.OnDeath += OnGameOver;
    }

    private void Awake()
    {
        spawner = FindObjectOfType<Spawner>();
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
    }

    void onNewWave(int waveNumber)
    {
        //Debug.Log(waveNumber);
        string[] numbers = { "One", "Two", "Three", "Four", "Five" };
        newWaveTitle.text = "- Wave " + numbers[waveNumber - 1] + " -";
        string enemyCountString = ((spawner.waves[waveNumber-1].infinite)?"Infinite":spawner.waves[waveNumber-1].enemyCount +"");
        newWaveEnemyCount.text = "Enemies: " + enemyCountString;

        updateMonsterInfo(waveNumber - 1);
        monsterInfoUI.SetActive(true);
        Cursor.visible = true;
        StopCoroutine("AnimateNewWaveBanner");
        StartCoroutine("AnimateNewWaveBanner");
    }

    void updateMonsterInfo(int currentWave)
    {
        currentMonster = monstersInfo[currentWave];
        monsterName.text = currentMonster.monsterName;
        monsterImage.gameObject.GetComponent<Image>().sprite = currentMonster.monsterImage;
        monsterDescription.gameObject.GetComponent<Text>().text = currentMonster.monsterDescription;
    }

    void OnGameOver()
    {
        Cursor.visible = true;
        StartCoroutine(Fade(Color.clear, new Color(0,0,0,.95f), 1));
        gameOverScoreUI.text = scoreUI.text;
        dashButton.gameObject.SetActive(false);
        leftJoystick.gameObject.SetActive(false);
        rightJoystick.gameObject.SetActive(false);
        scoreUI.gameObject.SetActive(false);
        healthBar.transform.parent.gameObject.SetActive(false);
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

            newWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-170, 45, animatePercent);

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
        SceneManager.LoadScene("Game");
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void MonsterInfoContinue()
    {
        Cursor.visible = false;
        monsterInfoUI.gameObject.SetActive(false);
    }

    [System.Serializable]
    public class Monster
    {
        public string monsterName;
        public Sprite monsterImage;
        [TextArea]
        public string monsterDescription;
    }
}
