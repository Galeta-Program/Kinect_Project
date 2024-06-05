using UnityEngine;
using Windows.Kinect;
using System.Collections.Generic;
using System;

public enum KeyCodeSF
{
    LightPunch,
    HighPunch,
    LightKick,
    HighKick,
    Forward,
    Backward,
    SquatDown,
    Jump,
    Defense
}

public enum KeyStateSF
{
    Null,
    Up,
    Down,
    Stay,
}

public class ControllerWithKinect : MonoBehaviour
{
    public GameManagerSF gameManager;
    private KinectSensor kinectSensor;
    private BodyFrameReader bodyFrameReader;
    private Body[] bodies;

    [Range(0.0f, 50.0f)]
    public float magnification = 10;
    List<List<KeyStateSF>> keyStates;

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

        keyStates = new List<List<KeyStateSF>>();
        keyStates.Add(new List<KeyStateSF>());
        keyStates.Add(new List<KeyStateSF>());

        for (int i = 0; i < Enum.GetValues(typeof(KeyCodeSF)).Length; i++)
        {
            keyStates[0].Add(KeyStateSF.Null);
            keyStates[1].Add(KeyStateSF.Null);
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
                    int countOK = 0;

                    for (int i = 0; i < bodies.Length; i++)
                    {
                        if (bodies[i] != null && bodies[i].IsTracked)
                        {
                            countOK++;
                        }                           
                    }

                    if (countOK > keyStates.Count)
                    {
                        for (int i = 0; i < countOK - keyStates.Count; i++)
                        {
                            keyStates.Add(new List<KeyStateSF>(Enum.GetValues(typeof(KeyCodeSF)).Length));

                            for (int j = 0; j < Enum.GetValues(typeof(KeyCodeSF)).Length; j++)
                            {
                                keyStates[i][j] = KeyStateSF.Null;;
                            }
                        }
                    }

                    for (int i = 0; i < countOK; i++)
                    {
                        for (int j = 0; j < Enum.GetValues(typeof(KeyCodeSF)).Length; j++)
                        {
                            if (keyStates[i][j] == KeyStateSF.Up)
                            {
                                keyStates[i][j] = KeyStateSF.Null;
                            }
                            else if (keyStates[i][j] == KeyStateSF.Down)
                            {
                                keyStates[i][j] = KeyStateSF.Stay;
                            }
                        }
                    }

                    for (int i = 0, j = 0; i < bodies.Length; i++)
                    {
                        if (bodies[i] == null || !bodies[i].IsTracked)
                        {
                            continue;
                        }

                        float dx = bodies[i].Joints[JointType.SpineBase].Position.X - bodies[i].Joints[JointType.SpineShoulder].Position.X;
                        float dz = -bodies[i].Joints[JointType.SpineBase].Position.Z + bodies[i].Joints[JointType.SpineShoulder].Position.Z;

                        Vector3 headPos = new Vector3(
                             bodies[i].Joints[JointType.Head].Position.X * magnification,
                             bodies[i].Joints[JointType.Head].Position.Y * magnification,
                            -bodies[i].Joints[JointType.Head].Position.Z * magnification);

                        Vector3 rightHandPos = new Vector3(
                             bodies[i].Joints[JointType.HandRight].Position.X * magnification,
                             bodies[i].Joints[JointType.HandRight].Position.Y * magnification,
                            -bodies[i].Joints[JointType.HandRight].Position.Z * magnification);

                        Vector3 leftHandPos = new Vector3(
                             bodies[i].Joints[JointType.HandLeft].Position.X * magnification,
                             bodies[i].Joints[JointType.HandLeft].Position.Y * magnification,
                            -bodies[i].Joints[JointType.HandLeft].Position.Z * magnification);

                        Vector3 rightShoulderPos = new Vector3(
                            bodies[i].Joints[JointType.ShoulderRight].Position.X * magnification,
                            bodies[i].Joints[JointType.ShoulderRight].Position.Y * magnification,
                           -bodies[i].Joints[JointType.ShoulderRight].Position.Z * magnification);

                        Vector3 leftShoulderPos = new Vector3(
                             bodies[i].Joints[JointType.ShoulderLeft].Position.X * magnification,
                             bodies[i].Joints[JointType.ShoulderLeft].Position.Y * magnification,
                            -bodies[i].Joints[JointType.ShoulderLeft].Position.Z * magnification);

                        Vector3 leftHipPos = new Vector3(
                             bodies[i].Joints[JointType.HipLeft].Position.X * magnification - dx, 
                             bodies[i].Joints[JointType.HipLeft].Position.Y * magnification, 
                            -bodies[i].Joints[JointType.HipLeft].Position.Z * magnification - dz);

                        Vector3 leftKneePos = new Vector3(
                             bodies[i].Joints[JointType.KneeLeft].Position.X * magnification - dx, 
                             bodies[i].Joints[JointType.KneeLeft].Position.Y * magnification, 
                            -bodies[i].Joints[JointType.KneeLeft].Position.Z * magnification - dz);

                        Vector3 leftFootPos = new Vector3(
                             bodies[i].Joints[JointType.FootLeft].Position.X * magnification - dx, 
                             bodies[i].Joints[JointType.FootLeft].Position.Y * magnification, 
                            -bodies[i].Joints[JointType.FootLeft].Position.Z * magnification - dz);

                        Vector3 rightHipPos = new Vector3(
                             bodies[i].Joints[JointType.HipRight].Position.X * magnification - dx,
                             bodies[i].Joints[JointType.HipRight].Position.Y * magnification,
                            -bodies[i].Joints[JointType.HipRight].Position.Z * magnification - dz);

                        Vector3 rightFootPos = new Vector3(
                             bodies[i].Joints[JointType.FootRight].Position.X * magnification - dx,
                             bodies[i].Joints[JointType.FootRight].Position.Y * magnification,
                            -bodies[i].Joints[JointType.FootRight].Position.Z * magnification - dz);

                        Vector3 shouderMidPos = new Vector3(
                            bodies[i].Joints[JointType.SpineShoulder].Position.X * magnification,
                            bodies[i].Joints[JointType.SpineShoulder].Position.Y * magnification,
                           -bodies[i].Joints[JointType.SpineShoulder].Position.Z * magnification);

                        float legsLong = Vector3.Distance(leftHipPos, leftKneePos) + Vector3.Distance(leftKneePos, leftFootPos);

                        // LightPunch
                        if (leftHandPos.z - leftShoulderPos.z > 8 && keyStates[j][(int)KeyCodeSF.LightPunch] != KeyStateSF.Stay)
                        {
                            keyStates[j][(int)KeyCodeSF.LightPunch] = KeyStateSF.Down;
                        }
                        else if (leftHandPos.z - leftShoulderPos.z <= 8 && keyStates[j][(int)KeyCodeSF.LightPunch] != KeyStateSF.Null)
                        {
                            keyStates[j][(int)KeyCodeSF.LightPunch] = KeyStateSF.Up;
                        }

                        // HighPunch
                        if (rightHandPos.z - rightShoulderPos.z > 8 && keyStates[j][(int)KeyCodeSF.HighPunch] != KeyStateSF.Stay)
                        {
                            keyStates[j][(int)KeyCodeSF.HighPunch] = KeyStateSF.Down;
                        }
                        else if (rightHandPos.z - rightShoulderPos.z <= 8 && keyStates[j][(int)KeyCodeSF.HighPunch] != KeyStateSF.Null)
                        {
                            keyStates[j][(int)KeyCodeSF.HighPunch] = KeyStateSF.Up;
                        }

                        // Jump
                        if ((leftShoulderPos.y - leftHandPos.y <= -8 || rightShoulderPos.y - rightHandPos.y <= -8) && keyStates[j][(int)KeyCodeSF.Jump] != KeyStateSF.Stay)
                        {
                            keyStates[j][(int)KeyCodeSF.Jump] = KeyStateSF.Down;
                        }
                        else if ((leftShoulderPos.y - leftHandPos.y > -8 && rightShoulderPos.y - rightHandPos.y > -8) && keyStates[j][(int)KeyCodeSF.Jump] != KeyStateSF.Null)
                        {
                            keyStates[j][(int)KeyCodeSF.Jump] = KeyStateSF.Up;
                        }

                        // SquatDown
                        if ((leftShoulderPos.y - leftHandPos.y > 8 || rightShoulderPos.y - rightHandPos.y > 8) && keyStates[j][(int)KeyCodeSF.SquatDown] != KeyStateSF.Stay)
                        {
                            keyStates[j][(int)KeyCodeSF.SquatDown] = KeyStateSF.Down;
                        }
                        else if ((leftShoulderPos.y - leftHandPos.y <= 8 && rightShoulderPos.y - rightHandPos.y <= 8) && keyStates[j][(int)KeyCodeSF.SquatDown] != KeyStateSF.Null)
                        {
                            keyStates[j][(int)KeyCodeSF.SquatDown] = KeyStateSF.Up;
                        }

                        // LightKick
                        if (leftHipPos.y - leftFootPos.y <= legsLong - 7 && leftFootPos.z - leftHipPos.z > 10 && keyStates[j][(int)KeyCodeSF.LightKick] != KeyStateSF.Stay)
                        {
                            keyStates[j][(int)KeyCodeSF.LightKick] = KeyStateSF.Down;
                        }
                        else if ((leftHipPos.y - leftFootPos.y > legsLong - 7 || leftFootPos.z - leftHipPos.z <= 10) && keyStates[j][(int)KeyCodeSF.LightKick] != KeyStateSF.Null)
                        {
                            keyStates[j][(int)KeyCodeSF.LightKick] = KeyStateSF.Up;
                        }

                        // HighKick
                        if (rightHipPos.y - rightFootPos.y <= legsLong - 7 && rightFootPos.z - rightHipPos.z > 10 && keyStates[j][(int)KeyCodeSF.HighKick] != KeyStateSF.Stay)
                        {
                            keyStates[j][(int)KeyCodeSF.HighKick] = KeyStateSF.Down;
                        }
                        else if ((rightHipPos.y - rightFootPos.y > legsLong - 7 || rightFootPos.z - rightHipPos.z <= 10) && keyStates[j][(int)KeyCodeSF.HighKick] != KeyStateSF.Null)
                        {
                            keyStates[j][(int)KeyCodeSF.HighKick] = KeyStateSF.Up;
                        }

                        string tag = "";

                        if (j == 0)
                        {
                            tag = "Player2";
                        }
                        else
                        {
                            tag = "Player1";
                        }

                        if (gameManager.GetOpponent(tag).transform.localScale.x > 0)
                        {
                            // Forward
                            if (((leftHipPos.y - leftFootPos.y <= legsLong - 1 && leftFootPos.z - leftHipPos.z - 5 > 1.8) ||
                                (rightHipPos.y - rightFootPos.y <= legsLong - 1 && rightFootPos.z - rightHipPos.z - 5 > 1.8)) && keyStates[j][(int)KeyCodeSF.Forward] != KeyStateSF.Stay)
                            {
                                keyStates[j][(int)KeyCodeSF.Forward] = KeyStateSF.Down;
                            }
                            else if (((leftHipPos.y - leftFootPos.y > legsLong - 1 && leftFootPos.z - leftHipPos.z - 5 <= 1.8) ||
                                (rightHipPos.y - rightFootPos.y > legsLong - 1 && rightFootPos.z - rightHipPos.z - 5 <= 1.8)) && keyStates[j][(int)KeyCodeSF.Forward] != KeyStateSF.Null)
                            {
                                keyStates[j][(int)KeyCodeSF.Forward] = KeyStateSF.Up;
                            }

                            // Backward
                            if (((leftHipPos.y - leftFootPos.y <= legsLong - 1 && leftFootPos.z - leftHipPos.z - 5 <= -1.8) ||
                               (rightHipPos.y - rightFootPos.y <= legsLong - 1 && rightFootPos.z - rightHipPos.z - 5 <= -1.8)) && keyStates[j][(int)KeyCodeSF.Backward] != KeyStateSF.Stay)
                            {
                                keyStates[j][(int)KeyCodeSF.Backward] = KeyStateSF.Down;
                            }
                            else if (((leftHipPos.y - leftFootPos.y > legsLong - 1 && leftFootPos.z - leftHipPos.z - 5 > -1.8) ||
                                (rightHipPos.y - rightFootPos.y > legsLong - 1 && rightFootPos.z - rightHipPos.z - 5 > -1.8)) && keyStates[j][(int)KeyCodeSF.Backward] != KeyStateSF.Null)
                            {
                                keyStates[j][(int)KeyCodeSF.Backward] = KeyStateSF.Up;
                            }
                        }
                        else if (gameManager.GetOpponent(tag).transform.localScale.x < 0)
                        {
                            // Backward
                            if (((leftHipPos.y - leftFootPos.y <= legsLong - 1 && leftFootPos.z - leftHipPos.z - 5 > 1.8) ||
                                (rightHipPos.y - rightFootPos.y <= legsLong - 1 && rightFootPos.z - rightHipPos.z - 5 > 1.8)) && keyStates[j][(int)KeyCodeSF.Backward] != KeyStateSF.Stay)
                            {
                                keyStates[j][(int)KeyCodeSF.Backward] = KeyStateSF.Down;
                            }
                            else if (((leftHipPos.y - leftFootPos.y > legsLong - 1 && leftFootPos.z - leftHipPos.z - 5 <= 1.8) ||
                                (rightHipPos.y - rightFootPos.y > legsLong - 1 && rightFootPos.z - rightHipPos.z - 5 <= 1.8)) && keyStates[j][(int)KeyCodeSF.Backward] != KeyStateSF.Null)
                            {
                                keyStates[j][(int)KeyCodeSF.Backward] = KeyStateSF.Up;
                            }

                            // Forward
                            if (((leftHipPos.y - leftFootPos.y <= legsLong - 1 && leftFootPos.z - leftHipPos.z - 5 <= -1.8) ||
                               (rightHipPos.y - rightFootPos.y <= legsLong - 1 && rightFootPos.z - rightHipPos.z - 5 <= -1.8)) && keyStates[j][(int)KeyCodeSF.Forward] != KeyStateSF.Stay)
                            {
                                keyStates[j][(int)KeyCodeSF.Forward] = KeyStateSF.Down;
                            }
                            else if (((leftHipPos.y - leftFootPos.y > legsLong - 1 && leftFootPos.z - leftHipPos.z - 5 > -1.8) ||
                                (rightHipPos.y - rightFootPos.y > legsLong - 1 && rightFootPos.z - rightHipPos.z - 5 > -1.8)) && keyStates[j][(int)KeyCodeSF.Forward] != KeyStateSF.Null)
                            {
                                keyStates[j][(int)KeyCodeSF.Forward] = KeyStateSF.Up;
                            }
                        }

                        // Defense
                        if ((Vector3.Distance(leftHandPos, headPos) <= 4 || Vector3.Distance(rightHandPos, headPos) <= 4) && keyStates[j][(int)KeyCodeSF.Defense] != KeyStateSF.Stay)
                        {
                            keyStates[j][(int)KeyCodeSF.Defense] = KeyStateSF.Down;
                        }
                        else if ((Vector3.Distance(leftHandPos, headPos) > 4 && Vector3.Distance(rightHandPos, headPos) > 4) && keyStates[j][(int)KeyCodeSF.Defense] != KeyStateSF.Null)
                        {
                            keyStates[j][(int)KeyCodeSF.Defense] = KeyStateSF.Up;
                        }

                        //string outputStr = "j State: ";
                        //for (int k = 0; k < Enum.GetValues(typeof(KeyCodeSF)).Length; k++)
                        //{
                        //    if (keyStates[j][k] == KeyStateSF.Stay)
                        //    {
                        //        outputStr += k + " ";
                        //    }
                        //}
                        //Debug.Log(outputStr);

                        j++;
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

    public bool GetKeyDown(KeyCodeSF keyCode, int index = 0)
    {
        if (index < 0 || index >= keyStates.Count)
        {
            Debug.LogError("Error! index is over range, keyStates count: " + keyStates.Count);
            return false;
        }

        return keyStates[index][(int)keyCode] == KeyStateSF.Down;
    }

    public bool GetKeyStay(KeyCodeSF keyCode, int index = 0)
    {
        if (index < 0 || index >= keyStates.Count)
        {
            Debug.LogError("Error! index is over range, keyStates count: " + keyStates.Count);
            return false;
        }

        return keyStates[index][(int)keyCode] == KeyStateSF.Stay;
    }

    public bool GetKeyUp(KeyCodeSF keyCode, int index = 0)
    {
        if (index < 0 || index >= keyStates.Count)
        {
            Debug.LogError("Error! index is over range, keyStates count: " + keyStates.Count);
            return false;
        }

        return keyStates[index][(int)keyCode] == KeyStateSF.Up;
    }

    public bool GetKeyNull(KeyCodeSF keyCode, int index = 0)
    {
        if (index < 0 || index >= keyStates.Count)
        {
            Debug.LogError("Error! index is over range, keyStates count: " + keyStates.Count);
            return false;
        }

        return keyStates[index][(int)keyCode] == KeyStateSF.Null;
    }
}
