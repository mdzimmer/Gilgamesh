using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public TextAsset definition;
    public Counter count;

    List<CardDef> cards;
    Dictionary<string, CardDef> cardDefsDict;

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
        cards = new List<CardDef>();
        cardDefsDict = new Dictionary<string, CardDef>();
        CardDefCollection cardDefs = JsonUtility.FromJson<CardDefCollection>(Resources.Load<TextAsset>("cards").text);
        foreach (CardDef def in cardDefs.cards)
        {
            cardDefsDict[def.name] = def;
        }
        StartingCards startingCards = JsonUtility.FromJson<StartingCards>(definition.text);
        foreach (string card in startingCards.cards)
        {
            cards.Add(cardDefsDict[card]);
        }
        System.Random rng = new System.Random();
        int n = cards.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            CardDef temp = cards[k];
            cards[k] = cards[n];
            cards[n] = temp;
        }
        count.Initialize(startingCards.cards.Length);
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

    [Serializable]
    public class CardDef
    {
        public int cost;
        public int attack;
        public int life;
        public int movement;
        public int range;
        public string name;
        public string description;
        public string visual;
    }

    [Serializable]
    class CardDefCollection
    {
        public CardDef[] cards;
    }

    [Serializable]
    class StartingCards
    {
        public string[] cards;
    }
}
