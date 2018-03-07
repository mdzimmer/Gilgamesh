using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    ManaCount mc;
    int playerBaseMana = 1;
    bool playerTurn = true;
    Commander player;
    Commander enemy;

	// Use this for initialization
	void Start ()
    {
        mc = FindObjectOfType<ManaCount>();
        player = GameObject.Find("Player").GetComponent<Commander>();
        enemy = GameObject.Find("Enemy").GetComponent<Commander>();
        StartTurn(player);
    }
	
	// Update is called once per frame
	void Update ()
    {

	}

    public void EndPlayerTurn()
    {
        mc.Set(0);
        playerTurn = false;
        //ec.TakeTurn();
    }

    public void EndEnemyTurn()
    {
        playerBaseMana++;
        mc.Set(playerBaseMana);
        playerTurn = true;
        foreach(Unit unit in FindObjectsOfType<Unit>())
        {
            unit.remainingMovement = unit.def.movement;
            unit.canAttack = true;
        }
    }

    void StartTurn(Commander commander)
    {
        //commander.DoTurn();
    }
}
