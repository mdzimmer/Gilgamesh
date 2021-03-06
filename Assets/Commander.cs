﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Commander : MonoBehaviour
{
    public bool enemy = false;
    public Commander opponentCommander;

    Hand hand;
    Board board;
    Deck deck;
    Counter mana;
    List<Tile> spawnField;
    int maxMana = 0;

    // Use this for initialization
    void Start()
    {
        board = FindObjectOfType<Board>();
        board.Initialize();
        deck = GetComponent<Deck>();
        deck.Initialize();
        hand = GetComponent<Hand>();
        hand.Initialize();
        mana = GameObject.Find(enemy ? "EnemyMana" : "PlayerMana").GetComponent<Counter>();
        mana.Initialize(0);
        if (enemy)
        {
            spawnField = new List<Tile>();
            foreach (KeyValuePair<Vector2, Tile> pair in board.board)
            {
                if (pair.Value.enemySpawn)
                {
                    spawnField.Add(pair.Value);
                }
            }
        } else
        {
            TakeTurn();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeTurn()
    {
        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            unit.remainingMovement = unit.def.movement;
            unit.canAttack = true;
            unit.UpdateTaunters();
        }
        maxMana++;
        mana.Set(maxMana);
        hand.Draw();
        if (enemy)
        {
            PlayCards();
            MoveUnits();
            EndTurn();
        }
    }

    public void EndTurn()
    {
        opponentCommander.TakeTurn();
    }

    void PlayCards()
    {
        while (true) {
            bool played = false;
            foreach (Card card in hand.cards)
            {
                if (card.def.cost <= mana.count)
                {
                    mana.count -= card.def.cost;
                    Unit unit = ((GameObject)Instantiate(Resources.Load("Unit"))).GetComponent<Unit>();
                    Tile spawnTile = FindUnoccupiedSpawnTile();
                    if (!spawnTile)
                    {
                        continue;
                    }
                    unit.Initialize(card.def, spawnTile, enemy);
                    unit.enemy = true;
                    hand.Discard(card);
                    played = true;
                    break;
                }
            }
            if (!played)
            {
                break;
            }
        }
    }

    Tile FindUnoccupiedSpawnTile()
    {
        Tile output = null;
        for (int i = 0; i < 100; i++)
        {
            Tile attempt = spawnField[Random.Range(0, spawnField.Count - 1)];
            if (!attempt.occupant)
            {
                output = attempt;
                break;
            }
        }
        return output;
    }

    void MoveUnits()
    {
        Unit[] units = FindObjectsOfType<Unit>();
        foreach (Unit unit in units)
        {
            if (!unit.enemy)
            {
                continue;
            }
            Unit closestAlly = null;
            float closestDistance = Mathf.Infinity;
            Unit[] availableTargets = (unit.taunters.Count == 0 ? units : unit.taunters.ToArray());
            foreach (Unit ally in availableTargets)
            {
                if (ally.enemy)
                {
                    continue;
                }
                int distance = board.TileDistance(unit.curTile, ally.curTile);
                if (distance < closestDistance)
                {
                    closestAlly = ally;
                    closestDistance = distance;
                }
            }
            if (closestAlly)
            {
                int distance = board.TileDistance(unit.curTile, closestAlly.curTile);
                if (distance <= unit.remainingMovement + unit.def.range)
                {
                    unit.MoveTowards(closestAlly.curTile, distance - unit.def.range);
                    unit.Attack(closestAlly);
                } else
                {
                    unit.MoveTowards(closestAlly.curTile, unit.remainingMovement);
                }
            }
        }
    }
}
