using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class show : MonoBehaviour
{
    public TextMeshProUGUI scoreText; // °Ñ¦Ò TextMeshPro ¤¸¯À
    public GameManager p;

    void Start()
    {
        UpdateScoreText();
        scoreText.alignment = TextAlignmentOptions.TopLeft;
    }

    public void UpdateScoreText()
    {
        scoreText.text = "Height: " + (int)(p.transform.position.y - 2);
    }
}
