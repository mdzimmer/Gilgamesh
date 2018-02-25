using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamUI : MonoBehaviour
{
    public UIBox discard;
    public UIBox destroy;
    public bool cardHeld = false;

    List<UIBox> elements;

	// Use this for initialization
	void Start ()
    {
        elements = new List<UIBox>();
        elements.Add(discard);
        elements.Add(destroy);
	}
	
	// Update is called once per frame
	void Update ()
    {
        foreach (UIBox box in elements)
        {
            if (box.mouseIn && cardHeld)
            {
                box.triggered = true;
            }
            else
            {
                box.triggered = false;
            }
        }
    }
}
