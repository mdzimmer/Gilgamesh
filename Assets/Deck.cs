using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    List<CardDef> cards;
    Counter count;
    int deckSize = 30;

	// Use this for initialization
	void Start ()
    {

    }
	
	// Update is called once per frame
	void Update ()
    {

	}

    public void Initialize()
    {
        count = FindObjectOfType<Counter>();
        count.Initialize(deckSize);
        cards = new List<CardDef>();
        for (int i = 0; i < deckSize; i++)
        {
            cards.Add(new CardDef(1, 1, 1, 1, 1));
        }
    }

    public CardDef Draw()
    {
        if (cards.Count <= 0)
        {
            return null;
        }
        CardDef output = cards[0];
        cards.Remove(output);
        count.Increment(-1);
        return output;
    }
    
    public class CardDef
    {
        public int cost;
        public int attack;
        public int life;
        public int movement;
        public int range;

        public CardDef(int _cost, int _attack, int _life, int _movement, int _range)
        {
            cost = _cost;
            attack = _attack;
            life = _life;
            movement = _movement;
            range = _range;
        }
    }
}
