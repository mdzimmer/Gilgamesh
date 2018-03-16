using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Type type;
    public SpriteRenderer sr;
    public SpriteRenderer overlay;
    public bool playerSpawn = false;
    public bool enemySpawn = false;
    public bool highlight = false;
    public Illumination illumination = Illumination.NONE;
    public Vector2 location;
    public Unit occupant;

    float overlayOpacity = 0.5f;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Card.showSpawnTiles && playerSpawn)
        {
            illumination = Illumination.SPAWN;
        } else if (!Card.showSpawnTiles && illumination == Illumination.SPAWN)
        {
            illumination = Illumination.NONE;
        }
        if (highlight || illumination != Illumination.NONE)
        {
            overlay.enabled = true;
        } else
        {
            overlay.enabled = false;
        }
        Color overlayColor = Color.white;
        overlayColor.a = 0.5f;
        switch (illumination)
        {
            case Illumination.ATTACK:
                overlayColor = Color.red;
                break;
            case Illumination.MOVEMENT:
                overlayColor = Color.blue;
                break;
            case Illumination.SPAWN:
                overlayColor = Color.cyan;
                break;
        }
        if (highlight)
        {
            overlayColor = Color.green;
        }
        overlayColor.a = overlayOpacity;
        overlay.color = overlayColor;
    }

    public void Instantiate(Type _type)
    {
        sr = GetComponent<SpriteRenderer>();
		SetType (_type);
        overlay = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

	public void SetType(Type newType) {
		type = newType;
		switch (type)
		{
		case Type.WHITE:
			sr.color = Color.grey;
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

    public enum Illumination
    {
        NONE,
        MOVEMENT,
        ATTACK,
        SPAWN
    }
}
