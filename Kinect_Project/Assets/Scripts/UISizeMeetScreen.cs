using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISizeMeetScreen : MonoBehaviour
{
    const float oriWidth = 673f;
    const float oriHeight = 438f;

    // Start is called before the first frame update
    void Start()
    {
        float wRatio = gameObject.transform.GetComponent<RectTransform>().sizeDelta.x / oriWidth;
        float hRatio = gameObject.transform.GetComponent<RectTransform>().sizeDelta.y / oriHeight;
        float avgRatio = (wRatio + hRatio) / 2f;

        Transform[] transforms = gameObject.GetComponentsInChildren<Transform>();

        foreach (Transform childTransform in transforms)
        {
            if (childTransform != null)
            {
                childTransform.GetComponent<RectTransform>().sizeDelta = childTransform.GetComponent<RectTransform>().sizeDelta * avgRatio;

                Vector3 newPosition
                    = childTransform.transform.parent.InverseTransformPoint(childTransform.GetComponent<RectTransform>().position) * avgRatio;
                childTransform.GetComponent<RectTransform>().position = childTransform.transform.parent.TransformPoint(newPosition);

                Button button = childTransform.GetComponent<Button>();
                TMP_Text text = childTransform.GetComponentInChildren<TMP_Text>();

                if (button != null && text != null)
                {
                    text.fontSize = (int)(text.fontSize * avgRatio);
                }
            }
        }
    }
}
