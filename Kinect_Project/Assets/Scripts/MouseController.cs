using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Windows.Kinect;
using TMPro;

public class MouseController : MonoBehaviour
{
    public RawImage mMouse;
    private Texture mouseImg;
    public Texture mouseDownImg;
    [Range(1, 1000)]
    public int mouseSensitivity;
    [Range(10, 10000)]
    public int magnification;
    private MouseState mouseState = MouseState.MouseHover;

    private KinectSensor kinectSensor;
    private BodyFrameReader bodyFrameReader;
    private Body[] bodies;

    enum MouseState
    {
        None,
        MouseDown,
        MouseHover,
        MouseUp,
    }

    // Start is called before the first frame update
    void Start()
    {
        kinectSensor = KinectSensor.GetDefault();

        if (kinectSensor != null)
        {
            kinectSensor.Open();
            bodyFrameReader = kinectSensor.BodyFrameSource.OpenReader();
            bodies = new Body[kinectSensor.BodyFrameSource.BodyCount];

            if (mMouse.texture != null)
                mouseImg = mMouse.texture;
        }
        else
        {
            Debug.LogError("No Kinect sensor found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (kinectSensor != null && !kinectSensor.IsOpen)
        {
            kinectSensor.Open();
        }

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

                        var handPosition = body.Joints[JointType.HandRight].Position;
                        Vector3 targetPosition = new Vector3(handPosition.X * magnification, handPosition.Y * magnification, 0);
                        targetPosition = mMouse.transform.parent.TransformPoint(targetPosition);
                        mMouse.transform.position = Vector3.Lerp(mMouse.transform.position, targetPosition, mouseSensitivity * Time.deltaTime);

                        UpdataMouseState(body);

                        Debug.Log((int)mouseState);

                        if (mouseState == MouseState.MouseUp)
                        {
                            Collider2D[] colliders = Physics2D.OverlapBoxAll(mMouse.transform.position, mMouse.rectTransform.sizeDelta / 2, 0);
                            Debug.Log("Size:" + colliders.Length);
                            foreach (Collider2D collider in colliders)
                            {
                                Button cButton = collider.GetComponent<Button>();

                                if (cButton != null)
                                {
                                    cButton.onClick.Invoke();
                                }
                            }
                        }
                        else if (mouseState == MouseState.MouseDown && mouseDownImg != null)
                        {
                            mMouse.texture = mouseDownImg;
                        }

                        if (mMouse.texture != mouseImg && mouseState != MouseState.MouseDown)
                        {
                            mMouse.texture = mouseImg;
                        }
                    }
                }
            }
        }
    }

    void UpdataMouseState(Body body)
    {
        if (body.HandRightState == HandState.Closed && mouseState == MouseState.MouseHover)
        {
            mouseState = MouseState.MouseDown;
        }
        else if (body.HandRightState == HandState.Open && mouseState == MouseState.MouseDown)
        {
            mouseState = MouseState.MouseUp;
        }
        else if (mouseState != MouseState.MouseDown)
        {
            mouseState = MouseState.MouseHover;
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
