using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaCount : MonoBehaviour
{
    public int count = 1;

    Text text;
    int maxMana = 10;

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
        count = Mathf.Clamp(amt, 0, maxMana);
        text.text = "" + count;
    }

    public void Increment(int amt)
    {
        Set(count + amt);
    }
}
