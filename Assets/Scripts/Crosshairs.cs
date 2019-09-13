using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshairs : MonoBehaviour
{
    public LayerMask targetMask;
    public SpriteRenderer dot;
    public float rotationSpeed;

    public Color dotHighlightColour;
    Color originalDotColour;

    private void Start()
    {
        originalDotColour = dot.color;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }

    public void DetectTargets (Ray ray)
    {
        if(Physics.Raycast(ray, 100, targetMask))
        {
            dot.color = dotHighlightColour;
        }else
        {
            dot.color = originalDotColour;
        }
    }
}
