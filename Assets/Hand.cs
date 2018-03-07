using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public bool enemy = false;
    public List<Card> cards;

    Deck deck;
    Vector2 handLowPoint = new Vector2(0.0f, -34.5f);
    float angleBetweenCardsDeg = 2.0f;
    float handOffset = 30.0f;
    int startingHandSize = 5;
    Camera cam;

	void Start ()
    {

    }
	
	void Update ()
    {

	}

    public void Initialize()
    {
        cam = Camera.main;
        cards = new List<Card>();
        deck = GetComponent<Deck>();
        for (int i = 0; i < startingHandSize; i++)
        {
            Draw();
        }
    }

    public void Draw()
    {
        Deck.CardDef cardDef = deck.Draw();
        if (cardDef != null)
        {
            Card card = ((GameObject)(Instantiate(Resources.Load("Card")))).GetComponent<Card>();
            card.Initialize(cardDef, enemy);
            cards.Add(card);
        }
        UpdateCards();
    }

    public void Discard(Card card)
    {
        cards.Remove(card);
        Destroy(card.gameObject);
        UpdateCards();
    }

    void UpdateCards()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            Card card = cards[i];
            float index = i + 1.0f;
            bool totalEven = cards.Count % 2 == 0;
            float middleIndex = cards.Count / 2.0f + (totalEven ? 0.5f : 0.0f);
            float deltaDeg = angleBetweenCardsDeg * (index - middleIndex - (totalEven ? 0.0f : 0.5f));
            float deltaRad = deltaDeg * Mathf.Deg2Rad;
            Vector2 offset = handOffset * Vector2.up;
            offset = new Vector2((Mathf.Cos(deltaRad) * offset.x - Mathf.Sin(deltaRad) * offset.y),
                                 (Mathf.Sin(deltaRad) * offset.x + Mathf.Cos(deltaRad) * offset.y));
            Vector2 point = offset + handLowPoint;
            if (enemy)
            {
                point = -point + new Vector2(0.0f, 1.0f);
            }
            card.targetPos = point;
            if (enemy)
            {
                deltaDeg += 180.0f;
            }
            card.targetRotation = deltaDeg;
            card.sortingOrder = i;
        }
    }
}
