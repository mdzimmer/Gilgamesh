using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckCount : MonoBehaviour
{
    public int count = 30;

    Text text;

	// Use this for initialization
	void Start ()
    {
        text = GetComponent<Text>();
        Set(count);
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void Set (int amt)
    {
        count = amt;
        text.text = "" + count;
    }

    public void Increment (int amt)
    {
        Set(count + amt);
    }
}
