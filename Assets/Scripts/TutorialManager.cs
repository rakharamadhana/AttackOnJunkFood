using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject[] popUps;
    private int popUpIndex = 0;
    public Player player;
    public FixedJoystick movementJoystick;
    public FixedJoystick rotationJoystick;
    public GameObject monsterInfoUI;
    public EnemySpawner enemySpawner;
    public float waitTime = 2f;

    bool isReady;

    // Start is called before the first frame update
    void Start()
    {
        enemySpawner.isDisabled = true;
        isReady = false;
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < popUps.Length; i++)
        {
            if(i == popUpIndex && monsterInfoUI.activeSelf != true)
            {
                popUps[i].gameObject.SetActive(true);
            }
            else
            {
                popUps[i].gameObject.SetActive(false);
            }
        }

        if (popUpIndex == 0)
        {
            if (isReady == true)
            {
                //Debug.Log("Moving");
                popUpIndex++;
                
            }
        }
        else if (popUpIndex == 1)
        {
            enemySpawner.isDisabled = false;
            Destroy(gameObject);
        }
    }

    public void Ready()
    {
        isReady = true;
    }
}
