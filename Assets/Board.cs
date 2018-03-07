using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width = 1;
    public int height = 1;
    public Tile curHighlight;
    public Dictionary<Vector2, Tile> board;

    float tileSize;
    int spawnFieldDepth = 3;

	// Use this for initialization
	void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {
        Tile mouseTile = GetTileAtMouse();
        if (mouseTile != curHighlight && curHighlight)
        {
            curHighlight.highlight = false;
        }
        if (mouseTile)
        {
            curHighlight = mouseTile;
            curHighlight.highlight = true;
        }
	}

    public void Initialize()
    {
        //get tile size
        Tile sizeTile = ((GameObject)(GameObject.Instantiate(Resources.Load("Tile")))).GetComponent<Tile>();
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
                Tile newTile = ((GameObject)(GameObject.Instantiate(Resources.Load("Tile")))).GetComponent<Tile>();
                board[loc] = newTile;
                newTile.Instantiate(Random.Range(0.0f, 1.0f) < 0.5f ? Tile.Type.WHITE : Tile.Type.BLACK);
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
        Tile centralTile = GetTile(new Vector2(width / 2, height / 2));
        Camera.main.gameObject.GetComponent<CameraControl>().Move(centralTile.transform.position);
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

    public List<Tile> Illuminate(Tile startTile, int movement, int range)
    {
        List<Tile> output = new List<Tile>();
        int total = movement + range + 1;
        for (int i = 0; i < total; i++)
        {
            for (int j = 0; j < total - i; j++)
            {
                //up and down, add unique
                Tile upRight = GetTile(startTile.location + new Vector2(i, j));
                Tile downRight = GetTile(startTile.location + new Vector2(i, -j));
                Tile upLeft = GetTile(startTile.location + new Vector2(-i, j));
                Tile downLeft = GetTile(startTile.location + new Vector2(-i, -j));
                foreach (Tile tile in new[] {upRight, downRight, upLeft, downLeft})
                {
                    if (tile)
                    {
                        tile.illumination = i + j <= movement ? Tile.Illumination.MOVEMENT : Tile.Illumination.ATTACK;
                        output.Add(tile);
                    }
                }
            }
        }
        return output;
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

    List<Tile> GetOpenNeighbors(Tile tile)
    {
        List<Tile> neighbors = new List<Tile>();
        foreach (Vector2 offset in new Vector2[] { new Vector2(-1.0f, 0.0f), new Vector2(0.0f, 1.0f), new Vector2(1.0f, 0.0f), new Vector2(0.0f, -1.0f) })
        {
            Tile neighbor = GetTile(tile.location + offset);
            if (neighbor && !neighbor.occupant)
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
