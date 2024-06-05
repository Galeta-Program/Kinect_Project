using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBoxController : MonoBehaviour
{
    [Header("Scripts")]

    public BodyManagerSF bodyManager;
    public RightHandCollisionEvent rightHandCE;
    public LeftHandCollisionEvent leftHandCE;
    public RightFootCollisionEvent rightFootCE;
    public LeftFootCollisionEvent leftFootCE;
    public RightFootCollisionEvent rightAnkleCE;
    public LeftFootCollisionEvent leftAnkleCE;

    [Header("TriggerBoxes")]

    public GameObject lightPunchTB;
    public GameObject highPunchTB;
    public GameObject squatDownTB;
    public GameObject squatDown2TB;
    public GameObject jumpTB;
    public GameObject jump2TB;
    public GameObject forwardTB;
    public GameObject backwardTB;
    public GameObject lightKickTB;
    public GameObject highKickTB;

    private Vector3 rightShoulderPos;
    private Vector3 leftShoulderPos;
    private Vector3 spineBasePos;
    private Vector3 headPos;

    // Start is called before the first frame update
    void Start()
    {
        rightShoulderPos = bodyManager.shoulderRight.transform.position;
        leftShoulderPos = bodyManager.shoulderLeft.transform.position;
        spineBasePos = bodyManager.spineBase.transform.position;
        headPos = bodyManager.head.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(bodyManager.shoulderLeft.transform.position, leftShoulderPos) >= 2)
        {
            lightPunchTB.transform.position = bodyManager.shoulderLeft.transform.position + new Vector3(0f, -0.5f, 4f);
            leftShoulderPos = bodyManager.shoulderLeft.transform.position;
            //Debug.Log("BP" + bodyManager.shoulderLeft.transform.position.ToString());
            //Debug.Log(lightPunchTB.transform.position.ToString());
        }

        if (Vector3.Distance(bodyManager.shoulderRight.transform.position, rightShoulderPos) >= 2)
        {
            highPunchTB.transform.position = bodyManager.shoulderRight.transform.position + new Vector3(0f, -0.25f, 4.25f);
            rightShoulderPos = bodyManager.shoulderRight.transform.position;
            //Debug.Log("BP H" + bodyManager.shoulderRight.transform.position.ToString());
            //Debug.Log(highPunchTB.transform.position.ToString());
        }

        if (Vector3.Distance(bodyManager.spineBase.transform.position, spineBasePos) >= 4.5)
        {
            float legsLong = Vector3.Distance(bodyManager.hipLeft.transform.position, bodyManager.kneeLeft.transform.position)
                + Vector3.Distance(bodyManager.kneeLeft.transform.position, bodyManager.footLeft.transform.position);

            forwardTB.transform.position = bodyManager.spineBase.transform.position + new Vector3(1.25f, -legsLong + 0.7f, 7f);
            backwardTB.transform.position = bodyManager.spineBase.transform.position + new Vector3(1.25f, -legsLong + 0.7f, -5.5f);
            squatDownTB.transform.position = bodyManager.spineBase.transform.position + new Vector3(-3.25f, -0.7f, 1f);
            squatDown2TB.transform.position = bodyManager.spineBase.transform.position + new Vector3(3.5f, -0.7f, 1.1f);
            lightKickTB.transform.position = bodyManager.spineBase.transform.position + new Vector3(-1.5f, -legsLong +5.7f, 6.5f);
            highKickTB.transform.position = bodyManager.spineBase.transform.position + new Vector3(2.8f, -legsLong + 5.7f, 6.5f);
            spineBasePos = bodyManager.spineBase.transform.position;
        }

        if (Vector3.Distance(bodyManager.head.transform.position, headPos) >= 2)
        {
            jumpTB.transform.position = bodyManager.head.transform.position + new Vector3(3f, 1.5f, 0.1f);
            jump2TB.transform.position = bodyManager.head.transform.position + new Vector3(-3f, 1.5f, 0.1f);
            headPos = bodyManager.head.transform.position;
        }
    }
}
