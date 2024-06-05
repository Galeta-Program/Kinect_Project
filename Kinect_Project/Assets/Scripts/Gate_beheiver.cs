using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate_beheiver : MonoBehaviour
{
    public GameObject gate;
    private Vector2 closedPosition;

    private void Start()
    {
        closedPosition = new Vector2(-17, 101);
    }
    public void closeGate()
    {
        gate.transform.position = closedPosition;
    }
}
