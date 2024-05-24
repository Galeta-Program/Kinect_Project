using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Character : MonoBehaviour
{
    public enum RIGLAYER
    {
        LOOKAT,
    }

    private Animator animator;
    public Dictionary<RIGLAYER, RigLayer> rig;
    [SerializeField] public GameObject LookAtTarget;
    private float velocity { 
        get { 
            return velocity;
        } 
        set { 
            velocity = value;
            animator.SetFloat("velocity", value);
        } 
    }
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rig[RIGLAYER.LOOKAT] = GetComponent<RigBuilder>().layers[0];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
