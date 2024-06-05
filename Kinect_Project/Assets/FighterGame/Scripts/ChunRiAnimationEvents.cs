using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ChunRiAnimationEvents : MonoBehaviour
{
    public Sprite simpleSprite;
    public Vector3 additionalPos;
    public RuntimeAnimatorController mQigongAnimator;

    public List<AttackCollider> attackColliders = new List<AttackCollider>();
    public List<KeyValuePair<AttackCollider, Timer>> bulletColliders = new List<KeyValuePair<AttackCollider, Timer>>();

    private void Start()
    {
        additionalPos = new Vector3(0, 0, 0);
    }

    void Update()
    {
        List<AttackCollider> tmpAttackColliders = new List<AttackCollider>();

        foreach (AttackCollider attackCollider in attackColliders)
        {
            attackCollider.Update();

            if (attackCollider.boxCollider != null)
            {
                if (Time.time - attackCollider.generateTime > attackCollider.deadline)
                {
                    Destroy(attackCollider.boxCollider);
                }
                else
                {
                    tmpAttackColliders.Add(attackCollider);
                }
            }      
        }
        attackColliders.Clear();
        attackColliders = tmpAttackColliders;

        List<KeyValuePair<AttackCollider, Timer>> tmpBulletColliders = new List<KeyValuePair<AttackCollider, Timer>>();

        foreach (KeyValuePair<AttackCollider, Timer> bulletCollider in bulletColliders)
        {
            if (bulletCollider.Key.boxCollider != null)
            {
                if (bulletCollider.Value.isTimeOut())
                {
                    Destroy(bulletCollider.Key.boxCollider);
                }
                else
                {
                    tmpBulletColliders.Add(bulletCollider);
                }
            }
        }
        bulletColliders.Clear();
        bulletColliders = tmpBulletColliders;
    }

    public void LightPunchAE()
    {
        attackColliders.Add(new AttackCollider(
            transform, 
            new Vector3(transform.position.x + transform.localScale.x * 0.3f, transform.position.y + 0.7f, transform.position.z),
            new Vector2(0.3f, 0.3f),
            transform.name + "" + attackColliders.Count,
            transform.localScale.x < 0,
            "LightPunchCB"
            ));

        transform.position -= additionalPos;
        additionalPos = new Vector3(0, 0, 0);
    }

    public void HighPunchAE()
    {
        attackColliders.Add(new AttackCollider(
            transform,
            new Vector3(transform.position.x + transform.localScale.x * 0.45f, transform.position.y + 0.7f, transform.position.z),
            new Vector2(0.3f, 0.3f),
            transform.name + "" + attackColliders.Count,
            transform.localScale.x < 0,
            "HighPunchCB"
            ));

        transform.position -= additionalPos;
        additionalPos = new Vector3(0, 0, 0);
    }

    public void LightKickAE()
    {
        attackColliders.Add(new AttackCollider(
            transform,
            new Vector3(transform.position.x + transform.localScale.x * 0.35f, transform.position.y + 0.55f, transform.position.z),
            new Vector2(0.3f, 0.3f),
            transform.name + "" + attackColliders.Count,
            transform.localScale.x < 0,
            "LightKickCB"
            ));

        transform.position -= additionalPos;
        additionalPos = new Vector3(0, 0, 0);
    }

    public void HighKickAE()
    {
        attackColliders.Add(new AttackCollider(
            transform,
            new Vector3(transform.position.x + transform.localScale.x * 0.42f, transform.position.y + 0.75f, transform.position.z),
            new Vector2(0.3f, 0.3f),
            transform.name + "" + attackColliders.Count,
            transform.localScale.x < 0,
            "HighKickCB"
            ));

        transform.position -= additionalPos;
        additionalPos = new Vector3(0, 0, 0);
        additionalPos += new Vector3(transform.localScale.x * 0.1f, 0f, 0f);     
        transform.position += additionalPos;
    }

    public void JumpLightPunchAE()
    {
        attackColliders.Add(new AttackCollider(
            transform,
            new Vector3(transform.position.x + transform.localScale.x * 0.4f, transform.position.y + 0.3f, transform.position.z),
            new Vector2(0.3f, 0.3f),
            transform.name + "" + attackColliders.Count,
            transform.localScale.x < 0,
            "JumpLightPunchCB"
            ));

        transform.position -= additionalPos;
        additionalPos = new Vector3(0, 0, 0);
    }

    public void JumpHighPunchAE()
    {
        attackColliders.Add(new AttackCollider(
            transform,
            new Vector3(transform.position.x + transform.localScale.x * 0.35f, transform.position.y + 0.35f, transform.position.z),
            new Vector2(0.3f, 0.3f),
            transform.name + "" + attackColliders.Count,
            transform.localScale.x < 0,
            "JumpHighPunchCB"
            ));

        transform.position -= additionalPos;
        additionalPos = new Vector3(0, 0, 0);
    }

    public void JumpLightKickAE()
    {
        attackColliders.Add(new AttackCollider(
            transform,
            new Vector3(transform.position.x + transform.localScale.x * 0.35f, transform.position.y + 0.15f, transform.position.z),
            new Vector2(0.3f, 0.3f),
            transform.name + "" + attackColliders.Count,
            transform.localScale.x < 0,
            "JumpLightKickCB"
            ));

        transform.position -= additionalPos;
        additionalPos = new Vector3(0, 0, 0);
    }

    public void JumpHighKickAE()
    {
        attackColliders.Add(new AttackCollider(
            transform,
            new Vector3(transform.position.x + transform.localScale.x * 0.35f, transform.position.y + 0.2f, transform.position.z),
            new Vector2(0.3f, 0.3f),
            transform.name + "" + attackColliders.Count,
            transform.localScale.x < 0,
            "JumpHighKickCB"
            ));

        transform.position -= additionalPos;
        additionalPos = new Vector3(0, 0, 0);      
    }

    public void SDLightPunchAE()
    {
        attackColliders.Add(new AttackCollider(
            transform,
            new Vector3(transform.position.x + transform.localScale.x * 0.48f, transform.position.y + 0.35f, transform.position.z),
            new Vector2(0.3f, 0.3f),
            transform.name + "" + attackColliders.Count,
            transform.localScale.x < 0,
            "SDLightPunchCB"
            ));

        transform.position -= additionalPos;
        additionalPos = new Vector3(0, 0, 0);
        additionalPos += new Vector3(transform.localScale.x * 0.15f, 0f, 0f);
        transform.position += additionalPos;
    }

    public void SDHighPunchAE()
    {
        attackColliders.Add(new AttackCollider(
            transform,
            new Vector3(transform.position.x + transform.localScale.x * 0.48f, transform.position.y + 0.35f, transform.position.z),
            new Vector2(0.3f, 0.3f),
            transform.name + "" + attackColliders.Count,
            transform.localScale.x < 0,
            "SDHighPunchCB"
            ));

        transform.position -= additionalPos;
        additionalPos = new Vector3(0, 0, 0);
        additionalPos += new Vector3(transform.localScale.x * 0.15f, 0f, 0f);
        transform.position += additionalPos;
    }

    public void SDLightKickAE()
    {
        attackColliders.Add(new AttackCollider(
            transform,
            new Vector3(transform.position.x + transform.localScale.x * 0.45f, transform.position.y + 0.15f, transform.position.z),
            new Vector2(0.3f, 0.3f),
            transform.name + "" + attackColliders.Count,
            transform.localScale.x < 0,
            "SDLightKickCB"
            ));

        transform.position -= additionalPos;
        additionalPos = new Vector3(0, 0, 0);
        additionalPos += new Vector3(transform.localScale.x * 0.1f, 0f, 0f);
        transform.position += additionalPos;
    }

    public void SDHighKickAE()
    {
        attackColliders.Add(new AttackCollider(
            transform,
            new Vector3(transform.position.x + transform.localScale.x * 0.5f, transform.position.y + 0.45f, transform.position.z),
            new Vector2(0.3f, 0.3f),
            transform.name + "" + attackColliders.Count,
            transform.localScale.x < 0,
            "SDHighKickCB"
            ));

        transform.position -= additionalPos;
        additionalPos = new Vector3(0, 0, 0);
        additionalPos += new Vector3(transform.localScale.x * 0.15f, 0f, 0f);
        transform.position += additionalPos;
    }

    public void QigongAE()
    {
        attackColliders.Add(new AttackCollider(
            transform,
            new Vector3(transform.position.x + transform.localScale.x * 0.4f, transform.position.y + 0.5f, transform.position.z),
            new Vector2(0.3f, 0.3f),
            transform.name + "" + attackColliders.Count,
            transform.localScale.x < 0,
            "QigongCB",
            20f,
            Time.time,
            4,
            new Vector3(transform.position.x + transform.localScale.x * 10f, transform.position.y + 0.5f, transform.position.z),
            mQigongAnimator,
            "ChunRi_qigongBulletFly1",
            default(Sprite),
            0.3f
            ));

        transform.position -= additionalPos;
        additionalPos = new Vector3(0, 0, 0);
        //additionalPos += new Vector3(transform.localScale.x * 0.13f, 0f, 0f);
        //transform.position += additionalPos;
    }

    public void CloseLightPunchAE()
    {
        attackColliders.Add(new AttackCollider(
            transform,
            new Vector3(transform.position.x + transform.localScale.x * 0.4f, transform.position.y + 0.8f, transform.position.z),
            new Vector2(0.3f, 0.3f),
            transform.name + "" + attackColliders.Count,
            transform.localScale.x < 0,
            "CloseLightPunchCB"
            ));

        transform.position -= additionalPos;
        additionalPos = new Vector3(0, 0, 0);
        additionalPos += new Vector3(transform.localScale.x * 0.1f, 0f, 0f);
        transform.position += additionalPos;
    }

    public void CloseHighPunchAE()
    {
        attackColliders.Add(new AttackCollider(
            transform,
            new Vector3(transform.position.x + transform.localScale.x * 0.35f, transform.position.y + 0.7f, transform.position.z),
            new Vector2(0.3f, 0.3f),
            transform.name + "" + attackColliders.Count,
            transform.localScale.x < 0,
            "CloseHighPunchCB"
            ));

        transform.position -= additionalPos;
        additionalPos = new Vector3(0, 0, 0);
        additionalPos += new Vector3(transform.localScale.x * 0.1f, 0f, 0f);
        transform.position += additionalPos;
    }

    public void CloseLightKickAE()
    {
        attackColliders.Add(new AttackCollider(
            transform,
            new Vector3(transform.position.x + transform.localScale.x * 0.58f, transform.position.y + 0.3f, transform.position.z),
            new Vector2(0.3f, 0.3f),
            transform.name + "" + attackColliders.Count,
            transform.localScale.x < 0,
            "CloseLightKickCB"
            ));

        transform.position -= additionalPos;
        additionalPos = new Vector3(0, 0, 0);
        additionalPos += new Vector3(transform.localScale.x * 0.35f, 0f, 0f);
        transform.position += additionalPos;
    }

    public void CloseHighKickAE()
    {
        attackColliders.Add(new AttackCollider(
            transform,
            new Vector3(transform.position.x + transform.localScale.x * 0.45f, transform.position.y + 0.6f, transform.position.z),
            new Vector2(0.3f, 0.3f),
            transform.name + "" + attackColliders.Count,
            transform.localScale.x < 0,
            "CloseHighKickCB"
            ));

        transform.position -= additionalPos;
        additionalPos = new Vector3(0, 0, 0);
        additionalPos += new Vector3(transform.localScale.x * 0.28f, 0f, 0f);
        transform.position += additionalPos;
    }

    public void UppercutAE()
    {
        attackColliders.Add(new AttackCollider(
            transform,
            new Vector3(transform.position.x + transform.localScale.x * 0.45f, transform.position.y + 0.8f, transform.position.z),
            new Vector2(0.3f, 0.3f),
            transform.name + "" + attackColliders.Count,
            transform.localScale.x < 0,
            "UppercutCB"
            ));

        transform.position -= additionalPos;
        additionalPos = new Vector3(0, 0, 0);
        additionalPos += new Vector3(transform.localScale.x * 0.13f, 0f, 0f);
        transform.position += additionalPos;
    }

    public void Ultimate3AE()
    {
        transform.position -= additionalPos;
        additionalPos = new Vector3(0, 0, 0);
        additionalPos += new Vector3(transform.localScale.x * 0.2f, -0.1f, 0f);
        transform.position += additionalPos;
    }

    public void Ultimate3AE_Reply()
    {
        transform.position -= additionalPos;
        additionalPos = new Vector3(0, 0, 0);
    }

    public void Ultimate3AE2()
    {
        attackColliders.Add(new AttackCollider(
            transform,
            new Vector3(transform.position.x - transform.localScale.x * -0.15f, transform.position.y + 0.6f, transform.position.z),
            new Vector2(1f, 1.2f),
            transform.name + "" + attackColliders.Count,
            transform.localScale.x < 0,
            "Ultimate3CB"
            ));

        transform.position -= additionalPos;
        additionalPos = new Vector3(0, 0, 0);
        additionalPos += new Vector3(transform.localScale.x * 0.13f, 0f, 0f);
        transform.position += additionalPos;
    }

    public void Ultimate2AE()
    {
        transform.position -= additionalPos;
        additionalPos = new Vector3(0, 0, 0);
        additionalPos += new Vector3(transform.localScale.x * 0.35f, -0.35f, 0f);
        transform.position += additionalPos;
    }

    public void Ultimate2AE_Reply()
    {
        transform.position -= additionalPos;
        additionalPos = new Vector3(0, 0, 0);
    }

    public void Ultimate2AE2()
    {
        attackColliders.Add(new AttackCollider(
            transform,
            new Vector3(transform.position.x + transform.localScale.x * 0.35f, transform.position.y + 0.55f, transform.position.z),
            new Vector2(0.5f, 0.5f),
            transform.name + "" + attackColliders.Count,
            transform.localScale.x < 0,
            "Ultimate2CB"
            ));

        transform.position -= additionalPos;
        additionalPos = new Vector3(0, 0, 0);
    }

    public void Ultimate2AE3()
    {
        attackColliders.Add(new AttackCollider(
            transform,
            new Vector3(transform.position.x + transform.localScale.x * 0.25f, transform.position.y + 0.9f, transform.position.z),
            new Vector2(0.3f, 0.3f),
            transform.name + "" + attackColliders.Count,
            transform.localScale.x < 0,
            "Ultimate2CB"
            ));

        transform.position -= additionalPos;
        additionalPos = new Vector3(0, 0, 0);
    }

    public void Ultimate1AE()
    {
        attackColliders.Add(new AttackCollider(
            transform,
            new Vector3(transform.position.x + transform.localScale.x * 0.3f, transform.position.y + 0.55f, transform.position.z),
            new Vector2(0.3f, 0.3f),
            transform.name + "" + attackColliders.Count,
            transform.localScale.x < 0,
            "Ultimate1CB"
            ));

        attackColliders.Add(new AttackCollider(
           transform,
           new Vector3(transform.position.x - transform.localScale.x * 0.3f, transform.position.y + 0.55f, transform.position.z),
           new Vector2(0.3f, 0.3f),
           transform.name + "" + attackColliders.Count,
           transform.localScale.x < 0,
           "Ultimate1CB"
           ));

        transform.position -= additionalPos;
        additionalPos = new Vector3(0, 0, 0);
    }
}