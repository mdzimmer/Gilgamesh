using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Type type;
    public SpriteRenderer sr;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void Instantiate(Type _type)
    {
        sr = GetComponent<SpriteRenderer>();
        type = _type;
        switch (type)
        {
            case Type.WHITE:
                sr.color = Color.white;
                break;
            case Type.BLACK:
                sr.color = Color.black;
                break;
        }
    }

    public enum Type
    {
        WHITE,
        BLACK
    }
}
