using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterSF
{
    Ryu,
    ChunRi,

}

public class GameManagerSF : MonoBehaviour
{
    public GameObject uiInterface;
    public GameObject mainInterface;

    public GameUIControl gameUIControl;
    private bool isGameRunning;
    private bool alreadyPlayBeginning;
    private Timer oneSecTimer;
    private Timer gameOverTimer;
    private Timer[] hpTimers;
    private Timer[] hpReplyTimers;
    private Timer waitTimer;
    private Timer roundTextTimer;
    private Timer fightTextTimer;
    private bool alreadyWaited;
    private bool alreadyGameOver;
    public GameObject[] player1s;
    public GameObject[] player2s;
    public CharacterSF player1_character;
    public CharacterSF player2_character;
    public bool isPlayer2UseAI;
    private Vector3 roundTextOriPos;
    private List<Vector3> player1sOriPos = new List<Vector3>();
    private List<Vector3> player2sOriPos = new List<Vector3>();
    public int whoWinRound = 0; // 贏得這局 0 = 沒人, 1 = player1, 2 = player2, 3 = 平手
    private int whoWinGame  = 0; // 贏得這場 0 = 沒人, 1 = player1, 2 = player2, 3 = 平手

    // Start is called before the first frame update
    void Awake()
    {
        isGameRunning = false;
        alreadyGameOver = false;
        isPlayer2UseAI = true;
        oneSecTimer = new Timer(1f);
        hpTimers = new Timer[2];
        hpReplyTimers = new Timer[2];
        waitTimer = new Timer(3f);
        gameOverTimer = new Timer(3f);
        roundTextTimer = new Timer(3f);
        fightTextTimer = new Timer(0.8f);
        roundTextOriPos = gameUIControl.gameRoundText.transform.position;
        gameUIControl.gameRoundText.gameObject.SetActive(false);
        gameUIControl.fightText.gameObject.SetActive(false);

        foreach (GameObject player in player1s)
        {
            player1sOriPos.Add(player.transform.position);
        }

        foreach (GameObject player in player2s)
        {
            player2sOriPos.Add(player.transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameRunning)
        {
            GameUpdata();
        }
        else
        {
            if (gameOverTimer.isTimeOut() && alreadyGameOver)
            {
                uiInterface.SetActive(true);
                mainInterface.SetActive(false);
            }
        }       
    }

    public void GameStart()
    {
        isGameRunning = true;
        GameSet();
    }

    void GameUpdata()
    {
        if (!alreadyPlayBeginning)
        {
            if (gameUIControl.gameRoundText.gameObject.activeSelf == true)
            {
                if (!roundTextTimer.isTimeOut())
                    gameUIControl.gameRoundText.transform.position = Vector3.Lerp(gameUIControl.gameRoundText.transform.position, roundTextOriPos, 1.8f * Time.deltaTime);
                else
                {
                    gameUIControl.gameRoundText.gameObject.SetActive(false);                                
                    oneSecTimer.Start(0.3f);
                }
            }           
            else if (oneSecTimer.isTimeOut())
            {
                gameUIControl.fightText.gameObject.SetActive(true);
                oneSecTimer.Start(10f);
                fightTextTimer.Start();
            }

            if (gameUIControl.fightText.gameObject.activeSelf == true && fightTextTimer.isTimeOut())
            {
                gameUIControl.fightText.gameObject.SetActive(false);
                alreadyPlayBeginning = true;
                oneSecTimer.Start(1f);

                for (int i = 0; i < player1s.Length; i++)
                {
                    SetHaveController(player1s[i].transform.parent.tag, true);
                }

                for (int i = 0; i < player2s.Length; i++)
                {
                    SetHaveController(player2s[i].transform.parent.tag, true);
                }
            }

            return;
        }

        if (waitTimer.isTimeOut())
        {
            if (oneSecTimer.isTimeOut())
            {
                if (gameUIControl.gameTimer - 1 < 0)
                {
                    if (gameUIControl.player1_HpPercent == gameUIControl.player2_HpPercent)
                    {
                        whoWinRound = 3;
                    }
                    else if (gameUIControl.player1_HpPercent < gameUIControl.player2_HpPercent)
                    {
                        whoWinRound = 2;
                    }
                    else if (gameUIControl.player1_HpPercent > gameUIControl.player2_HpPercent)
                    {
                        whoWinRound = 1;
                    }

                    if (!alreadyWaited)
                    {
                        waitTimer.Start();
                        alreadyWaited = true;
                        return;
                    }
                }
                else if (gameUIControl.gameTimer <= 99)
                {
                    gameUIControl.gameTimer--;
                    oneSecTimer.Start();
                }
            }

            if (gameUIControl.player1_HpPercent <= 0 && gameUIControl.player2_HpPercent <= 0)
            {
                whoWinRound = 3;

                if (!alreadyWaited)
                {
                    waitTimer.Start();
                    alreadyWaited = true;
                    return;
                }
            }
            else if (gameUIControl.player1_HpPercent <= 0)
            {
                whoWinRound = 2;

                if (!alreadyWaited)
                {
                    waitTimer.Start();
                    alreadyWaited = true;
                    return;
                }
            }
            else if (gameUIControl.player2_HpPercent <= 0)
            {
                whoWinRound = 1;

                if (!alreadyWaited)
                {
                    waitTimer.Start();
                    alreadyWaited = true;
                    return;
                }
            }

            if (whoWinRound > 0)
            {
                switch (whoWinRound)
                {
                    case 1:
                        gameUIControl.player1_WinTime++;

                        if (gameUIControl.player1_WinTime >= 3)
                        {
                            whoWinGame = 1;
                            gameUIControl.whoWinGame = 1;
                            isGameRunning = false;
                            alreadyGameOver = true;
                            gameOverTimer.Start();
                            return;
                        }
                        break;

                    case 2:
                        gameUIControl.player2_WinTime++;

                        if (gameUIControl.player2_WinTime >= 3)
                        {
                            whoWinGame = 2;
                            gameUIControl.whoWinGame = 2;
                            isGameRunning = false;
                            alreadyGameOver = true;
                            gameOverTimer.Start();
                            return;
                        }
                        break;

                    case 3:
                        gameUIControl.player1_WinTime++;
                        gameUIControl.player2_WinTime++;

                        if (gameUIControl.player1_WinTime >= 3 && gameUIControl.player2_WinTime >= 3)
                        {
                            whoWinGame = 3;
                            gameUIControl.whoWinGame = 3;
                            isGameRunning = false;
                            alreadyGameOver = true;
                            gameOverTimer.Start();
                            return;
                        }
                        else if (gameUIControl.player2_WinTime >= 3)
                        {
                            whoWinGame = 2;
                            gameUIControl.whoWinGame = 2;
                            isGameRunning = false;
                            alreadyGameOver = true;
                            gameOverTimer.Start();
                            return;
                        }
                        else if (gameUIControl.player1_WinTime >= 3)
                        {
                            whoWinGame = 1;
                            gameUIControl.whoWinGame = 1;
                            isGameRunning = false;
                            alreadyGameOver = true;
                            gameOverTimer.Start();
                            return;
                        }
                        break;
                    default:
                        Debug.LogError("Invaild Value whoWinRound: " + whoWinRound);
                        break;
                }

                GameRoundSet();
                return;
            }

            if (hpTimers[0].isTimeOut())
            {
                if (gameUIControl.player1_HpPercent < gameUIControl.player1_HpReplyPercent)
                {
                    gameUIControl.player1_HpPercent += 1f;
                }

                hpTimers[0].Start(0.5f);
            }

            if (hpReplyTimers[0].isTimeOut())
            {
                if (gameUIControl.player1_HpPercent < gameUIControl.player1_HpReplyPercent)
                {
                    gameUIControl.player1_HpReplyPercent -= 1f;
                }
                else if (gameUIControl.player1_HpPercent > gameUIControl.player1_HpReplyPercent)
                {
                    gameUIControl.player1_HpReplyPercent = gameUIControl.player1_HpPercent;
                }

                hpReplyTimers[0].Start(0.2f);
            }

            if (hpTimers[1].isTimeOut())
            {
                if (gameUIControl.player2_HpPercent < gameUIControl.player2_HpReplyPercent)
                {
                    gameUIControl.player2_HpPercent += 1f;
                }

                hpTimers[1].Start(0.5f);
            }

            if (hpReplyTimers[1].isTimeOut())
            {
                if (gameUIControl.player2_HpPercent < gameUIControl.player2_HpReplyPercent)
                {
                    gameUIControl.player2_HpReplyPercent -= 1f;
                }
                else if (gameUIControl.player2_HpPercent > gameUIControl.player2_HpReplyPercent)
                {
                    gameUIControl.player2_HpReplyPercent = gameUIControl.player2_HpPercent;
                }

                hpReplyTimers[1].Start(0.2f);
            }

            if (gameUIControl.player1_QigongNum < 3 && gameUIControl.player1_QigongPercent >= 100)
            {
                gameUIControl.player1_QigongNum++;
                gameUIControl.player1_QigongPercent %= 100.0f;
            }

            if (gameUIControl.player2_QigongNum < 3 && gameUIControl.player2_QigongPercent >= 100)
            {
                gameUIControl.player2_QigongNum++;
                gameUIControl.player2_QigongPercent %= 100.0f;
            }
        }
    }

    void GameSet()
    {       
        gameUIControl.player1_WinTime = 0;
        gameUIControl.player2_WinTime = 0;
        gameUIControl.player1_QigongPercent = 0f;
        gameUIControl.player2_QigongPercent = 0f;
        gameUIControl.player1_QigongNum = 0;
        gameUIControl.player2_QigongNum = 0;
        gameUIControl.gameRound = 0;
        gameUIControl.whoWinGame = 0;
        gameUIControl.backgroundIndex = Random.Range(0, gameUIControl.backgroundSprites.Length);
        whoWinGame = 0;
        alreadyGameOver = false;

        foreach (GameObject player in player1s)
        {
            player.SetActive(false);
        }

        player1s[(int)player1_character].SetActive(true);

        foreach (GameObject player in player2s)
        {
            player.SetActive(false);
        }

        gameUIControl.player1_character = player1_character;
        gameUIControl.player2_character = player2_character;

        player2s[(int)player2_character].SetActive(true);
        SetUseAI(player2s[(int)player2_character].transform.parent.tag, isPlayer2UseAI);

        GameRoundSet();
    }

    void GameRoundSet()
    {
        gameUIControl.gameTimer = 100;
        gameUIControl.player1_HpPercent = 100.0f;
        gameUIControl.player2_HpPercent = 100.0f;
        gameUIControl.player1_HpReplyPercent = 100.0f;
        gameUIControl.player2_HpReplyPercent = 100.0f;       
        oneSecTimer = new Timer(1f);
        hpTimers = new Timer[2] { new Timer(0.4f), new Timer(0.4f) };
        hpReplyTimers = new Timer[2] { new Timer(0.1f), new Timer(0.1f) };
        whoWinRound = 0;
        alreadyWaited = false;
        alreadyPlayBeginning = false;
        gameUIControl.gameRound++;
        gameUIControl.gameRoundText.gameObject.SetActive(true);
        gameUIControl.gameRoundText.transform.position = new Vector3(roundTextOriPos.x + 3, roundTextOriPos.y, roundTextOriPos.z);
        roundTextTimer = new Timer(3f);

        for (int i = 0; i < player1s.Length; i++)
        {
            SetHaveController(player1s[i].transform.parent.tag, false);
            SetGroundCheck(player1s[i].transform.parent.tag, false);
            player1s[i].transform.position = player1sOriPos[i];
        }

        for (int i = 0; i < player2s.Length; i++)
        {
            SetHaveController(player2s[i].transform.parent.tag, false);
            SetGroundCheck(player2s[i].transform.parent.tag, false);
            player2s[i].transform.position = player2sOriPos[i];
        }
    }

    public GameObject GetOpponent(string selfTag)
    {
        if (selfTag == "Player1")
        {
            switch (player2_character)
            {
                case CharacterSF.Ryu:
                    return player2s[0];
                case CharacterSF.ChunRi:
                    return player2s[1];
                default:
                    return new GameObject();
            }
        }
        else if (selfTag == "Player2")
        {
            switch (player1_character)
            {
                case CharacterSF.Ryu:
                    return player1s[0];
                case CharacterSF.ChunRi:
                    return player1s[1];
                default:
                    return new GameObject();
            }
        }
        else
            return new GameObject();
    }

    public List<AttackCollider> GetOpponentAttackColliders(string selfTag)
    {
        if (selfTag == "Player1")
        {
            switch (player2_character)
            {
                case CharacterSF.Ryu:
                    return player2s[0].GetComponent<RyuAnimationEvents>().attackColliders;
                case CharacterSF.ChunRi:
                    return player2s[1].GetComponent<ChunRiAnimationEvents>().attackColliders;
                default:
                    return new List<AttackCollider>();
            }
        }
        else if (selfTag == "Player2")
        {
            switch (player1_character)
            {
                case CharacterSF.Ryu:
                    return player1s[0].GetComponent<RyuAnimationEvents>().attackColliders;
                case CharacterSF.ChunRi:
                    return player1s[1].GetComponent<ChunRiAnimationEvents>().attackColliders;
                default:
                    return new List<AttackCollider>();
            }
        }
        else
            return new List<AttackCollider>();
    }

    public List<KeyValuePair<AttackCollider, Timer>> GetOpponentBulletColliders(string selfTag)
    {
        if (selfTag == "Player1")
        {
            switch (player2_character)
            {
                case CharacterSF.Ryu:
                    return player2s[0].GetComponent<RyuAnimationEvents>().bulletColliders;
                case CharacterSF.ChunRi:
                    return player2s[1].GetComponent<ChunRiAnimationEvents>().bulletColliders;
                default:
                    return new List<KeyValuePair<AttackCollider, Timer>>();
            }
        }
        else if (selfTag == "Player2")
        {
            switch (player1_character)
            {
                case CharacterSF.Ryu:
                    return player1s[0].GetComponent<RyuAnimationEvents>().bulletColliders;
                case CharacterSF.ChunRi:
                    return player1s[1].GetComponent<ChunRiAnimationEvents>().bulletColliders;
                default:
                    return new List<KeyValuePair<AttackCollider, Timer>>();
            }
        }
        else
            return new List<KeyValuePair<AttackCollider, Timer>>();
    }

    public bool SetHaveController(string selfTag, bool setValue)
    {
        if (selfTag == "Player1")
        {
            switch (player1_character)
            {
                case CharacterSF.Ryu:
                    player1s[0].GetComponent<RyuAnimation>().haveController = setValue;
                    break;
                case CharacterSF.ChunRi:
                    player1s[1].GetComponent<ChunRiAnimation>().haveController = setValue;
                    break;
                default:
                    return false;
            }
        }
        else if (selfTag == "Player2")
        {
            switch (player2_character)
            {
                case CharacterSF.Ryu:
                    player2s[0].GetComponent<RyuAnimation>().haveController = setValue;
                    break;
                case CharacterSF.ChunRi:
                    player2s[1].GetComponent<ChunRiAnimation>().haveController = setValue;
                    break;
                default:
                    return false;
            }
        }
        else
            return false;

        return true;
    }

    public bool SetGroundCheck(string selfTag, bool setValue)
    {
        if (selfTag == "Player1")
        {
            switch (player1_character)
            {
                case CharacterSF.Ryu:
                    player1s[0].GetComponent<RyuAnimation>().groundCheck = setValue;
                    break;
                case CharacterSF.ChunRi:
                    player1s[1].GetComponent<ChunRiAnimation>().groundCheck = setValue;
                    break;
                default:
                    return false;
            }
        }
        else if (selfTag == "Player2")
        {
            switch (player2_character)
            {
                case CharacterSF.Ryu:
                    player2s[0].GetComponent<RyuAnimation>().groundCheck = setValue;
                    break;
                case CharacterSF.ChunRi:
                    player2s[1].GetComponent<ChunRiAnimation>().groundCheck = setValue;
                    break;
                default:
                    return false;
            }
        }
        else
            return false;

        return true;
    }

    public bool SetUseAI(string selfTag, bool setValue)
    {
        if (selfTag == "Player1")
        {
            switch (player1_character)
            {
                case CharacterSF.Ryu:
                    player1s[0].GetComponent<RyuAnimation>().useAI = setValue;
                    break;
                case CharacterSF.ChunRi:
                    player1s[1].GetComponent<ChunRiAnimation>().useAI = setValue;
                    break;
                default:
                    return false;
            }
        }
        else if (selfTag == "Player2")
        {
            switch (player2_character)
            {
                case CharacterSF.Ryu:
                    player2s[0].GetComponent<RyuAnimation>().useAI = setValue;
                    break;
                case CharacterSF.ChunRi:
                    player2s[1].GetComponent<ChunRiAnimation>().useAI = setValue;
                    break;
                default:
                    return false;
            }
        }
        else
            return false;

        return true;
    }
}
