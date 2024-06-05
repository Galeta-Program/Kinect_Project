using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ChunRiAnimation : MonoBehaviour
{
    public ChunRiAnimationEvents playerAEs;
    public ControllerWithKinect controllerWithKinect;
    public ControllerWithAI controllerWithAI;
    public GameUIControl gameUIControl;
    public GameManagerSF gameManager;
    public Camera gameCamera;
    public Animator animator;
    public bool haveController;
    public bool canBeHurt = true;
    public float speed = 1;
    public float speedBonus = 1;
    public bool useAI;

    public bool groundCheck = false;
    public bool jumpingCheck = false;
    public bool fallingCheck = false;  
    public bool jumptofallcheck = false;

    private bool isIdle = true;
    private bool isSquatDwon = true;
    private bool isAttacking = false;
    private bool isDefensing = false;
    private float gravity = 9.8f;
    private string playingTrigger = "";
    private Timer playingTriggerTimer;
    private bool isOpponentOnRight;
    private Timer turnAroundTimer;
    private Timer knockedDownTimer;
    private Timer knockedDownCountResetTimer;
    private Timer getUpTimer;
    private Timer ultimateTimer;
    private Timer ultimateJumpTimer;
    private int knockedDownCount = 0;
    private bool isReset = false;

    public PauseEvent pauseEvent;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        playingTriggerTimer = new Timer();
        turnAroundTimer = new Timer();
        knockedDownTimer = new Timer();
        getUpTimer = new Timer();
        ultimateTimer = new Timer();
        knockedDownCountResetTimer = new Timer();
        ultimateJumpTimer = new Timer();
        isOpponentOnRight = gameManager.GetOpponent(transform.parent.tag).transform.position.x - 0.1 > transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {      
        isIdle = true;
        isSquatDwon = false;

        if (isAttacking && playingTriggerTimer.isTimeOut())
        {
            isAttacking = false;
        }

        if (gameManager.whoWinRound != 0)
        {
            if (!isReset)
            {
                animator.ResetTrigger(playingTrigger);
                playingTrigger = "";
                animator.Play("ChunRi_idle1");
                isReset = true;
            }

            if (transform.parent.tag == "Player1" && playingTriggerTimer.isTimeOut())
            {
                switch (gameManager.whoWinRound)
                {
                    case 1:
                        playingTrigger = "Win";
                        playingTriggerTimer.Start(3f);
                        break;
                    case 2:
                        playingTrigger = "Lose";
                        playingTriggerTimer.Start(3f);
                        break;
                    case 3:
                        playingTrigger = "Win";
                        playingTriggerTimer.Start(3f);
                        break;
                }
            }
            else if (transform.parent.tag == "Player2" && playingTriggerTimer.isTimeOut())
            {
                switch (gameManager.whoWinRound)
                {
                    case 1:
                        playingTrigger = "Lose";
                        playingTriggerTimer.Start(3f);
                        break;
                    case 2:
                        playingTrigger = "Win";
                        playingTriggerTimer.Start(3f);
                        break;
                    case 3:
                        playingTrigger = "Win";
                        playingTriggerTimer.Start(3f);
                        break;
                }
            }
        }
        else
        {
            isReset = false;
        }


        isDefensing = false;
        //canBeHurt = playingTrigger != "Dodge" && playingTrigger != "SDHurt";
        canBeHurt = true;

        Dictionary<KeyCodeSF, bool> keyCodeIsTrigger = useAI ? controllerWithAI.keyCodeIsTrigger : GetKeyCodesIsTrigger(transform.parent.tag);

        // test
        //if (!haveController)
        //{
        //    if (playingTriggerTimer.isTimeOut())
        //    {
        //        playingTrigger = "SquatDown";
        //        playingTriggerTimer.Start(0.1f);
        //    }

        //    isSquatDwon = true;
        //    isIdle = false;
        //}

        bool blockStun = (playingTrigger == "Dodge"       && !playingTriggerTimer.isTimeOut()) ||
                         (playingTrigger == "SDHurt"      && !playingTriggerTimer.isTimeOut()) ||
                         (playingTrigger == "KnockedDown" && !playingTriggerTimer.isTimeOut());

        if (haveController && knockedDownTimer.isTimeOut() && ultimateTimer.isTimeOut() && gameManager.whoWinRound == 0 && !blockStun)
        {
            if (keyCodeIsTrigger[KeyCodeSF.SquatDown] && (keyCodeIsTrigger[KeyCodeSF.Forward] || keyCodeIsTrigger[KeyCodeSF.Backward]) && keyCodeIsTrigger[KeyCodeSF.HighPunch] && groundCheck && playingTrigger == "SquatDown")
            {
                if (transform.parent.tag == "Player1" && gameUIControl.player1_QigongNum == 3)
                {
                    playingTrigger = "Ultimate3";
                    playingTriggerTimer.Start(6f);
                    ultimateTimer.Start(6f);
                    isIdle = false;
                    gameUIControl.player1_QigongNum = 0;
                    gameUIControl.player1_QigongPercent = 0;
                    speedBonus = 0;
                }
                else if (transform.parent.tag == "Player2" && gameUIControl.player2_QigongNum == 3)
                {
                    playingTrigger = "Ultimate3";
                    playingTriggerTimer.Start(6f);
                    ultimateTimer.Start(6f);
                    isIdle = false;
                    gameUIControl.player2_QigongNum = 0;
                    gameUIControl.player2_QigongPercent = 0;
                    speedBonus = 0;
                }

                isAttacking = true;
            }
            else if (keyCodeIsTrigger[KeyCodeSF.SquatDown] && keyCodeIsTrigger[KeyCodeSF.Jump] && keyCodeIsTrigger[KeyCodeSF.HighKick] && groundCheck && playingTrigger == "SquatDown")
            {
                if (transform.parent.tag == "Player1" && gameUIControl.player1_QigongNum >= 2)
                {
                    playingTrigger = "Ultimate2";
                    playingTriggerTimer.Start(4f);
                    ultimateTimer.Start(4f);
                    isIdle = false;
                    gameUIControl.player1_QigongNum -= 2;
                    gameUIControl.player1_QigongPercent = 0;
                    speedBonus = 0.5f;
                }
                else if (transform.parent.tag == "Player2" && gameUIControl.player2_QigongNum >= 2)
                {
                    playingTrigger = "Ultimate2";
                    playingTriggerTimer.Start(4f);
                    ultimateTimer.Start(4f);
                    isIdle = false;
                    gameUIControl.player2_QigongNum -= 2;
                    gameUIControl.player2_QigongPercent = 0;
                    speedBonus = 0.5f;
                }

                isAttacking = true;
            }
            else if (keyCodeIsTrigger[KeyCodeSF.SquatDown] && keyCodeIsTrigger[KeyCodeSF.Jump] && keyCodeIsTrigger[KeyCodeSF.LightKick] && groundCheck && playingTrigger == "SquatDown")
            {
                if (transform.parent.tag == "Player1" && gameUIControl.player1_QigongNum >= 1)
                {
                    playingTrigger = "Ultimate1";
                    playingTriggerTimer.Start(2f);
                    ultimateTimer.Start(2f);
                    isIdle = false;
                    gameUIControl.player1_QigongNum--;
                    gameUIControl.player1_QigongPercent = 0;
                    speedBonus = 1;
                }
                else if (transform.parent.tag == "Player2" && gameUIControl.player2_QigongNum >= 1)
                {
                    playingTrigger = "Ultimate1";
                    playingTriggerTimer.Start(2f);
                    ultimateTimer.Start(2f);
                    isIdle = false;
                    gameUIControl.player2_QigongNum--;
                    gameUIControl.player2_QigongPercent = 0;
                    speedBonus = 1;
                }

                isAttacking = true;
            }
            else if (keyCodeIsTrigger[KeyCodeSF.SquatDown])
            {
                if (playingTriggerTimer.isTimeOut())
                {
                    playingTrigger = "SquatDown";
                    playingTriggerTimer.Start(0.1f);
                    transform.position -= playerAEs.additionalPos;
                    playerAEs.additionalPos = new Vector3(0, 0, 0);
                }
                
                isSquatDwon = true;
                isIdle = false;              
            }

            if (keyCodeIsTrigger[KeyCodeSF.Jump] && !keyCodeIsTrigger[KeyCodeSF.SquatDown] && groundCheck && !isSquatDwon && ultimateTimer.isTimeOut())
            {
                playingTrigger = "JumpUp";
                playingTriggerTimer.Start(0.1f);
                gravity = -9.8f;
                groundCheck = false;
                isIdle = false;
            }
            else if (!groundCheck && (fallingCheck || jumptofallcheck) && playingTriggerTimer.isTimeOut() && knockedDownTimer.isTimeOut() && ultimateTimer.isTimeOut())
            {
                //Debug.Log("Downing");
                playingTrigger = "JumpUp";
                playingTriggerTimer.Start(0.1f);
                isIdle = false;
                transform.position -= playerAEs.additionalPos;
                playerAEs.additionalPos = new Vector3(0, 0, 0);
            }

            bool chackTime = !groundCheck || isSquatDwon || playingTrigger == "Backward" || playingTrigger == "Forward" ? true : playingTriggerTimer.isTimeOut();

            if (keyCodeIsTrigger[KeyCodeSF.Defense] && chackTime)
            {
                if (groundCheck && !isSquatDwon)
                {
                    playingTrigger = "Defense";
                    playingTriggerTimer.Start(0.5f);
                }                
                else if (isSquatDwon && groundCheck)
                {
                    playingTrigger = "SDDefense";
                    playingTriggerTimer.Start(0.5f);
                }

                isDefensing = true;
                isIdle = false;
            }
            else if (keyCodeIsTrigger[KeyCodeSF.LightPunch] && keyCodeIsTrigger[KeyCodeSF.HighPunch])
            {
                if (groundCheck && !isSquatDwon)
                {
                    playingTrigger = "Qigong";
                    playingTriggerTimer.Start(1f);
                }
                //else if (!groundCheck && !isSquatDwon)
                //{
                //    playingTrigger = "JumpLightPunch";
                //    playingTriggerTimer.Start(0.4f);
                //}
                //else if (isSquatDwon && groundCheck)
                //{
                //    playingTrigger = "SDLightPunch";
                //    playingTriggerTimer.Start(0.4f);
                //}

                isAttacking = true;
                isIdle = false;
            }
            else if (keyCodeIsTrigger[KeyCodeSF.LightPunch] && !keyCodeIsTrigger[KeyCodeSF.HighPunch] && chackTime)
            {
                if (groundCheck && !isSquatDwon)
                {
                    playingTrigger = "LightPunch";
                    playingTriggerTimer.Start(0.6f);
                }
                else if (!groundCheck && !isSquatDwon)
                {
                    playingTrigger = "JumpLightPunch";
                    playingTriggerTimer.Start(0.6f);
                }
                else if (isSquatDwon && groundCheck)
                {
                    playingTrigger = "SDLightPunch";
                    playingTriggerTimer.Start(0.4f);
                }

                isAttacking = true;
                isIdle = false;
            }
            else if (keyCodeIsTrigger[KeyCodeSF.HighPunch] && !keyCodeIsTrigger[KeyCodeSF.LightPunch] && chackTime)
            {
                if (groundCheck && !isSquatDwon)
                {
                    playingTrigger = "HighPunch";
                    playingTriggerTimer.Start(0.8f);
                }
                else if (!groundCheck && !isSquatDwon)
                {
                    playingTrigger = "JumpHighPunch";
                    playingTriggerTimer.Start(0.5f);
                }
                else if (isSquatDwon && groundCheck)
                {
                    playingTrigger = "SDHighPunch";
                    playingTriggerTimer.Start(0.4f);
                }

                isAttacking = true;
                isIdle = false;
            }
            else if (keyCodeIsTrigger[KeyCodeSF.LightKick] && chackTime)
            {
                if (groundCheck && !isSquatDwon)
                {
                    playingTrigger = "LightKick";
                    playingTriggerTimer.Start(0.4f);
                }
                else if(!groundCheck && !isSquatDwon)
                {
                    playingTrigger = "JumpLightKick";
                    playingTriggerTimer.Start(0.4f);
                }
                else if (isSquatDwon && groundCheck)
                {
                    playingTrigger = "SDLightKick";
                    playingTriggerTimer.Start(0.4f);
                }

                isAttacking = true;
                isIdle = false;
            }
            else if (keyCodeIsTrigger[KeyCodeSF.HighKick] && chackTime)
            {
                if (groundCheck && !isSquatDwon)
                {
                    playingTrigger = "HighKick";
                    playingTriggerTimer.Start(0.9f);
                }
                else if (!groundCheck && !isSquatDwon)
                {
                    playingTrigger = "JumpHighKick";
                    playingTriggerTimer.Start(1.3f);
                }
                else if (isSquatDwon && groundCheck)
                {
                    playingTrigger = "SDHighKick";
                    playingTriggerTimer.Start(0.5f);
                }

                isAttacking = true;
                isIdle = false;
            }

            if (keyCodeIsTrigger[KeyCodeSF.Backward] && !keyCodeIsTrigger[KeyCodeSF.Forward] && !isSquatDwon && !isAttacking)
            {
                if (groundCheck)
                {
                    animator.StopPlayback();
                    if (transform.localScale.x > 0)
                        playingTrigger = "Backward";
                    else
                        playingTrigger = "Forward";
                    playingTriggerTimer.Start(0.1f);
                }

                bool isCollidOpponent = transform.position.x < gameManager.GetOpponent(transform.parent.tag).transform.position.x + 0.45f &&
                    transform.position.x > gameManager.GetOpponent(transform.parent.tag).transform.position.x &&
                    transform.position.y < gameManager.GetOpponent(transform.parent.tag).transform.position.y + 1f &&
                    transform.position.y > gameManager.GetOpponent(transform.parent.tag).transform.position.y - 1f;

                if (!isCollidOpponent)
                {
                    transform.Translate(-speed * Time.deltaTime, 0, 0);
                }
                else
                {
                    if (transform.localScale.x > 0)
                    {
                        transform.position = new Vector3(gameManager.GetOpponent(transform.parent.tag).transform.position.x - 0.2f, transform.position.y, transform.position.z);
                    }
                    else
                    {
                        transform.position = new Vector3(gameManager.GetOpponent(transform.parent.tag).transform.position.x + 0.5f, transform.position.y, transform.position.z);
                    }
                }

                isIdle = false;
            }

            if (keyCodeIsTrigger[KeyCodeSF.Forward] && !keyCodeIsTrigger[KeyCodeSF.Backward] && !isSquatDwon && !isAttacking)
            {
                if (groundCheck)
                {
                    animator.StopPlayback();
                    if (transform.localScale.x > 0) 
                        playingTrigger = "Forward";
                    else
                        playingTrigger = "Backward";
                    playingTriggerTimer.Start(0.1f);
                }

                bool isCollidOpponent = transform.position.x < gameManager.GetOpponent(transform.parent.tag).transform.position.x &&
                     transform.position.x > gameManager.GetOpponent(transform.parent.tag).transform.position.x - 0.35f &&
                     transform.position.y < gameManager.GetOpponent(transform.parent.tag).transform.position.y + 1f &&
                     transform.position.y > gameManager.GetOpponent(transform.parent.tag).transform.position.y - 1f;

                if (!isCollidOpponent)
                {
                    transform.Translate(speed * Time.deltaTime, 0, 0);
                }
                else
                {
                    if (transform.localScale.x > 0)
                    {
                        transform.position = new Vector3(gameManager.GetOpponent(transform.parent.tag).transform.position.x - 0.4f, transform.position.y, transform.position.z);
                    }
                    else
                    {
                        transform.position = new Vector3(gameManager.GetOpponent(transform.parent.tag).transform.position.x + 0.2f, transform.position.y, transform.position.z);
                    }
                }

                isIdle = false;
            }
        }

        if (isIdle && groundCheck && playingTriggerTimer.isTimeOut() && knockedDownTimer.isTimeOut() && ultimateTimer.isTimeOut())
        {
            animator.ResetTrigger(playingTrigger);
            playingTrigger = "";
            animator.Play("ChunRi_idle1");
            transform.position -= playerAEs.additionalPos;
            playerAEs.additionalPos = new Vector3(0, 0, 0);
        }

        if (isOpponentOnRight != gameManager.GetOpponent(transform.parent.tag).transform.position.x - 0.1 > transform.position.x - playerAEs.additionalPos.x && groundCheck)
        {
            isOpponentOnRight = gameManager.GetOpponent(transform.parent.tag).transform.position.x - 0.1 > transform.position.x - playerAEs.additionalPos.x;

            if (isIdle)
                playingTrigger = "TurnAround";
            else if (isSquatDwon)
                playingTrigger = "SDTurnAround";

            playingTriggerTimer.Start(0.2f);
            turnAroundTimer.Start(0.2f);
        }

        if (turnAroundTimer.isTimeOut() && groundCheck)
        {
            float newScaleX = Mathf.Abs(transform.localScale.x);

            if (gameManager.GetOpponent(transform.parent.tag).transform.position.x - 0.1 <= transform.position.x - playerAEs.additionalPos.x)
            {
                newScaleX = -newScaleX;
            }

            transform.localScale = new Vector3(newScaleX, transform.localScale.y, transform.localScale.z);
        }

        if (!knockedDownTimer.isTimeOut() && !getUpTimer.isTimeOut())
        {
            if (transform.localScale.x > 0)
                transform.Translate(-speed * Time.deltaTime, 0, 0);
            else
                transform.Translate(speed * Time.deltaTime, 0, 0);
        }
        else if (!ultimateTimer.isTimeOut())
        {
            canBeHurt = false;
            if (transform.localScale.x < 0)
            {
                bool isCollidOpponent = transform.position.x + -speed * Time.deltaTime * 2 < gameManager.GetOpponent(transform.parent.tag).transform.position.x + 0.3f &&
                  //transform.position.x + -speed * Time.deltaTime * 2 > opponentAEs.transform.position.x + 0.3f &&
                  transform.position.y < gameManager.GetOpponent(transform.parent.tag).transform.position.y + 1f &&
                  transform.position.y > gameManager.GetOpponent(transform.parent.tag).transform.position.y - 1f;

                if (!isCollidOpponent)
                   transform.Translate(-speed * Time.deltaTime * speedBonus, 0, 0);
            }              
            else
            {
                bool isCollidOpponent = //transform.position.x + speed * Time.deltaTime * 2 < opponentAEs.transform.position.x + 0.4f &&
                  transform.position.x + speed * Time.deltaTime * 2 > gameManager.GetOpponent(transform.parent.tag).transform.position.x - 0.3f && 
                  transform.position.y < gameManager.GetOpponent(transform.parent.tag).transform.position.y + 1f &&
                  transform.position.y > gameManager.GetOpponent(transform.parent.tag).transform.position.y - 1f;

                if (!isCollidOpponent)
                    transform.Translate(speed * Time.deltaTime * speedBonus, 0, 0);
            }
                
        }

        if (knockedDownCountResetTimer.isTimeOut())
        {
            knockedDownCount = 0;
        }

        AnimationTriggerEvent();
        JumpEvent();
        GravityControl();
        Debug.Log(transform.parent.tag + " " + playingTrigger);

        if (playingTrigger != "Ultimate3" && playingTrigger != "Ultimate2")
        {
            Vector3 pos = gameCamera.WorldToViewportPoint(transform.position);
            pos.x = Mathf.Clamp(pos.x, 0.0f, 1.0f);
            pos.y = Mathf.Clamp(pos.y, 0.0f, 1.0f);
            transform.position = gameCamera.ViewportToWorldPoint(pos);
        }      
    }

    void GravityControl()
    {
        gravity = (gravity + 0.2f < 9.8f) ? gravity + 0.2f : 9.8f;

        if (!groundCheck)
        {
            transform.Translate(0, -gravity * 0.8f * Time.deltaTime, 0);

            if (transform.position.y < -2.7)
            {
                transform.position = new Vector3(transform.position.x, -2.7f, transform.position.z);
                groundCheck = true;
                fallingCheck = false;
                jumptofallcheck = false;
                jumpingCheck = false;
                playingTriggerTimer.SetDuration(-1);
            }              
        }
    }

    void AnimationTriggerEvent()
    {
        //Debug.Log(playingTrigger);
        if (!playingTriggerTimer.isTimeOut() && playingTrigger != "")
        {
            animator.SetTrigger(playingTrigger);
        }       
    }

    void JumpEvent()
    {
        if (gravity != 9.8f && !fallingCheck)
        {
            jumpingCheck = true;
            groundCheck = false;
        }

        if (gravity >= -1 && gravity <= 1 && groundCheck == false)
        {
            jumptofallcheck = true;
            jumpingCheck = false;
        }

        if (gravity > 1 && groundCheck == false && jumptofallcheck == true)
        {
            jumpingCheck = false;
            fallingCheck = true;
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "ground")
        {
            groundCheck = true;
            jumpingCheck = false;
            Debug.Log("ground!");
        }
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        AttackCollider collidedObject = null;

        foreach (AttackCollider attackCollider in gameManager.GetOpponentAttackColliders(transform.parent.tag))
        {
            if (attackCollider.boxCollider.GetComponent<Text>().text == collision.gameObject.GetComponent<Text>().text)
            {
                collidedObject = attackCollider;
                break;
            }
        }

        bool isGetUpping = !knockedDownTimer.isTimeOut() && getUpTimer.isTimeOut();

        if (!ultimateTimer.isTimeOut() || playingTrigger == "Defense" || playingTrigger == "SDDefense" || isGetUpping)
        {
            canBeHurt = false;
        }

        if (collidedObject != null)
        {
            float damage = 0;
            float gatherQi = 0;
            int nowKnockedDownCount = 0;

            if (collision.gameObject.tag == "LightPunchCB" && canBeHurt)
            {
                damage = 2;
                gatherQi = 5;
                nowKnockedDownCount = 1;
            }
            else if (collision.gameObject.tag == "HighPunchCB" && canBeHurt)
            {
                damage = 5;
                gatherQi = 10;
                nowKnockedDownCount = 1;
            }
            else if (collision.gameObject.tag == "LightKickCB" && canBeHurt)
            {
                damage = 3;
                gatherQi = 6;
                nowKnockedDownCount = 1;
            }
            else if (collision.gameObject.tag == "HighKickCB" && canBeHurt)
            {
                damage = 7;
                gatherQi = 12;
                nowKnockedDownCount = 1;
            }
            else if (collision.gameObject.tag == "JumpLightPunchCB" && canBeHurt)
            {
                damage = 2;
                gatherQi = 5;
                nowKnockedDownCount = 1;
            }
            else if (collision.gameObject.tag == "JumpHighPunchCB" && canBeHurt)
            {
                damage = 5;
                gatherQi = 10;
                nowKnockedDownCount = 1;
            }
            else if (collision.gameObject.tag == "JumpLightKickCB" && canBeHurt)
            {
                damage = 3;
                gatherQi = 6;
                nowKnockedDownCount = 1;
            }
            else if (collision.gameObject.tag == "JumpHighKickCB" && canBeHurt)
            {
                damage = 7;
                gatherQi = 12;
                nowKnockedDownCount = 1;
            }
            else if (collision.gameObject.tag == "SDLightPunchCB" && canBeHurt)
            {
                damage = 2;
                gatherQi = 5;
                nowKnockedDownCount = 1;
            }
            else if (collision.gameObject.tag == "SDHighPunchCB" && canBeHurt)
            {
                damage = 5;
                gatherQi = 10;
                nowKnockedDownCount = 1;
            }
            else if (collision.gameObject.tag == "SDLightKickCB" && canBeHurt)
            {
                damage = 3;
                gatherQi = 6;
                nowKnockedDownCount = 2;
            }
            else if (collision.gameObject.tag == "SDHighKickCB" && canBeHurt)
            {
                damage = 7;
                gatherQi = 12;
                nowKnockedDownCount = 3;
            }
            else if (collision.gameObject.tag == "CloseLightPunchCB" && canBeHurt)
            {
                damage = 2;
                gatherQi = 5;
                nowKnockedDownCount = 1;
            }
            else if (collision.gameObject.tag == "CloseHighPunchCB" && canBeHurt)
            {
                damage = 5;
                gatherQi = 10;
                nowKnockedDownCount = 1;
            }
            else if (collision.gameObject.tag == "CloseLightKickCB" && canBeHurt)
            {
                damage = 3;
                gatherQi = 6;
                nowKnockedDownCount = 1;
            }
            else if (collision.gameObject.tag == "CloseHighKickCB" && canBeHurt)
            {
                damage = 7;
                gatherQi = 12;
                nowKnockedDownCount = 1;
            }
            else if (collision.gameObject.tag == "QigongCB" && canBeHurt)
            {
                collision.gameObject.GetComponent<Animator>().SetTrigger("Hit");
                damage = 7;
                gatherQi = 12;
                nowKnockedDownCount = 1;
            }
            else if (collision.gameObject.tag == "UppercutCB" && canBeHurt)
            {
                damage = 5;
                gatherQi = 10;
                nowKnockedDownCount = 5;
            }
            else if (collision.gameObject.tag == "Ultimate3CB" && canBeHurt)
            {
                damage = 10;
                gatherQi = 15;
                nowKnockedDownCount = 2;
            }
            else if (collision.gameObject.tag == "Ultimate2CB" && canBeHurt)
            {
                damage = 11;
                gatherQi = 13;
                nowKnockedDownCount = 3;
            }
            else if (collision.gameObject.tag == "Ultimate1CB" && canBeHurt)
            {
                damage = 12;
                gatherQi = 11;
                nowKnockedDownCount = 5;
            }

            if (nowKnockedDownCount != 0)
            {
                knockedDownCount += nowKnockedDownCount;
                knockedDownCountResetTimer.Start(2f);
                Debug.Log(transform.parent.tag + " knockedDownCount: " + knockedDownCount);

                if (knockedDownCount >= 5)
                {
                    knockedDownTimer.Start(1.6f);
                    getUpTimer.Start(0.8f);
                }
            }

            if (!knockedDownTimer.isTimeOut() && !isDefensing && canBeHurt)
            {
                //Debug.Log("Chun Hurt");
                playingTrigger = "KnockedDown";
                playingTriggerTimer.Start(2.5f);
                gravity = -6.5f;
            }
            else if (isSquatDwon && !isDefensing && canBeHurt)
            {
                //Debug.Log("SDHurt");
                playingTrigger = "SDHurt";
                playingTriggerTimer .Start(0.4f);
            }
            else if (!isSquatDwon && !isDefensing && canBeHurt)
            {
                //Debug.Log("Chun Hurt");
                playingTrigger = "Dodge";
                playingTriggerTimer .Start(0.6f);
            }

            if (transform.parent.tag == "Player1" && canBeHurt)
            {
                gameUIControl.player1_HpPercent -= damage;
                gameUIControl.player1_QigongPercent += gatherQi;
                gameUIControl.player2_QigongPercent += gatherQi * 1.5f;
            }
            else if (transform.parent.tag == "Player2" && canBeHurt)
            {
                gameUIControl.player2_HpPercent -= damage;
                gameUIControl.player2_QigongPercent += gatherQi;
                gameUIControl.player1_QigongPercent += gatherQi * 1.5f;
            }


            //pauseEvent.Pause(0.2f);

            if (collision.gameObject.tag != "QigongCB")
            {
                gameManager.GetOpponentAttackColliders(transform.parent.tag).Remove(collidedObject);
                Destroy(collidedObject.boxCollider);
            }
            else
            {
                gameManager.GetOpponentAttackColliders(transform.parent.tag).Remove(collidedObject);
                gameManager.GetOpponentBulletColliders(transform.parent.tag).Add(new KeyValuePair<AttackCollider, Timer>(collidedObject, new Timer(0.2f)));
            }
        }
    }

    Dictionary<KeyCodeSF, bool> GetKeyCodesIsTrigger(string playerTag)
    {
        Dictionary<KeyCodeSF, bool> keyCodeIsTrigger = new Dictionary<KeyCodeSF, bool>() {
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

        if (playerTag == "Player1")
        {
            keyCodeIsTrigger[KeyCodeSF.LightPunch] = Input.GetKey(KeyCode.G) || controllerWithKinect.GetKeyStay(KeyCodeSF.LightPunch);
            keyCodeIsTrigger[KeyCodeSF.HighPunch]  = Input.GetKey(KeyCode.H) || controllerWithKinect.GetKeyStay(KeyCodeSF.HighPunch);
            keyCodeIsTrigger[KeyCodeSF.LightKick]  = Input.GetKey(KeyCode.V) || controllerWithKinect.GetKeyStay(KeyCodeSF.LightKick);
            keyCodeIsTrigger[KeyCodeSF.HighKick]   = Input.GetKey(KeyCode.B) || controllerWithKinect.GetKeyStay(KeyCodeSF.HighKick);
            keyCodeIsTrigger[KeyCodeSF.Defense]    = Input.GetKey(KeyCode.N) || controllerWithKinect.GetKeyStay(KeyCodeSF.Defense);
            keyCodeIsTrigger[KeyCodeSF.Jump]       = Input.GetKey(KeyCode.W) || controllerWithKinect.GetKeyStay(KeyCodeSF.Jump);
            keyCodeIsTrigger[KeyCodeSF.SquatDown]  = Input.GetKey(KeyCode.S) || controllerWithKinect.GetKeyStay(KeyCodeSF.SquatDown);
            keyCodeIsTrigger[KeyCodeSF.Forward]    = Input.GetKey(KeyCode.D) || controllerWithKinect.GetKeyStay(KeyCodeSF.Forward);
            keyCodeIsTrigger[KeyCodeSF.Backward]   = Input.GetKey(KeyCode.A) || controllerWithKinect.GetKeyStay(KeyCodeSF.Backward);
        }
        else if (playerTag == "Player2")
        {
            keyCodeIsTrigger[KeyCodeSF.LightPunch] = Input.GetKey(KeyCode.Keypad4)    || controllerWithKinect.GetKeyStay(KeyCodeSF.LightPunch, 1);
            keyCodeIsTrigger[KeyCodeSF.HighPunch]  = Input.GetKey(KeyCode.Keypad5)    || controllerWithKinect.GetKeyStay(KeyCodeSF.HighPunch,  1);
            keyCodeIsTrigger[KeyCodeSF.LightKick]  = Input.GetKey(KeyCode.Keypad1)    || controllerWithKinect.GetKeyStay(KeyCodeSF.LightKick,  1);
            keyCodeIsTrigger[KeyCodeSF.HighKick]   = Input.GetKey(KeyCode.Keypad2)    || controllerWithKinect.GetKeyStay(KeyCodeSF.HighKick,   1);
            keyCodeIsTrigger[KeyCodeSF.Defense]    = Input.GetKey(KeyCode.Keypad3)    || controllerWithKinect.GetKeyStay(KeyCodeSF.Defense,    1);
            keyCodeIsTrigger[KeyCodeSF.Jump]       = Input.GetKey(KeyCode.UpArrow)    || controllerWithKinect.GetKeyStay(KeyCodeSF.Jump,       1);
            keyCodeIsTrigger[KeyCodeSF.SquatDown]  = Input.GetKey(KeyCode.DownArrow)  || controllerWithKinect.GetKeyStay(KeyCodeSF.SquatDown,  1);
            keyCodeIsTrigger[KeyCodeSF.Forward]    = Input.GetKey(KeyCode.RightArrow) || controllerWithKinect.GetKeyStay(KeyCodeSF.Forward,    1);
            keyCodeIsTrigger[KeyCodeSF.Backward]   = Input.GetKey(KeyCode.LeftArrow)  || controllerWithKinect.GetKeyStay(KeyCodeSF.Backward,   1);
        }

        return keyCodeIsTrigger;
    }
}
