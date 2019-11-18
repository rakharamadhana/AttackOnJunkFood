using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoading : MonoBehaviour
{
    public Image progressBar;
    public Text tipsHolderContent;
    public Tips[] tips;

    Tips currentTips;

    // Start is called before the first frame update
    void Start()
    {
        //start async operation
        StartCoroutine("LoadAsyncOperation");
        currentTips = tips[Random.Range(0, tips.Length)];
    }

    void Update()
    {
        tipsHolderContent.text = currentTips.tipsContent;
    }

    IEnumerator LoadAsyncOperation()
    {
        //create an async operatoin
        AsyncOperation gameLevel = SceneManager.LoadSceneAsync("Game");
        
        while(gameLevel.progress < 1)
        {
            //take the progress bar fill = async operation progress.
            progressBar.fillAmount = gameLevel.progress;

            yield return new WaitForEndOfFrame();
        }

        //when finished, load the game scene
    }

    [System.Serializable]
    public class Tips
    {
        [TextArea]
        public string tipsContent;
    }
}
