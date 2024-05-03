using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Windows.Kinect;

public class UIManager : MonoBehaviour
{
    public Camera uiCamera;
    public Camera mainCamera;

    public Canvas mainMenuCanvas;
    public Canvas seleteGameCanvas;

    public RawImage mMouse;
    private Texture mouseImg;
    public Texture mouseDownImg;
    [Range(1, 100)]
    public int mouseSensitivity;
    [Range(10, 1000)]
    public int magnification;
    //public GameObject mMouse;
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
            //mouseSensitivity = 15;
            //magnification = 200;
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
                        // mMouse.transform.position = new Vector3(handPosition.X * mouseSensitivity, handPosition.Y * mouseSensitivity, 105.1f);
                
                        Vector3 targetPosition = new Vector3(handPosition.X * magnification, handPosition.Y * magnification, 0);
                        targetPosition = mMouse.transform.parent.TransformPoint(targetPosition);
                        mMouse.transform.position = Vector3.Lerp(mMouse.transform.position, targetPosition, mouseSensitivity * Time.deltaTime);

                        UpdataMouseState(body);

                        Debug.Log((int)mouseState);

                        if (mouseState == MouseState.MouseUp)
                        {
                            Collider2D[] colliders = Physics2D.OverlapBoxAll(mMouse.transform.position, mMouse.rectTransform.sizeDelta / 2, 0);
                            //Collider2D[] colliders = Physics2D.OverlapBoxAll(mMouse.transform.position, mMouse.transform.localScale / 2, 0);

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
        // Ãö³¬Kinect
        if (kinectSensor != null)
        {
            kinectSensor.Close();
        }
    }

    public void Click_StartBtn()
    {
        mainMenuCanvas.gameObject.SetActive(false);
        seleteGameCanvas.gameObject.SetActive(true);
    }

    public void Click_QuitBtn()
    {
        if (kinectSensor != null)
        {
            kinectSensor.Close();
        }

        Application.Quit();
    }

    public void Click_BackBtn()
    {
        mainMenuCanvas.gameObject.SetActive(true);
        seleteGameCanvas.gameObject.SetActive(false);
    }

    public void Click_NextBtn()
    {
        mainMenuCanvas.gameObject.SetActive(true);
        seleteGameCanvas.gameObject.SetActive(false);
    }

    public void Click_PreviousBtn()
    {
        mainMenuCanvas.gameObject.SetActive(true);
        seleteGameCanvas.gameObject.SetActive(false);
    }

   
}
