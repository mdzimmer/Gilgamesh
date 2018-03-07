using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Counter : MonoBehaviour
{
    public int count;
    public float max = Mathf.Infinity;

    Text text;

    // Use this for initialization
    void Start()
    {

    }

    public void Initialize(int startVal)
    {
        text = GetComponent<Text>();
        Set(startVal);
    }

    public void Set(int amt)
    {
        count = (int)Mathf.Clamp(amt, 0, max);
        text.text = "" + count;
    }

    public void Increment(int amt)
    {
        Set(count + amt);
    }
}
