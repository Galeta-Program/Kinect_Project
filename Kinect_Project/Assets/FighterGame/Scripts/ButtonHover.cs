using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float transparencyRate = 0.03f;
    public bool isActive = false;

    void Update()
    {
        if (isActive)
        {
            Color color = GetComponent<Image>().color;
            if (color.a - transparencyRate <= 0 || color.a - transparencyRate >= 1)
            {
                transparencyRate = -transparencyRate;
            }
            GetComponent<Image>().color = new Color(color.r, color.g, color.b, color.a - transparencyRate);
        }       
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("Enter");
        isActive = true;            
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isActive = false;
        Color color = GetComponent<Image>().color;
        GetComponent<Image>().color = new Color(color.r, color.g, color.b, 1);
    }
}
