using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Windows.Kinect;

public class JointsCatcher : MonoBehaviour
{
    [System.Serializable]
    public class JointSpeed
    {
        public JointType joint;
        [Range(0.1f, 1000.0f)]
        public float magnification = 10;
        public Vector3 position;
        public float speed;

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

                        foreach (JointSpeed jointSpeed in jointSpeeds)
                        {
                            var bonePosition = body.Joints[jointSpeed.joint].Position;
                            float deltaTime = Time.time - jointSpeed.prevTime;
                            jointSpeed.position
                                = new Vector3(bonePosition.X * jointSpeed.magnification, bonePosition.Y * jointSpeed.magnification, -bonePosition.Z * jointSpeed.magnification);
                            float distance = Vector3.Distance(jointSpeed.position, jointSpeed.prevPosition);
                            jointSpeed.speed = distance / deltaTime;
                            jointSpeed.prevPosition = jointSpeed.position;
                            jointSpeed.prevTime = Time.time;
                        }
                    }
                }
            }
        }
    }

    void OnDestroy()
    {
        if (kinectSensor != null)
        {
            kinectSensor.Close();
        }
    }
}
