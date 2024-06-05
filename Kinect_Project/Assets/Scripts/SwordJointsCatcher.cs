using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Windows.Kinect;

public class SwrodJointsCatcher : MonoBehaviour
{
    [System.Serializable]
    public class JointSpeed
    {
        public int playerID;
        public JointType joint;
        [Range(0.1f, 1000.0f)]
        public float magnification = 10;
        public Vector3 position;
        public float speed;
        public bool handOpen = false;
        public bool tracked;

        [HideInInspector]
        public Vector3 prevPosition;

        [HideInInspector]
        public float prevTime;
    }

    public JointSpeed[] jointSpeeds;

    private KinectSensor kinectSensor;
    private BodyFrameReader bodyFrameReader;
    private Body[] bodies;

    // Start is called before the first frame update
    void Start()
    {
        kinectSensor = KinectSensor.GetDefault();

        if (kinectSensor != null)
        {
            kinectSensor.Open();
            bodyFrameReader = kinectSensor.BodyFrameSource.OpenReader();
            bodies = new Body[kinectSensor.BodyFrameSource.BodyCount];
        }
        else
        {
            Debug.LogError("No Kinect sensor found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //jointSpeeds[0].tracked = false;
        //jointSpeeds[1].tracked = false;

        if (bodyFrameReader != null)
        {
            using (BodyFrame bodyFrame = bodyFrameReader.AcquireLatestFrame())
            {
                if (bodyFrame != null)
                {
                    bodyFrame.GetAndRefreshBodyData(bodies);

                    Body bodyL = null;
                    Body bodyR = null;

                    foreach (Body body in bodies)
                    {
                        if (body == null || !body.IsTracked)
                        {
                            continue;
                        }

                        if (body.Joints[JointType.SpineBase].Position.X < 0f)
                        {
                            if (bodyL == null)
                            {
                                bodyL = body;
                            }
                            else if (body.Joints[JointType.SpineBase].Position.X < bodyL.Joints[JointType.SpineBase].Position.X)
                            {
                                bodyL = body;
                            }
                        }

                        if (body.Joints[JointType.SpineBase].Position.X > 0f)
                        {
                            if (bodyR == null)
                            {
                                bodyR = body;
                            }
                            else if (body.Joints[JointType.SpineBase].Position.X > bodyR.Joints[JointType.SpineBase].Position.X)
                            {
                                bodyR = body;
                            }
                        }
                    }

                    foreach (JointSpeed jointSpeed in jointSpeeds)
                    {
                        Body body = null;
                        if (jointSpeed.playerID == 0 && bodyL != null)      body = bodyL;
                        else if(jointSpeed.playerID == 1 && bodyR != null)  body = bodyR;

                        if(body == null)
                        {
                            jointSpeed.tracked = false;
                            continue;
                        }
                        else
                        {
                            jointSpeed.tracked = true;
                        }

                        var bonePosition = body.Joints[jointSpeed.joint].Position;
                        float deltaTime = Time.time - jointSpeed.prevTime;
                        jointSpeed.position
                            = new Vector3(bonePosition.X * jointSpeed.magnification, bonePosition.Y * jointSpeed.magnification, -bonePosition.Z * jointSpeed.magnification);
                        float distance = Vector3.Distance(jointSpeed.position, jointSpeed.prevPosition);
                        jointSpeed.speed = distance / deltaTime;
                        jointSpeed.prevPosition = jointSpeed.position;
                        jointSpeed.prevTime = Time.time;
                        jointSpeed.handOpen = (body.HandRightState == HandState.Open);
                    }
                }
            }
        }
    }

    void OnDestroy()
    {
        // ����Kinect
        if (kinectSensor != null)
        {
            kinectSensor.Close();
        }
    }
}
