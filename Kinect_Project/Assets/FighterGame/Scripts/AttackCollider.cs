using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AttackCollider
{
    public RuntimeAnimatorController qigongAnimator;
    public GameObject boxCollider;
    public Transform parentTransform;
    public Sprite sprite;
    public Vector3 generatePos;
    public Vector3 targetPos;
    public Vector2 sizeDelta;
    public string tag;
    public string qigongAnimateName;
    public float generateTime;
    public float deadline;
    public float speed;
    public int order;

    public AttackCollider(Transform _parentTransform, Vector3 _generatePos, Vector2 _sizeDelta, string name, bool scaleXNegative = false, string _tag = "", float _deadline = 0.1f,
        float _generateTime = -1, int _order = 4, Vector3 _targetPos = default(Vector3), RuntimeAnimatorController _qigongAnimator = default(RuntimeAnimatorController),
        string _qigongAnimateName = "", Sprite _sprite = default(Sprite), float _speed = 0.5f)
    {
        qigongAnimator = _qigongAnimator;
        parentTransform = _parentTransform;
        targetPos = _targetPos;
        generatePos = _generatePos;
        sizeDelta = _sizeDelta;
        generateTime = _generateTime;
        tag = _tag;
        sprite = _sprite;
        order = _order;
        deadline = _deadline;
        speed = _speed;
        qigongAnimateName = _qigongAnimateName;

        if (targetPos == default(Vector3))
        {
            targetPos = generatePos;
        }

        if (generateTime == -1)
        {
            generateTime = Time.time;
        }

        boxCollider = new GameObject();
        //boxCollider.transform.SetParent(parentTransform);
        boxCollider.tag = tag;

        Text text = boxCollider.AddComponent<Text>();
        text.text = name;

        RectTransform rectTransform = boxCollider.GetComponent<RectTransform>();
        rectTransform.sizeDelta = sizeDelta;
        if (scaleXNegative)
            rectTransform.localScale = new Vector3(-rectTransform.localScale.x, rectTransform.localScale.y, rectTransform.localScale.z);
        rectTransform.position = generatePos; //new Vector3(parentTransform.position.x + parentTransform.localScale.x * 0.35f, parentTransform.position.y + 0.6f, parentTransform.position.z); // 可以自定義位置

        SpriteRenderer spriteRenderer = boxCollider.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = order;

        BoxCollider2D boxCollider2D = boxCollider.AddComponent<BoxCollider2D>();
        boxCollider2D.size = sizeDelta;
        boxCollider2D.isTrigger = true;

        if (tag == "QigongCB")
        {
            Animator animator = boxCollider.AddComponent<Animator>();
            animator.runtimeAnimatorController = qigongAnimator;
            animator.Play(qigongAnimateName);
        }
    }

    public void Update()
    {
        boxCollider.transform.position = Vector3.Lerp(boxCollider.transform.position, targetPos, Time.deltaTime * speed);
    }
}
