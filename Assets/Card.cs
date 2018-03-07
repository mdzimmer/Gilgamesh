using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Vector2 targetPos;
    public float targetRotation;
    public float sortingOrder = 0;
    public Deck.CardDef def;
    public bool enemy = false;

    public static bool showSpawnTiles = false;

    Camera cam;
    Vector2 startOffset = new Vector2(9.0f, -9.0f);
    float sortingDistance = 0.1f;
    float mouseOverOffset = 1.0f;
    bool mousedOver = false;
    bool grabbed = false;
    Hand hand;
    Board board;
    Counter mana;
    Text description;
    Text cost;
    Text life;
    Text attack;
    Text movement;
    Text range;
    Image visual;

	// Use this for initialization
	void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!Input.GetMouseButton(0) && grabbed) //play card
        {
            Tile targetTile = board.GetTileAtMouse();
            if (!mousedOver && targetTile && def.cost <= mana.count)
            {
                Unit unit = ((GameObject)(GameObject.Instantiate(Resources.Load("Unit")))).GetComponent<Unit>();
                unit.Initialize(def, targetTile, enemy);
                hand.Discard(this);
                mana.Increment(-def.cost);
            } else
            {
                grabbed = false;
            }
            showSpawnTiles = false;
        }
        bool doMouseOver = grabbed || mousedOver;
        //move towards target
        Vector2 camPos = cam.transform.position;
        transform.position = (Vector3)camPos + (Vector3)targetPos + transform.up * (doMouseOver ? mouseOverOffset : 0.0f) + new Vector3(0.0f, 0.0f, (doMouseOver ? -1.0f : sortingOrder) * sortingDistance);
        transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, (grabbed ? 0.0f : targetRotation)));
	}

    public void Initialize(Deck.CardDef _def, bool _enemy)
    {
        def = _def;
        enemy = _enemy;
        cam = Camera.main;
        transform.position = cam.transform.position - (Vector3)startOffset;
        targetPos = transform.position;
        targetRotation = transform.rotation.eulerAngles.z;
        hand = FindObjectOfType<Hand>();
        board = FindObjectOfType<Board>();
        mana = GameObject.Find("PlayerMana").GetComponent<Counter>();
        description = transform.GetChild(0).GetComponent<Text>();
        visual = transform.GetChild(1).GetComponent<Image>();
        cost = transform.GetChild(2).GetComponent<Text>();
        life = transform.GetChild(3).GetComponent<Text>();
        attack = transform.GetChild(4).GetComponent<Text>();
        movement = transform.GetChild(5).GetComponent<Text>();
        range = transform.GetChild(6).GetComponent<Text>();
        if (enemy)
        {
            description.enabled = false;
            visual.enabled = false;
            cost.enabled = false;
            life.enabled = false;
            attack.enabled = false;
            movement.enabled = false;
            range.enabled = false;
        }
    }

    void OnMouseEnter()
    {
        if (enemy)
        {
            return;
        }
        mousedOver = true;
    }

    void OnMouseExit()
    {
        if (enemy)
        {
            return;
        }
        mousedOver = false;
    }

    void OnMouseDown()
    {
        if (enemy)
        {
            return;
        }
        grabbed = true;
        showSpawnTiles = true;
    }
}
