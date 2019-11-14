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

    // Start is called before the first frame update
    void Start()
    {
        enemySpawner.isDisabled = true;
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
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0 || movementJoystick.Horizontal != 0 || movementJoystick.Vertical != 0)
            {
                //Debug.Log("Moving");
                popUpIndex++;
                
            }
        }
        else if (popUpIndex == 1)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) || rotationJoystick.Horizontal != 0 || rotationJoystick.Vertical != 0)
            {
                //Debug.Log("Shooting");
                popUpIndex++;
            }
        }
        else if (popUpIndex == 2)
        {
            if (Input.GetKeyDown(KeyCode.Space) || player.isDashing)
            {
                //Debug.Log("Dashing");
                popUpIndex++;
            }
        }
        else
        {
            enemySpawner.isDisabled = false;
            Destroy(gameObject);
        }

        //Debug.Log(popUpIndex);
    }
}
