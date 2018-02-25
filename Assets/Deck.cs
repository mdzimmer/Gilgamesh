using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    List<CardDef> cards;
    float deckSize = 30;
    bool initialized = false;

	// Use this for initialization
	void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public CardDef Draw()
    {
        if (!initialized)
        {
            Initialize();
        }
        if (cards.Count <= 0)
        {
            return null;
        }
        CardDef output = cards[0];
        cards.Remove(output);
        return output;
    }

    void Initialize()
    {
        cards = new List<CardDef>();
        for (int i = 0; i < 10; i++)
        {
            cards.Add(new CardDef(1));
        }
    }
    
    public class CardDef
    {
        int cost;

        public CardDef(int _cost)
        {
            cost = _cost;
        }
    }
}
