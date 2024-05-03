using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KT_BackButtonEvent : MonoBehaviour
{
    public Camera currentCamera;
    public Camera UICamera;

    private void Start()
    {
    }

    private void Updata()
    {

    }

    public void Click_BackBtn()
    {
        Debug.Log("success");
        
        currentCamera.gameObject.SetActive(false);
        UICamera.gameObject.SetActive(true);
    }
}
