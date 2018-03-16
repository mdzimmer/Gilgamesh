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
    public List<Unit> taunters;

    public static bool usingUnit = false;

    Board board;
    bool inUse = false;
    int curLife;
    Objective obj;
    SpriteRenderer sr;
    Card showcase;
    List<Trait> traits;

	// Use this for initialization
	void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {
        bool mousedOver = (board.curHighlight == curTile && !usingUnit);
        if (!showcase && !inUse && mousedOver)
        {
            showcase = Instantiate(Resources.Load<Card>("Card"));
            showcase.Initialize(def, false, null, true);
            showcase.transform.position = transform.position + new Vector3(0.0f, 2.0f, 1.0f) * (enemy ? 1.0f : -1.0f);
        } else if (showcase && (inUse || !mousedOver))
        {
            Destroy(showcase.gameObject);
            showcase = null;
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
			int dist = board.PathTo(mouseTile, curTile).Count;
            if (mouseTile.occupant && mouseTile.occupant.enemy && canAttack)
            {
                if (remainingMovement + def.range >= dist)
                {
                    if (taunters.Count == 0 || taunters.Contains(mouseTile.occupant))
                    {
                        MoveTowards(mouseTile, dist - def.range);
                        Attack(mouseTile.occupant);
                    }
                }
            } else
            {
                if (dist <= remainingMovement)
                {
					Debug.Log (dist + " : " + remainingMovement);
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
        //illuminated = new List<Tile>();
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
        traits = new List<Trait>();
        if (def.description.Contains("Taunt"))
        {
            traits.Add(Trait.TAUNT);
        }
        if (def.description.Contains("Charge"))
        {
            traits.Add(Trait.CHARGE);
        }
        if (def.description.Contains("Poison"))
        {
            traits.Add(Trait.POISON);
        }
        if (traits.Contains(Trait.CHARGE))
        {
            remainingMovement = def.movement;
            canAttack = true;
        }
        taunters = new List<Unit>();
    }

    public void UpdateTaunters()
    {
        taunters.Clear();
        foreach(Unit unit in FindObjectsOfType<Unit>())
        {
            if (unit.enemy == enemy)
            {
                continue;
            }
            if (!unit.traits.Contains(Trait.TAUNT))
            {
                continue;
            }
            if (board.TileDistance(curTile, unit.curTile) > remainingMovement + def.range)
            {
                continue;
            }
            taunters.Add(unit);
        }
    }

    public void Attack(Unit opponent)
    {
        opponent.TakeDamage(def.attack);
        if (opponent && traits.Contains(Trait.POISON))
        {
            opponent.Die();
        }
        canAttack = false;
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
		Tile go = curTile;
		for (int i = Mathf.Min(dist, path.Count - 1); i >= 0; i--) {
			if (!path [i].occupant) {
				go = path [i];
				break;
			}
		}
		MoveTo (go);
    }

    public void Die()
    {
        if (enemy)
        {
            obj.IncrementProgress(-1);
        }
        Destroy(gameObject);
    }

    void MoveTo(Tile target)
    {
        if (curTile)
        {
            curTile.occupant = null;
			remainingMovement -= board.PathTo(curTile, target).Count;
			Debug.Log (remainingMovement);
        }
        curTile = target;
        curTile.occupant = this;
        transform.position = curTile.transform.position;
    }

    enum Trait
    {
        TAUNT,
        CHARGE,
        POISON
    }
}
