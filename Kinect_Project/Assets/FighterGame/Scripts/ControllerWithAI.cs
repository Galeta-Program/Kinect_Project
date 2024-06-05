using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerWithAI : MonoBehaviour
{
    public GameManagerSF gameManager;  
    public Dictionary<KeyCodeSF, bool> keyCodeIsTrigger;
    private bool wasIdle = false;

    Timer timer;
    Timer idleTimer;
    Timer walkTimer;
    Timer defenseTimer;
    Timer backwardTimer;

    // Start is called before the first frame update
    void Start()
    {
        timer = new Timer(0.4f);
        idleTimer = new Timer(1f);
        walkTimer = new Timer(1f);
        defenseTimer = new Timer(1f);
        backwardTimer = new Timer(0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        keyCodeIsTrigger = new Dictionary<KeyCodeSF, bool>() {
            { KeyCodeSF.LightPunch, false },
            { KeyCodeSF.HighPunch,  false },
            { KeyCodeSF.LightKick,  false },
            { KeyCodeSF.HighKick,   false },
            { KeyCodeSF.Jump,       false },
            { KeyCodeSF.SquatDown,  false },
            { KeyCodeSF.Forward,    false },
            { KeyCodeSF.Backward,   false },
            { KeyCodeSF.Defense,    false },
        };

        if (!idleTimer.isTimeOut())
        {
            return;
        }

        if (!walkTimer.isTimeOut())
        {
            if (transform.localScale.x > 0)
                keyCodeIsTrigger[KeyCodeSF.Forward] = true;
            else if (transform.localScale.x < 0)
                keyCodeIsTrigger[KeyCodeSF.Backward] = true;

            timer = new Timer(0.2f);
            return;
        }

        if (!defenseTimer.isTimeOut())
        {
            if (GetProbabilityResult(0.3))
                keyCodeIsTrigger[KeyCodeSF.SquatDown] = true;

            keyCodeIsTrigger[KeyCodeSF.Defense] = true;
            return;
        }

        if (!backwardTimer.isTimeOut())
        {
            if (transform.localScale.x > 0)
                keyCodeIsTrigger[KeyCodeSF.Backward] = true;
            else if (transform.localScale.x < 0)
                keyCodeIsTrigger[KeyCodeSF.Forward] = true;

            timer = new Timer(0.2f);
            return;
        }

        int qigongNum = 0;

        if (transform.parent.tag == "Player1")
        {
            qigongNum = gameManager.gameUIControl.player1_QigongNum;
        }
        else if (transform.parent.tag == "Player2")
        {
            qigongNum = gameManager.gameUIControl.player1_QigongNum;
        }

        if (GetProbabilityResult(0.2))
        {
            idleTimer.Start();
        }
        else if (GetProbabilityResult(0.15))
        {           
            defenseTimer.Start();
        }
        else if (GetProbabilityResult(0.18))
        {
            backwardTimer.Start();
        }
        else if (qigongNum >= 2 && GetProbabilityResult(0.5))
        {
            if (GetProbabilityResult(0.3))
            {
                keyCodeIsTrigger[KeyCodeSF.SquatDown] = true;
                keyCodeIsTrigger[KeyCodeSF.Jump] = true;
                keyCodeIsTrigger[KeyCodeSF.LightKick] = true;
                timer = new Timer(1f);
            }
            else if (GetProbabilityResult(0.3))
            {
                keyCodeIsTrigger[KeyCodeSF.SquatDown] = true;
                keyCodeIsTrigger[KeyCodeSF.Forward] = true;
                keyCodeIsTrigger[KeyCodeSF.HighPunch] = true;
                timer = new Timer(1f);
            }
            else
            {
                keyCodeIsTrigger[KeyCodeSF.SquatDown] = true;
                keyCodeIsTrigger[KeyCodeSF.Jump] = true;
                keyCodeIsTrigger[KeyCodeSF.HighKick] = true;
                timer = new Timer(1f);
            }
        }
        else
        {
            if (Vector3.Distance(gameManager.GetOpponent(transform.parent.tag).transform.position, transform.position) < 0.5 && timer.isTimeOut())
            {
                if (GetProbabilityResult(0.2))
                    keyCodeIsTrigger[KeyCodeSF.SquatDown] = true;
                else if (GetProbabilityResult(0.1))
                    keyCodeIsTrigger[KeyCodeSF.Jump] = true;

                if (GetProbabilityResult(0.2))
                {
                    keyCodeIsTrigger[KeyCodeSF.HighPunch] = true;
                    keyCodeIsTrigger[KeyCodeSF.LightPunch] = true;
                    timer = new Timer(1f);
                }
                else
                {
                    int whichAttack = Random.Range(0, 4);
                    keyCodeIsTrigger[(KeyCodeSF)whichAttack] = true;
                    timer = new Timer(1f);
                }                    
            }
            else if (Vector3.Distance(gameManager.GetOpponent(transform.parent.tag).transform.position, transform.position) > 0.5)
            {
                if (GetProbabilityResult(0.5))
                {
                    walkTimer.Start();
                }
                else
                {
                    keyCodeIsTrigger[KeyCodeSF.HighPunch] = true;
                    keyCodeIsTrigger[KeyCodeSF.LightPunch] = true;
                    timer = new Timer(1f);
                }
            }
        }
    }

    bool GetProbabilityResult (double probability)
    {
        if (probability >= 1)
        {
            return true;
        }
        else if (probability <= 0)
        {
            return false;
        }

        bool[] pool = new bool[1000];
        long trueNum = (long)(probability * 1000);
        long falseNum = 1000 - trueNum;
     
        for (long i = 0; i < 1000; i++)
        {
            int rand = Random.Range(0, 2);

            if (trueNum <= 0)
            {
                pool[i] = false;
                falseNum--;
                continue;
            }
            else if (falseNum <= 0)
            {
                pool[i] = false;
                trueNum--;
                continue;
            }

            if (rand == 0)
            {
                pool[i] = false;
                trueNum--;
            }
            else if (rand == 1)
            {
                pool[i] = true;
                falseNum--;
            }   
        }

        return pool[Random.Range(0, 1000)];
    }
}
