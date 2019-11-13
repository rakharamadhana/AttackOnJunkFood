using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    public static int score { get; private set; }
    float lastEnemyKilledTime;
    public static int streakCount { get; private set; }
    float streakExpiryTime = 1;

    private void Start()
    {
        Enemy.OnDeathStatic += OnEnemyKilled;
        FindObjectOfType<Player>().OnDeath += OnPlayerDeath;
        score = 0;
    }

    void OnEnemyKilled()
    {
        if(Time.time < lastEnemyKilledTime + streakExpiryTime)
        {
            streakCount++;
        }
        else
        {
            streakCount = 0;
        }

        lastEnemyKilledTime = Time.time;

        AddScore(10);
    }

    public void AddScore(int value)
    {
        if (streakCount > 0)
        {
            score += value * streakCount;
        }
        else
        {
            score += value;
        }
    }

    void OnPlayerDeath()
    {
        Enemy.OnDeathStatic -= OnEnemyKilled;
    }
}
