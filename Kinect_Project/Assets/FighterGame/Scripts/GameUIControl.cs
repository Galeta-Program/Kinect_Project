using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUIControl : MonoBehaviour
{
    public Sprite[] winBarSprites;
    public Sprite[] roundTextSprites;
    public Sprite[] roleWinSprites;
    public Sprite[] roleLoseSprites;
    public Sprite[] backgroundSprites;
    public CharacterSF player1_character;
    public CharacterSF player2_character;
    public Image player1_WinLoseImage;
    public Image player2_WinLoseImage;
    public Image gameOverScene;
    public Image player1_HpBar;
    public Image player2_HpBar;
    public Image player1_HpReplyBar;
    public Image player2_HpReplyBar;
    public Image player1_WinBar;
    public Image player2_WinBar;
    public Image player1_QigongBar;
    public Image player2_QigongBar;
    public Image fightText;
    public Image gameRoundText;
    public Image background;
    public TMP_Text player1_QigongNumStr;
    public TMP_Text player2_QigongNumStr;
    public TMP_Text gameTimerStr;

    [Range(0, 100)]
    public int gameTimer = 100;

    [Range(0.0f, 100.0f)]
    public float player1_HpPercent = 100f;

    [Range(0.0f, 100.0f)]
    public float player2_HpPercent = 100f;

    [Range(0.0f, 100.0f)]
    public float player1_HpReplyPercent = 100f;

    [Range(0.0f, 100.0f)]
    public float player2_HpReplyPercent = 100f;

    [Range(0, 3)]
    public int player1_WinTime = 0;

    [Range(0, 3)]
    public int player2_WinTime = 0;

    [Range(0.0f, 125.0f)]
    public float player1_QigongPercent = 0f;

    [Range(0.0f, 125.0f)]
    public float player2_QigongPercent = 0f;

    [Range(0, 3)]
    public int player1_QigongNum = 0;

    [Range(0, 3)]
    public int player2_QigongNum = 0;

    [Range(1, 5)]
    public int gameRound = 1;

    [Range(0, 3)]
    public int whoWinGame = 0; // Ĺ�o�o�� 0 = �S�H, 1 = player1, 2 = player2, 3 = ����

    public int backgroundIndex = -1;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (gameTimer > 99)
        {
            gameTimerStr.text = "∞";
            gameTimerStr.fontSize = 120;
        }           
        else
        {
            gameTimerStr.text = "" + gameTimer;
            gameTimerStr.fontSize = 90;
        }        

        player1_HpBar.fillAmount = player1_HpPercent / 100.0f;
        player2_HpBar.fillAmount = player2_HpPercent / 100.0f;
        player1_HpReplyBar.fillAmount = player1_HpReplyPercent / 100.0f;
        player2_HpReplyBar.fillAmount = player2_HpReplyPercent / 100.0f;
        player1_WinBar.sprite = winBarSprites[player1_WinTime];
        player2_WinBar.sprite = winBarSprites[player2_WinTime];
        player1_QigongBar.fillAmount = player1_QigongPercent / 100.0f;
        player2_QigongBar.fillAmount = player2_QigongPercent / 100.0f;
        player1_QigongNumStr.text = "" + player1_QigongNum;
        player2_QigongNumStr.text = "" + player2_QigongNum;
        gameRoundText.sprite = roundTextSprites[gameRound - 1];

        if (player1_HpBar.fillAmount > 0.4)
        {
            player1_HpBar.color = Color.green;
        }
        else if (player1_HpBar.fillAmount < 0.4 && player1_HpBar.fillAmount > 0.2)
        {
            player1_HpBar.color = new Color(255, 107, 0);
        }
        else
        {
            player1_HpBar.color = Color.red;
        }

        if (player2_HpBar.fillAmount > 0.4)
        {
            player2_HpBar.color = Color.green;
        }
        else if (player2_HpBar.fillAmount < 0.4 && player2_HpBar.fillAmount > 0.2)
        {
            player2_HpBar.color = new Color(255, 107, 0);
        }
        else
        {
            player2_HpBar.color = Color.red;
        }

        switch (whoWinGame)
        {
            case 0:
                gameOverScene.gameObject.SetActive(false);
                player1_WinLoseImage.sprite = null;
                player2_WinLoseImage.sprite = null;
                break;
            case 1:
                gameOverScene.gameObject.SetActive(true);
                player1_WinLoseImage.sprite = roleWinSprites[(int)player1_character];
                player2_WinLoseImage.sprite = roleLoseSprites[(int)player2_character];
                break;
            case 2:
                gameOverScene.gameObject.SetActive(true);
                player1_WinLoseImage.sprite = roleLoseSprites[(int)player1_character];
                player2_WinLoseImage.sprite = roleWinSprites[(int)player2_character];
                break;
            case 3:
                gameOverScene.gameObject.SetActive(true);
                player1_WinLoseImage.sprite = roleWinSprites[(int)player1_character];
                player2_WinLoseImage.sprite = roleWinSprites[(int)player2_character];
                break;
        }

        if (backgroundSprites.Length == 0)
        {
            backgroundIndex = -1;
        }
        
        if (backgroundIndex >= backgroundSprites.Length)
        {
            backgroundIndex = backgroundSprites.Length - 1;
        }

        if (backgroundIndex >= 0)
        {
            background.sprite = backgroundSprites[backgroundIndex];
        }
    }
}
