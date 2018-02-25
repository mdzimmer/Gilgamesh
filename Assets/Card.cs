using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Vector2 targetPos;
    public float targetRotation;
    public float sortingOrder = 0;

    Camera cam;
    Vector2 startOffset = new Vector2(9.0f, -9.0f);
    float sortingDistance = 0.1f;
    float mouseOverOffset = 1.0f;
    bool mousedOver = false;
    bool grabbed = false;
    CamUI ui;

	// Use this for initialization
	void Start ()
    {
        cam = Camera.main;
        transform.position = cam.transform.position - (Vector3)startOffset;
        targetPos = transform.position;
        targetRotation = transform.rotation.eulerAngles.z;
        ui = cam.GetComponent<CamUI>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!Input.GetMouseButton(0) && grabbed)
        {
            grabbed = false;
            ui.cardHeld = false;
        }
        bool doMouseOver = grabbed || mousedOver;
        //move towards target
        Vector2 camPos = cam.transform.position;
        transform.position = (Vector3)camPos + (Vector3)targetPos + transform.up * (doMouseOver ? mouseOverOffset : 0.0f) + new Vector3(0.0f, 0.0f, (doMouseOver ? -1.0f : sortingOrder) * sortingDistance);
        transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, (grabbed ? 0.0f : targetRotation)));
	}

    void OnMouseEnter()
    {
        mousedOver = true;
    }

    void OnMouseExit()
    {
        mousedOver = false;
    }

    void OnMouseDown()
    {
        grabbed = true;
        ui.cardHeld = true;
    }
}
