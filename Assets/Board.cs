using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class Board : MonoBehaviour
{
    public TextAsset definitionFile;
    public int width = 1;
    public int height = 1;
    public Tile curHighlight;
    public Dictionary<Vector2, Tile> board;

    float tileSize;
    int spawnFieldDepth = 3;
    bool initialized = false;
    List<Tile> illuminated;

	// Use this for initialization
	void Start ()
    {

    }
	
	// Update is called once per frame
	void Update ()
    {
        Tile mouseTile = GetTileAtMouse();
        if (mouseTile != curHighlight)
        {
            if (curHighlight)
            {
                curHighlight.highlight = false;
            }
            if (mouseTile)
            {
                mouseTile.highlight = true;
				if (Input.GetKey(KeyCode.T)) {
					mouseTile.SetType ((mouseTile.type == Tile.Type.BLACK) ? Tile.Type.WHITE : Tile.Type.BLACK);
				}
            }
            curHighlight = mouseTile;
        }
        if (Unit.usingUnit)
        {
            //keep old illumination
        } else if (curHighlight && curHighlight.occupant)
        {
            Illuminate(curHighlight.occupant);
        } else if (illuminated.Count > 0)
        {
            ClearIllumination();
        }
	}

    public void Initialize()
    {
        if (initialized)
        {
            return;
        }
        var definition = JSON.Parse(definitionFile.text)["definition"];
        width = definition[0].Count;
        height = definition.Count;
        //get tile size
        Tile sizeTile = ((GameObject)(Instantiate(Resources.Load("Tile")))).GetComponent<Tile>();
        sizeTile.Instantiate(Tile.Type.WHITE);
        tileSize = sizeTile.sr.bounds.extents.x * 2.0f;
        Destroy(sizeTile.gameObject);
        //spawn board tiles
        board = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 loc = new Vector2(x, y);
                Tile newTile = ((GameObject)(Instantiate(Resources.Load("Tile")))).GetComponent<Tile>();
                board[loc] = newTile;
                newTile.Instantiate(definition[y][x] == 0 ? Tile.Type.WHITE : Tile.Type.BLACK);
                newTile.transform.position = (Vector2)transform.position + (loc * tileSize) + new Vector2(tileSize / 2.0f, tileSize / 2.0f);
                newTile.transform.parent = transform;
                newTile.location = loc;
                if (y < spawnFieldDepth)
                {
                    newTile.playerSpawn = true;
                }
                else if (height - y <= spawnFieldDepth)
                {
                    newTile.enemySpawn = true;
                }
            }
        }
        Camera.main.gameObject.GetComponent<CameraControl>().Move(GetTile(new Vector2(width / 2, 0)).transform.position);
        initialized = true;
        illuminated = new List<Tile>();
    }

    public Tile GetTileAtMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 position = new Vector2(Mathf.FloorToInt(mousePos.x / tileSize), Mathf.FloorToInt(mousePos.y / tileSize));
        return GetTile(position);
    }

    public int TileDistance(Tile a, Tile b)
    {
        int difX = (int)(Mathf.Abs(a.location.x - b.location.x));
        int difY = (int)(Mathf.Abs(a.location.y - b.location.y));
        return difX + difY;
    }

	public void Illuminate(Unit unit)
	{
		ClearIllumination();
		Tile startTile = unit.curTile;
		Dictionary<Tile, bool> explored = new Dictionary<Tile, bool> ();
		RecursiveIlluminate (startTile, startTile, explored, unit);
	}

	void RecursiveIlluminate(Tile start, Tile cur, Dictionary<Tile, bool> explored, Unit unit) {
		if (!cur || explored.ContainsKey (cur)) {
			return;
		}
		if (cur.type == Tile.Type.BLACK) {
			explored[cur] = true;
			return;
		}
		int distance = PathTo (start, cur).Count;
		Unit occupant = cur.occupant;
		if (distance <= unit.remainingMovement) {
			if (!occupant) {
				cur.illumination = Tile.Illumination.MOVEMENT;
				illuminated.Add (cur);
			}
		} else if (distance <= unit.remainingMovement + (unit.canAttack ? unit.def.range : 0)) {
			if (!occupant || (occupant && occupant.enemy != unit.enemy)) {
				cur.illumination = Tile.Illumination.ATTACK;
				illuminated.Add (cur);
			}
		} else {
			explored [cur] = true;
			return;
		}
		explored [cur] = true;
		Tile up = GetTile(cur.location + new Vector2(0, 1));
		Tile down = GetTile(cur.location + new Vector2(0, -1));
		Tile left = GetTile(cur.location + new Vector2(-1, 0));
		Tile right = GetTile(cur.location + new Vector2(1, 0));
		foreach (Tile neighbor in new[] {up, down, left, right}) {
			RecursiveIlluminate (start, neighbor, explored, unit);
		}
	}

    public Tile GetTile(Vector2 location)
    {
        if (board.ContainsKey(location))
        {
            return board[location];
        }
        return null;
    }

    public List<Tile> PathTo(Tile start, Tile goal)
    {
        PriorityQueue frontier = new PriorityQueue(width * height);
        frontier.Enqueue(start, 0);
        Dictionary<Tile, Tile> cameFrom = new Dictionary<Tile, Tile>();
        Dictionary<Tile, int> costSoFar = new Dictionary<Tile, int>();
        cameFrom[start] = null;
        costSoFar[start] = 0;
        Tile closestNode = null;
        float closestNodeDistance = Mathf.Infinity;
        while (frontier.Count() > 0)
        {
            Tile current = frontier.Dequeue();
            if (current == goal)
            {
                break;
            }
            int currentDistance = TileDistance(current, goal);
            if (currentDistance < closestNodeDistance)
            {
                closestNode = current;
                closestNodeDistance = currentDistance;
            }
            foreach (Tile next in GetOpenNeighbors(current))
            {
                int navCost = costSoFar[current] + 1;
                if (!costSoFar.ContainsKey(next) || navCost < costSoFar[next])
                {
                    costSoFar[next] = navCost;
                    frontier.Enqueue(next, navCost);
                    cameFrom[next] = current;
                }
            }
        }
        List<Tile> path = new List<Tile>();
        Tile cur = goal;
        if (!cameFrom.ContainsKey(goal))
        {
            cur = closestNode;
        }
        while (true) {
            path.Add(cur);
            if (cur == start)
            {
                break;
            }
            cur = cameFrom[cur];
        }
        path.Reverse();
        return path;
    }

    void ClearIllumination()
    {
        foreach (Tile tile in illuminated)
        {
            tile.illumination = Tile.Illumination.NONE;
        }
        illuminated.Clear();
    }

    List<Tile> GetOpenNeighbors(Tile tile)
    {
        List<Tile> neighbors = new List<Tile>();
        foreach (Vector2 offset in new Vector2[] { new Vector2(-1.0f, 0.0f), new Vector2(0.0f, 1.0f), new Vector2(1.0f, 0.0f), new Vector2(0.0f, -1.0f) })
        {
            Tile neighbor = GetTile(tile.location + offset);
			if (neighbor && neighbor.type == Tile.Type.WHITE)
            {
                neighbors.Add(neighbor);
            }
        }
        return neighbors;
    }

    class PriorityQueue
    {
        Dictionary<int, List<Tile>> storage;
        int max;

        public PriorityQueue(int _max)
        {
            storage = new Dictionary<int, List<Tile>>();
            max = _max;
        }

        public void Enqueue(Tile tile, int priority)
        {
            if (!storage.ContainsKey(priority))
            {
                storage[priority] = new List<Tile>();
            }
            storage[priority].Add(tile);
        }

        public Tile Dequeue()
        {
            float lowest = Mathf.Infinity;
            Tile output = null;
            for (int i = 0; i < max; i++) {
                if (!storage.ContainsKey(i))
                {
                    continue;
                }
                List<Tile> value = storage[i];
                if (i < lowest)
                {
                    lowest = i;
                    output = value[0];
                    value.Remove(output);
                    if (value.Count <= 0)
                    {
                        storage.Remove(i);
                    }
                }
            }
            return output;
        }

        public int Count()
        {
            return storage.Count;
        }
    }
}
