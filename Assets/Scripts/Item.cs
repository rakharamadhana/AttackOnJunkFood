using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{    
    private Player playerEntity;
    public int scorePoint;
    public int healthPoint;
    public GameObject scoreKeeper;

    private void Awake()
    {
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            playerEntity = FindObjectOfType<Player>();
        }        
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 50 * Time.deltaTime, 0); //rotates 50 degrees per second around z axis
    }

    private void OnCollisionEnter(Collision collider)
    {
        
        if(collider.gameObject.name == "Player")
        {
            //Debug.Log("GET");
            AudioManager.instance.PlaySound("Item Get", transform.position);
            playerEntity.TakeRecovery(healthPoint);
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        AddScore(scorePoint);
    }

    public void AddScore(int value)
    {
        ScoreKeeper.score += value;
    }
}
