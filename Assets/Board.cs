using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width = 1;
    public int height = 1;

    Dictionary<Vector2, Tile> board;
    float tileSize;

	// Use this for initialization
	void Start ()
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
                newTile.transform.position = (Vector2)transform.position + loc * tileSize;
                newTile.transform.parent = transform;
            }
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
