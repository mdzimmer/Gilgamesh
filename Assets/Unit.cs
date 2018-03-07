using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public bool enemy = false;
    public bool canAttack = false;
    public int remainingMovement = 0;
    public Deck.CardDef def;
    public Tile curTile;

    static bool usingUnit = false;

    Board board;
    bool inUse = false;
    List<Tile> illuminated;
    int curLife;
    Objective obj;
    SpriteRenderer sr;

	// Use this for initialization
	void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!board || illuminated == null)
        {
            return;
        }
        bool mousedOver = (board.curHighlight == curTile && !usingUnit);
        if (mousedOver && !inUse)
        {
            illuminated = board.Illuminate(curTile, remainingMovement, canAttack ? def.range : 0);
        }
        else if (illuminated.Count != 0 && !inUse)
        {
            ClearIllumination();
        }
        if (enemy)
        {
            return;
        }
		if (Input.GetMouseButtonDown(0) && mousedOver)
        {
            inUse = true;
            usingUnit = true;
        }
        else if (Input.GetMouseButtonUp(0) && inUse)
        {
            Tile mouseTile = board.GetTileAtMouse();
            int dist = board.TileDistance(mouseTile, curTile);
            if (mouseTile.occupant && mouseTile.occupant.enemy && canAttack)
            {
                if (remainingMovement + def.range >= dist)
                {
                    MoveTowards(mouseTile, dist - def.range);
                    mouseTile.occupant.TakeDamage(def.attack);
                    canAttack = false;
                }
            } else
            {
                if (dist <= remainingMovement)
                {
                    MoveTo(mouseTile);
                }
            }
            inUse = false;
            usingUnit = false;
        }
    }

    public void Initialize(Deck.CardDef _def, Tile startingTile, bool _enemy)
    {
        board = FindObjectOfType<Board>();
        illuminated = new List<Tile>();
        def = _def;
        enemy = _enemy;
        MoveTo(startingTile);
        curLife = def.life;
        obj = FindObjectOfType<Objective>();
        sr = GetComponent<SpriteRenderer>();
        if (enemy)
        {
            sr.color = Color.red;
        }
    }

    public void TakeDamage(int damage)
    {
        curLife -= damage;
        if (curLife <= 0)
        {
            Die();
        }
    }

    public void MoveTowards(Tile target, int dist)
    {
        List<Tile> path = board.PathTo(curTile, target);
        MoveTo(path[Mathf.Min(dist, path.Count - 1)]);
    }

    void ClearIllumination()
    {
        if (illuminated.Count == 0)
        {
            return;
        }
        foreach (Tile tile in illuminated)
        {
            tile.illumination = Tile.Illumination.NONE;
        }
        illuminated.Clear();
    }

    void MoveTo(Tile target)
    {
        if (curTile)
        {
            curTile.occupant = null;
            remainingMovement -= (int)(target.location - curTile.location).magnitude;
        }
        curTile = target;
        curTile.occupant = this;
        transform.position = curTile.transform.position;
        ClearIllumination();
    }

    void Die()
    {
        if (enemy)
        {
            obj.IncrementProgress(-1);
        }
        Destroy(gameObject);
    }
}
