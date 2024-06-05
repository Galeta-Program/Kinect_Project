using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SwordCharacter : MonoBehaviour
{
    [SerializeField] public SwordGameManager gameManager;
    [SerializeField] public SwrodJointsCatcher jointsCatcher;
    [SerializeField] public AudioSource[] audioSources;
    private Animator animator;
    public string[] animatorStatesName = { "idle", "blockIdle", "blockReact", "react", "attackHorizontal", "attackBackhand", "attackDownward", "dizzy", "fall" };
    public string[] animatorParametersName = { "action", "blocking", "blockSuccess", "beingAttack", "attack", "dizzy", "fall" };
    enum ANIMSTATES_NAME
    {
        IDLE = 0,
        BLOCKIDLE,
        BLOCKREACT,
        REACT,
        ATTACKHORIZONTAL,
        ATTACKBACKHAND,
        ATTACKDOWNWARD,
        DIZZY,
        FALL
    };
    enum ANIMPARAM_NAME
    {
        ACTION = 0,
        BLOCKING,
        BLOCKSUCCESS,
        BEINGATTACK,
        ATTACK,
        DIZZY,
        FALL
    }
    public SwordCharacter opposite;
    public int id;
    public bool fall = false;
    public bool isDizzy = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        moveCapture();

        if (math.abs(transform.localPosition.z) > 3.5f) fallHandler();

        if (animator.GetCurrentAnimatorStateInfo(0).IsName(animatorStatesName[(int)ANIMSTATES_NAME.IDLE])) idleHandler();
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(animatorStatesName[(int)ANIMSTATES_NAME.ATTACKHORIZONTAL])) attackHorizontalHandler();
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(animatorStatesName[(int)ANIMSTATES_NAME.ATTACKBACKHAND])) attackBackhandHandler();
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(animatorStatesName[(int)ANIMSTATES_NAME.ATTACKDOWNWARD])) attackDownwardHandler();
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(animatorStatesName[(int)ANIMSTATES_NAME.BLOCKREACT])) blockReactHandler();
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(animatorStatesName[(int)ANIMSTATES_NAME.REACT])) beingAttackHandler();
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(animatorStatesName[(int)ANIMSTATES_NAME.DIZZY])) inDizzy();
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(animatorStatesName[(int)ANIMSTATES_NAME.FALL])) falling();
    }

    public void resetStatus()
    {
        fall = false;
        if(id == 1)
        {
            transform.localPosition = new Vector3(0f, 6.775f, -0.7f);
        }
        else if(id == 0)
        {
            transform.localPosition = new Vector3(0f, 6.775f, 0.7f);
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        animator.SetBool(animatorParametersName[(int)ANIMPARAM_NAME.ACTION], false);
        animator.SetBool(animatorParametersName[(int)ANIMPARAM_NAME.BLOCKING], false);
        animator.SetBool(animatorParametersName[(int)ANIMPARAM_NAME.BLOCKSUCCESS], false);
        animator.SetBool(animatorParametersName[(int)ANIMPARAM_NAME.BEINGATTACK], false);
        animator.SetBool(animatorParametersName[(int)ANIMPARAM_NAME.DIZZY], false);
        animator.SetBool(animatorParametersName[(int)ANIMPARAM_NAME.FALL], false);
        animator.SetInteger(animatorParametersName[(int)ANIMPARAM_NAME.ATTACK], 0);
    }

    void moveCapture()
    {
        if (gameManager.currentMode != gameManager.mode["playing"]) return;

        if (jointsCatcher.jointSpeeds[id].handOpen)
        {
            animator.SetBool(animatorParametersName[(int)ANIMPARAM_NAME.BLOCKING], true);
        }
        else if(animator.GetBool(animatorParametersName[(int)ANIMPARAM_NAME.BLOCKING]))
        {
            animator.SetBool(animatorParametersName[(int)ANIMPARAM_NAME.BLOCKING], false);
            animator.SetBool(animatorParametersName[(int)ANIMPARAM_NAME.BLOCKSUCCESS], false);
        }

        if(jointsCatcher.jointSpeeds[id + 2].speed > 1f)
        {
            if (!(opposite.animator.GetCurrentAnimatorStateInfo(0).IsName(animatorStatesName[(int)ANIMSTATES_NAME.REACT]) ||
                opposite.animator.GetCurrentAnimatorStateInfo(0).IsName(animatorStatesName[(int)ANIMSTATES_NAME.BLOCKREACT])))
            {
                animator.SetBool(animatorParametersName[(int)ANIMPARAM_NAME.ACTION], true);

                int randomAtk = UnityEngine.Random.Range(0, 3);
                animator.SetInteger(animatorParametersName[(int)ANIMPARAM_NAME.ATTACK], randomAtk);
            }
        }

        if (Input.GetKeyDown(KeyCode.Q) && id == 0)
        {
            if (!(opposite.animator.GetCurrentAnimatorStateInfo(0).IsName(animatorStatesName[(int)ANIMSTATES_NAME.REACT]) ||
                opposite.animator.GetCurrentAnimatorStateInfo(0).IsName(animatorStatesName[(int)ANIMSTATES_NAME.BLOCKREACT])))
            {
                animator.SetBool(animatorParametersName[(int)ANIMPARAM_NAME.ACTION], true);
                animator.SetInteger(animatorParametersName[(int)ANIMPARAM_NAME.ATTACK], 0);
            }
        }
        if (Input.GetKeyDown(KeyCode.W) && id == 0)
        {
            if (!(opposite.animator.GetCurrentAnimatorStateInfo(0).IsName(animatorStatesName[(int)ANIMSTATES_NAME.REACT]) ||
                opposite.animator.GetCurrentAnimatorStateInfo(0).IsName(animatorStatesName[(int)ANIMSTATES_NAME.BLOCKREACT])))
            {
                animator.SetBool(animatorParametersName[(int)ANIMPARAM_NAME.ACTION], true);
                animator.SetInteger(animatorParametersName[(int)ANIMPARAM_NAME.ATTACK], 1);
            }
        }
        if (Input.GetKeyDown(KeyCode.E) && id == 0)
        {
            if (!(opposite.animator.GetCurrentAnimatorStateInfo(0).IsName(animatorStatesName[(int)ANIMSTATES_NAME.REACT]) ||
                opposite.animator.GetCurrentAnimatorStateInfo(0).IsName(animatorStatesName[(int)ANIMSTATES_NAME.BLOCKREACT])))
            {
                animator.SetBool(animatorParametersName[(int)ANIMPARAM_NAME.ACTION], true);
                animator.SetInteger(animatorParametersName[(int)ANIMPARAM_NAME.ATTACK], 2);
            }
        }
        if (Input.GetKeyDown(KeyCode.L) && id == 1)
        {
            animator.SetBool(animatorParametersName[(int)ANIMPARAM_NAME.BLOCKING], true);
        }
        if (Input.GetKeyUp(KeyCode.L) && id == 1)
        {
            animator.SetBool(animatorParametersName[(int)ANIMPARAM_NAME.BLOCKING], false);
            animator.SetBool(animatorParametersName[(int)ANIMPARAM_NAME.BLOCKSUCCESS], false);
        }
        if (Input.GetKeyDown(KeyCode.P) && id == 1)
        {
            if (!(opposite.animator.GetCurrentAnimatorStateInfo(0).IsName(animatorStatesName[(int)ANIMSTATES_NAME.REACT]) ||
                opposite.animator.GetCurrentAnimatorStateInfo(0).IsName(animatorStatesName[(int)ANIMSTATES_NAME.BLOCKREACT])))
            {
                animator.SetBool(animatorParametersName[(int)ANIMPARAM_NAME.ACTION], true);
                animator.SetInteger(animatorParametersName[(int)ANIMPARAM_NAME.ATTACK], 0);
            }
        }
    }

    void fallHandler()
    {
        fall = true;
        animator.SetBool(animatorParametersName[(int)ANIMPARAM_NAME.FALL], true);
    }

    void idleHandler()
    {
        
    }

    void attackHorizontalHandler()
    {
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.9f)
            animator.SetBool(animatorParametersName[(int)ANIMPARAM_NAME.ACTION], false);

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.5f)
        {
            if (opposite.animator.GetCurrentAnimatorStateInfo(0).IsName(animatorStatesName[(int)ANIMSTATES_NAME.BLOCKIDLE])) 
                opposite.animator.SetBool(animatorParametersName[(int)ANIMPARAM_NAME.BLOCKSUCCESS], true);
            else opposite.animator.SetBool(animatorParametersName[(int)ANIMPARAM_NAME.BEINGATTACK], true);
        }
    }

    void attackBackhandHandler()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.9f)
            animator.SetBool(animatorParametersName[(int)ANIMPARAM_NAME.ACTION], false);

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.5f)
        {
            if (opposite.animator.GetCurrentAnimatorStateInfo(0).IsName(animatorStatesName[(int)ANIMSTATES_NAME.BLOCKIDLE]))
                opposite.animator.SetBool(animatorParametersName[(int)ANIMPARAM_NAME.BLOCKSUCCESS], true);
            else opposite.animator.SetBool(animatorParametersName[(int)ANIMPARAM_NAME.BEINGATTACK], true);
        }
    }

    void attackDownwardHandler()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.9f)
            animator.SetBool(animatorParametersName[(int)ANIMPARAM_NAME.ACTION], false);

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.5f)
        {
            if (opposite.animator.GetCurrentAnimatorStateInfo(0).IsName(animatorStatesName[(int)ANIMSTATES_NAME.BLOCKIDLE]))
                opposite.animator.SetBool(animatorParametersName[(int)ANIMPARAM_NAME.BLOCKSUCCESS], true);
            else opposite.animator.SetBool(animatorParametersName[(int)ANIMPARAM_NAME.BEINGATTACK], true);
        }
    }

    void blockReactHandler()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.9f)
            animator.SetBool(animatorParametersName[(int)ANIMPARAM_NAME.BLOCKSUCCESS], false);

        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f && !opposite.animator.GetCurrentAnimatorStateInfo(0).IsName(animatorStatesName[(int)ANIMSTATES_NAME.DIZZY]))
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.4f) audioSources[1].Play();
            opposite.animator.SetBool(animatorParametersName[(int)ANIMPARAM_NAME.DIZZY], true);
        }
    }

    void beingAttackHandler()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
            animator.SetBool(animatorParametersName[(int)ANIMPARAM_NAME.BEINGATTACK], false);

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.4f)
        {
            audioSources[0].Play();
            if (isDizzy)
            {
                transform.Translate(Vector3.back * 4.8f * Time.deltaTime);
                if(math.abs(transform.localPosition.z) + (4.8f * Time.deltaTime) < 3.5f)
                    opposite.transform.Translate(Vector3.forward * 4.8f * Time.deltaTime);
            }
            else
            {
                transform.Translate(Vector3.back * 2.4f * Time.deltaTime);
                if (math.abs(transform.localPosition.z) + (2.4 * Time.deltaTime) < 3.5f)
                    opposite.transform.Translate(Vector3.forward * 2.4f * Time.deltaTime);
            }
        }
        else
        {
            isDizzy = false;
        }
    }

    void inDizzy()
    {
        animator.SetBool(animatorParametersName[(int)ANIMPARAM_NAME.DIZZY], false);
        animator.SetBool(animatorParametersName[(int)ANIMPARAM_NAME.ACTION], false);

        isDizzy = true;
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f) isDizzy = false;
    }

    void falling()
    {
        animator.SetBool(animatorParametersName[(int)ANIMPARAM_NAME.FALL], false);

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.5f)
        {
            transform.Translate(Vector3.down * 5f * Time.deltaTime);
            transform.Translate(Vector3.back * 3f * Time.deltaTime);
            opposite.transform.Translate(Vector3.forward * 0.5f * Time.deltaTime);
        }
    }
}
