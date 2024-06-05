using DG.Tweening;
using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class GameManager : MonoBehaviour
{
    [SerializeField] public JointsCatcher jointsCatcher;
    enum Gamestate
    {
        standby = 0,
        playing = 1,
        pause = 2,
        endgame = 3,
    }
    enum Facing
    {
        left = 1,
        right = 2,
    }
    const bool keyboardmode = true;
    const bool kinectmode = true;
    const float maxSpeed = 10f;

    public enum Action
    {
        walk_left = 0,
        walk_right = 1,
        jump_vert = 2,
        jump_left_s = 3,
        jump_left_m = 4,
        jump_left_w = 5,
        jump_right_s = 6,
        jump_right_m = 7,
        jump_right_w = 8,
        turn_left = 9,
        turn_right = 10,
        idle,
    }
    public Action act;
    private Gamestate gamestate;
    private Facing facing;
    public Rigidbody2D rb;
    public Camera mainCamera;
    public float bounceForce = 0.8f;
    public Vector2 lastV;
    public show s;
    public Gate_beheiver gate;

    Vector3 cameraPos;
    Rigidbody2D r2d;
    CapsuleCollider2D mainCollider;
    Transform t;

    public GameObject mainMenu;
    public GameObject jumpKing;

    // Start is called before the first frame update
    void Start()
    {
        gamestate = Gamestate.standby;
        facing = Facing.right;
        t = transform;
        rb = GetComponent<Rigidbody2D>();
        mainCollider = GetComponent<CapsuleCollider2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.gravityScale = 1.75f;
        rb.freezeRotation = true;
        if (mainCamera)
        {
            cameraPos = mainCamera.transform.position;
        }
        LockCameraToPlayer();
        lastV = Vector2.zero;
    }
    void Update()
    {
        LockCameraToPlayer();
        if (Input.GetKey(KeyCode.D))
        {
            act = Action.walk_right;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            act = Action.walk_left;
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            act = Action.jump_vert;
        }
        else if (Input.GetKey(KeyCode.Y))
        {
            act = Action.jump_left_s;
        }
        else if (Input.GetKey(KeyCode.H))
        {
            act = Action.jump_left_m;
        }
        else if (Input.GetKey(KeyCode.N))
        {
            act = Action.jump_left_w;
        }
        else if (Input.GetKey(KeyCode.U))
        {
            act = Action.jump_right_s;
        }
        else if (Input.GetKey(KeyCode.J))
        {
            act = Action.jump_right_m;
        }
        else if (Input.GetKey(KeyCode.M))
        {
            act = Action.jump_right_w;
        }
        else
        {
            act = Action.idle;
        }
        Vector2 HandLeft = new Vector2(jointsCatcher.jointSpeeds[0].position.x - jointsCatcher.jointSpeeds[1].position.x,
                                        jointsCatcher.jointSpeeds[0].position.y - jointsCatcher.jointSpeeds[1].position.y);
        HandLeft.x *= HandLeft.x > 0 ? 1 : -1;
        Vector2 HandRight = new Vector2(jointsCatcher.jointSpeeds[2].position.x - jointsCatcher.jointSpeeds[3].position.x,
                                        jointsCatcher.jointSpeeds[2].position.y - jointsCatcher.jointSpeeds[3].position.y);
        HandRight.x *= HandRight.x > 0 ? 1 : -1;
        if (HandLeft.y < 0 && HandLeft.y / HandLeft.x > -1 && jointsCatcher.jointSpeeds[0].speed < 0.2)
        {
            act = Action.walk_left;
        }
        else if (HandRight.y < 0 && HandRight.y / HandRight.x > -1 && jointsCatcher.jointSpeeds[2].speed < 0.2)
        {
            act = Action.walk_right;
        }
        else if (HandLeft.y / HandLeft.x > 0 && HandLeft.y / HandLeft.x < 0.57736 && jointsCatcher.jointSpeeds[0].speed < 0.2)
        {
            act = Action.jump_left_w;
        }
        else if (HandLeft.y / HandLeft.x > 0.57736 && HandLeft.y / HandLeft.x < 1.732 && jointsCatcher.jointSpeeds[0].speed < 0.2)
        {
            act = Action.jump_left_m;
        }
        else if (HandLeft.y / HandLeft.x > 1.732 && jointsCatcher.jointSpeeds[0].speed < 0.2)
        {
            act = Action.jump_left_s;
        }
        else if (HandRight.y / HandRight.x > 0 && HandRight.y / HandRight.x < 0.57736 && jointsCatcher.jointSpeeds[2].speed < 0.2)
        {
            act = Action.jump_right_w;
        }
        else if (HandRight.y / HandRight.x > 0.57736 && HandRight.y / HandRight.x < 1.732 && jointsCatcher.jointSpeeds[2].speed < 0.2)
        {
            act = Action.jump_right_m;
        }
        else if (HandRight.y / HandRight.x > 1.732 && jointsCatcher.jointSpeeds[2].speed < 0.2)
        {
            act = Action.jump_right_s;
        }
        //else
        //{
        //    act = Action.idle;
        //}
        if (onGround())
        {
            s.UpdateScoreText();
            DoAction(act);
        }
        lastV = rb.velocity;
    }
    public void DoAction(Action act)
    {
        switch(act)
        {
            case Action.walk_left:
                Turn_left();
                Walk(Facing.left);
                break;
            case Action.walk_right:
                Turn_right();
                Walk(Facing.right);
                break;
            case Action.jump_vert:
                Jump_Vert();
                break;
            case Action.jump_left_s:
                Turn_left();
                Jump(15f);
                break;
            case Action.jump_left_m:
                Turn_left();
                Jump(10f);
                break;
            case Action.jump_left_w:
                Turn_left();
                Jump(5f);
                break;
            case Action.jump_right_s:
                Turn_right();
                Jump(15f);
                break;
            case Action.jump_right_m:
                Turn_right();
                Jump(10f);
                break;
            case Action.jump_right_w:
                Turn_right();
                Jump(5f);
                break;
            case Action.turn_left:
                Turn_left();
                break;
            case Action.turn_right:
                Turn_right();
                break;
            case Action.idle:
                rb.velocity = Vector2.zero;
                break;
        }
    }
    void Walk(Facing face)
    {
        rb.velocity = ((face == Facing.right ? Vector2.right : Vector2.left) * 2f);
    }

    void Jump(float hight)
    {
        float dist = 10f;
        dist *= (facing == Facing.right ? 1 : -1);
        rb.velocity = new Vector2(dist, hight);
    }

    void Jump_Vert()
    {
        rb.velocity = new Vector2(0, 10f);

    }
    void Turn_left()
    {
        if(facing == Facing.right) 
        {
            facing= Facing.left;
            transform.localScale = new Vector2(1, 1);
        }
    }
    void Turn_right()
    {
        if(facing == Facing.left)
        {
            facing = Facing.right;
            transform.localScale = new Vector2(-1, 1);
        }
    }
    void Turn()
    {
        if(facing == Facing.left)
        {
            Turn_right();
        }
        else if(facing == Facing.right)
        {
            Turn_left();
        }
    }

    bool onGround()
    {
        Vector2 origin = new Vector2(transform.position.x, transform.position.y - 0.51f);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 0.01f); 
        if (hit.collider != null)
        {
            //Debug.Log("Collided with: " + hit.collider.gameObject.name);
            return true;
        }
        return false;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 normal = collision.contacts[0].normal;
        if (Mathf.Abs(normal.y) < 0.1f)
        {
            //Debug.Log("這是水平碰撞");
            Vector2 temp = lastV;
            temp.x *= (-bounceForce);
            rb.velocity = temp;
            Turn();
        }
        if (Mathf.Abs(normal.x) < 0.1f)
        {
            //Debug.Log("這是鉛直碰撞");
            rb.velocity = Vector2.zero;
            if (collision.gameObject.CompareTag("Finish"))
            {
                gate.closeGate();
            }
        }
        //Debug.DrawRay(collisionPoint, normal, Color.red, 10.0f);
        //Debug.Log("Bounced with " + collision.gameObject);
    }



    void LockCameraToPlayer()
    {
        if (mainCamera)
        {
            mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, t.position.y > 10 ? t.position.y:10, mainCamera.transform.position.z);
        }
    }

    public void Click_Exit()
    {
        transform.position = new Vector2(0, 2);
        mainMenu.SetActive(true);
        jumpKing.SetActive(false);
    }

}
