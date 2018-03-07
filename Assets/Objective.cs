using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Objective : MonoBehaviour
{
    Text text;
    int goal = 10;
    string description = "Kill {0} enemy units";

	// Use this for initialization
	void Start ()
    {
        text = GetComponent<Text>();
        UpdateProgress();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void IncrementProgress(int amt)
    {
        goal += amt;
        UpdateProgress();
    }

    void UpdateProgress()
    {
        text.text = string.Format(description, goal);
    }
}
