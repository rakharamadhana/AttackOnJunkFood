using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwipeController : MonoBehaviour
{
    public GameObject scrollBar;

    public float scrollPosition = 0;
    public float[] pos;

    public int position = 0;

    // Start is called before the first frame update
    void Start()
    {
        scrollPosition = 0;
    }

    // Update is called once per frame
    void Update()
    {   
        pos = new float[transform.childCount];
        float distance = 1f / (pos.Length - 1f);
        for(int i = 0; i < pos.Length; i++)
        {
            pos[i] = distance * i;
        }

        if (Input.GetMouseButton(0))
        {
            scrollPosition = scrollBar.GetComponent<Scrollbar>().value;
        }
        else
        {
            for (int i = 0; i<pos.Length;i++)
            {
                if (scrollPosition < pos[i] + (distance/2) && scrollPosition > pos[i] - (distance / 2))
                {
                    scrollBar.GetComponent<Scrollbar>().value = Mathf.Lerp(scrollBar.GetComponent<Scrollbar>().value, pos[i], 0.125f);
                    position = i;
                }
            }
        }
    }

    public void NextSlide()
    {
        if(position<pos.Length - 1)
        {
            position += 1;
            scrollPosition = pos[position];
        }
    }

    public void PrevSlide()
    {
        if (position > 0)
        {
            position -= 1;
            scrollPosition = pos[position];
        }
    }
}
