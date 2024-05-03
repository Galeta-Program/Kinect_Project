using UnityEngine;
using Windows.Kinect;
using System.Collections.Generic;
using System;

public class BodyManager : MonoBehaviour
{
    private KinectSensor kinectSensor;
    private BodyFrameReader bodyFrameReader;
    private Body[] bodies;
    private GameObject[] joints;
    private GameObject[] bones;
    private Dictionary<GameObject, (JointType, JointType)> bone2JointsMap;

    [Range(0.0f, 50.0f)]
    public float magnification = 10;

    [Header("身體關節(Body Joints)")]

    public GameObject spineBase;
    public GameObject spineMid;
    public GameObject neck;
    public GameObject head;

    [Header("")]

    public GameObject shoulderLeft;
    public GameObject elbowLeft;
    public GameObject wristLeft;
    public GameObject handLeft;

    [Header("")]

    public GameObject shoulderRight;
    public GameObject elbowRight;
    public GameObject wristRight;
    public GameObject handRight;

    [Header("")]

    public GameObject hipLeft;
    public GameObject kneeLeft;
    public GameObject ankleLeft;
    public GameObject footLeft;

    [Header("")]

    public GameObject hipRight;
    public GameObject kneeRight;
    public GameObject ankleRight;
    public GameObject footRight;

    [Header("")]

    public GameObject spineShoulder;
    public GameObject handTipLeft;
    public GameObject thumbLeft;
    public GameObject handTipRight;
    public GameObject thumbRight;

    [Header("身體骨頭(Body Bones)")] 

    public GameObject upperArmLeft;
    public GameObject forearmLeft;
    public GameObject thighLeft;
    public GameObject calfLeft;
    public GameObject solesOfFeetLeft;

    [Header("")]

    public GameObject upperArmRight;
    public GameObject forearmRight;
    public GameObject thighRight;
    public GameObject calfRight;
    public GameObject solesOfFeetRight;

    [Header("")]

    public GameObject headToNeck;
    public GameObject spineFirst;
    public GameObject spineMiddle;
    public GameObject spineLast;
    public GameObject shoulder;
    public GameObject hip;

    void Start()
    {
        kinectSensor = KinectSensor.GetDefault();

        if (kinectSensor != null)
        {           
            kinectSensor.Open();
            bodyFrameReader = kinectSensor.BodyFrameSource.OpenReader();
            bodies = new Body[kinectSensor.BodyFrameSource.BodyCount];

            joints = new GameObject[] {
                spineBase,     spineMid,   neck,         head,
                shoulderLeft,  elbowLeft,  wristLeft,    handLeft,
                shoulderRight, elbowRight, wristRight,   handRight,
                hipLeft,       kneeLeft,   ankleLeft,    footLeft,
                hipRight,      kneeRight,  ankleRight,   footRight,
                spineShoulder, 
                handTipLeft,   thumbLeft,  handTipRight, thumbRight,
            };

            bones = new GameObject[] {
                upperArmLeft,  forearmLeft,  thighLeft,   calfLeft,  solesOfFeetLeft,
                upperArmRight, forearmRight, thighRight,  calfRight, solesOfFeetRight,
                headToNeck,    spineFirst,   spineMiddle, spineLast, shoulder, 
                hip
            };

            bone2JointsMap = new Dictionary<GameObject, (JointType, JointType)>() {
                { upperArmLeft,     (JointType.WristLeft,     JointType.ElbowLeft    )},
                { forearmLeft,      (JointType.ElbowLeft,     JointType.ShoulderLeft )},
                { thighLeft,        (JointType.HipLeft,       JointType.KneeLeft     )},
                { calfLeft,         (JointType.KneeLeft,      JointType.AnkleLeft    )},
                { solesOfFeetLeft,  (JointType.AnkleLeft,     JointType.FootLeft     )},
                { upperArmRight,    (JointType.WristRight,    JointType.ElbowRight   )},
                { forearmRight,     (JointType.ElbowRight,    JointType.ShoulderRight)},
                { thighRight,       (JointType.HipRight,      JointType.KneeRight    )},
                { calfRight,        (JointType.KneeRight,     JointType.AnkleRight   )},
                { solesOfFeetRight, (JointType.AnkleRight,    JointType.FootRight    )},
                { headToNeck,       (JointType.Head,          JointType.Neck         )},
                { spineFirst,       (JointType.Neck,          JointType.SpineShoulder)},
                { spineMiddle,      (JointType.SpineShoulder, JointType.SpineMid     )},
                { spineLast,        (JointType.SpineMid,      JointType.SpineBase    )},
                { shoulder,         (JointType.ShoulderLeft,  JointType.ShoulderRight)},
                { hip,              (JointType.HipLeft,       JointType.HipRight     )},
            };

        }
        else
        {
            Debug.LogError("No Kinect sensor found!");
        }
    }

    void Update()
    {        
        if (bodyFrameReader != null)
        {
            using (BodyFrame bodyFrame = bodyFrameReader.AcquireLatestFrame())
            {
                if (bodyFrame != null)
                {
                    bodyFrame.GetAndRefreshBodyData(bodies);

                    foreach (Body body in bodies)
                    {
                        if (body == null || !body.IsTracked)
                        {
                            continue;
                        }

                        for (int i = 0; i < joints.Length; i++)
                        {
                            if (joints[i] != null)
                            {
                                var bonePosition = body.Joints[(JointType)i].Position;
                                joints[i].transform.position 
                                    = new Vector3(bonePosition.X * magnification, bonePosition.Y * magnification, -bonePosition.Z * magnification);
                            }
                        }

                        for (int i = 0; i < bones.Length; i++)
                        {
                            if (bones[i] != null)
                            {
                                JointType joint1 = bone2JointsMap[bones[i]].Item1;
                                JointType joint2 = bone2JointsMap[bones[i]].Item2;
                                UpdateBonePosition(body.Joints[joint1], body.Joints[joint2], bones[i]);
                            }
                        }
                    }
                }
            }
        }
    }

    Vector3 GetVector3FromJoint(Windows.Kinect.Joint joint)
    {
        // Kinect 坐標系與 Unity 坐標系有所不同，需要進行轉換
        Vector3 pos = new Vector3(joint.Position.X * magnification, joint.Position.Y * magnification, -joint.Position.Z * magnification);
        return pos;
    }

    void UpdateBonePosition(Windows.Kinect.Joint joint1, Windows.Kinect.Joint joint2, GameObject forearm)
    {
        if (joint1.TrackingState == TrackingState.Tracked && joint2.TrackingState == TrackingState.Tracked)
        {
            Vector3 joint1Pos = GetVector3FromJoint(joint1);
            Vector3 joint2Pos = GetVector3FromJoint(joint2);

            forearm.transform.position = (joint1Pos + joint2Pos) / 2;

            Vector3 armDirection = joint1Pos - joint2Pos;
            armDirection.Normalize();
            forearm.transform.rotation = Quaternion.LookRotation(armDirection) * Quaternion.Euler(90, 90, 90);
        }
    }

    void OnDestroy()
    {
        // 關閉Kinect
        if (kinectSensor != null)
        {
            kinectSensor.Close();
        }
    }
}
