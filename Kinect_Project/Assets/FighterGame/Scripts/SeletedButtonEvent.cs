using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeletedButtonEvent : MonoBehaviour
{
    public void Click_SeletionBtn()
    {
        GetComponent<Image>().color = new Color(130, 130, 130);
    }
}
