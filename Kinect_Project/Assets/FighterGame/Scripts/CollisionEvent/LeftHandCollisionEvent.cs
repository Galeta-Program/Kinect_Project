using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHandCollisionEvent : MonoBehaviour
{
    public bool lightPunch = false;
    public bool squatDown = false;
    public bool jump = false;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "LightPunch")
        {
            lightPunch = true;
            collision.gameObject.GetComponent<Renderer>().material.color = new Color(0, 255, 0, 255);
        }
        else if (collision.gameObject.tag == "SquatDown")
        {
            squatDown = true;
            collision.gameObject.GetComponent<Renderer>().material.color = new Color(0, 255, 0, 255);
        }
        else if (collision.gameObject.tag == "Jump")
        {
            jump = true;
            collision.gameObject.GetComponent<Renderer>().material.color = new Color(0, 255, 0, 255);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "LightPunch")
        {
            lightPunch = false;
            collision.gameObject.GetComponent<Renderer>().material.color = new Color(255, 255, 255, 255);
        }
        else if (collision.gameObject.tag == "SquatDown")
        {
            squatDown = false;
            collision.gameObject.GetComponent<Renderer>().material.color = new Color(255, 255, 255, 255);
        }
        else if (collision.gameObject.tag == "Jump")
        {
            jump = false;
            collision.gameObject.GetComponent<Renderer>().material.color = new Color(255, 255, 255, 255);
        }
    }
}
