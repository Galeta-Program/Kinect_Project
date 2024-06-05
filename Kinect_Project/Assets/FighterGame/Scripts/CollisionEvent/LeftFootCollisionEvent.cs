using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftFootCollisionEvent : MonoBehaviour
{
    public bool forward = false;
    public bool backward = false;
    public bool lightKick = false;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Forward")
        {
            forward = true;
            collision.gameObject.GetComponent<Renderer>().material.color = new Color(0, 255, 0, 255);
        }
        else if (collision.gameObject.tag == "Backward")
        {
            backward = true;
            collision.gameObject.GetComponent<Renderer>().material.color = new Color(0, 255, 0, 255);
        }
        else if (collision.gameObject.tag == "LightKick")
        {
            lightKick = true;
            collision.gameObject.GetComponent<Renderer>().material.color = new Color(0, 255, 0, 255);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Forward")
        {
            forward = false;
            collision.gameObject.GetComponent<Renderer>().material.color = new Color(255, 255, 255, 255);
        }
        else if (collision.gameObject.tag == "Backward")
        {
            backward = false;
            collision.gameObject.GetComponent<Renderer>().material.color = new Color(255, 255, 255, 255);
        }
        else if (collision.gameObject.tag == "LightKick")
        {
            lightKick = false;
            collision.gameObject.GetComponent<Renderer>().material.color = new Color(255, 255, 255, 255);
        }
    }
}
