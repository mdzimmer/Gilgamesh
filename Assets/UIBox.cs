using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBox : MonoBehaviour
{
    public bool triggered = false;
    public bool mouseIn = false;

    SpriteRenderer sr;
    float fullOpacity = 0.5f;

	// Use this for initialization
	void Start ()
    {
        sr = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        Color newColor = sr.color;
        newColor.a = triggered ? fullOpacity : 0.0f;
        sr.color = newColor;
    }

    void OnMouseEnter()
    {
        mouseIn = true;
    }

    void OnMouseExit()
    {
        mouseIn = false;
    }
}
