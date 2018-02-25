using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    Deck deck;
    List<Card> cards;
    //card positions
    Vector2 handLowPoint = new Vector2(0.0f, -34.5f);
    float angleBetweenCardsDeg = 2.0f;
    float handOffset = 30.0f;

	// Use this for initialization
	void Start ()
    {
        deck = GameObject.FindObjectOfType<Deck>();
        cards = new List<Card>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		//if less than 5 cards, draw
        while (cards.Count < 5)
        {
            Deck.CardDef cardDef = deck.Draw();
            if (cardDef == null) //stop when no cards in deck
            {
                break;
            }
            //make card object
            Card card = ((GameObject)(GameObject.Instantiate(Resources.Load("Card")))).GetComponent<Card>();
            cards.Add(card);
        }
        for(int i = 0; i < cards.Count; i++)
        {
            Card card = cards[i];
            //Vector2 targetPos = GetTargetPos(i, cards.Count);
            //card.targetPos = targetPos;
            UpdateCard(card, i, cards.Count);
        }
	}

    public void Draw()
    {

    }

    void UpdateCard(Card card, int realIndex, int total)
    {
        float index = realIndex + 1.0f; //undo zero indexing on index and make float
        bool totalEven = total % 2 == 0;
        float middleIndex = (float)total / 2.0f + (totalEven ? 0.5f : 0.0f); // 1->0.5, 2->1.5, 3->1.5, 4->2.5, 5->2.5, etc
        float deltaDeg = angleBetweenCardsDeg * (index - middleIndex - (totalEven ? 0.0f : 0.5f));
        float deltaRad = deltaDeg * Mathf.Deg2Rad; //convert to rads for sin and cos
        Vector2 lowPoint = Vector2.zero + handLowPoint;
        Vector2 offset = handOffset * Vector2.up;
        offset = new Vector2((Mathf.Cos(deltaRad) * offset.x - Mathf.Sin(deltaRad) * offset.y),
                             (Mathf.Sin(deltaRad) * offset.x + Mathf.Cos(deltaRad) * offset.y)); //rotate offset by angle
        card.targetPos = offset + lowPoint;
        card.targetRotation = deltaDeg;
        card.sortingOrder = realIndex;
    }
}
