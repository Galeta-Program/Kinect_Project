using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Windows.Kinect;
using TMPro;

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

    public GameObject mainMeunBG;
    public GameObject seleteGameBG;
    public GameObject[] uiObjects;


    private MouseState mouseState = MouseState.MouseHover;

    private KinectSensor kinectSensor;
    private BodyFrameReader bodyFrameReader;
    private Body[] bodies;

    const float oriWidth  = 673f;
    const float oriHeight = 438f;

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

        if (mainMeunBG != null)
        {
            mainMeunBG.transform.parent.GetComponent<RectTransform>().sizeDelta
                = mainMeunBG.transform.parent.transform.parent.GetComponent<RectTransform>().sizeDelta;

            mainMeunBG.transform.parent.GetComponent<RectTransform>().position
                = mainMeunBG.transform.parent.transform.parent.GetComponent<RectTransform>().position;

            mainMeunBG.GetComponent<RectTransform>().sizeDelta
                = mainMeunBG.transform.parent.transform.parent.GetComponent<RectTransform>().sizeDelta;
        }

        if (seleteGameBG != null)
        {
            seleteGameBG.transform.parent.GetComponent<RectTransform>().sizeDelta
                = seleteGameBG.transform.parent.transform.parent.GetComponent<RectTransform>().sizeDelta;

            seleteGameBG.transform.parent.GetComponent<RectTransform>().position
                = seleteGameBG.transform.parent.transform.parent.GetComponent<RectTransform>().position;

            seleteGameBG.GetComponent<RectTransform>().sizeDelta
                = seleteGameBG.transform.parent.transform.parent.GetComponent<RectTransform>().sizeDelta;
        }

        foreach (GameObject gameObject in uiObjects)
        {
            if (gameObject != null)
            {
                float wRatio = gameObject.transform.parent.transform.parent.GetComponent<RectTransform>().sizeDelta.x / oriWidth;
                float hRatio = gameObject.transform.parent.transform.parent.GetComponent<RectTransform>().sizeDelta.y / oriHeight;
                float avgRatio = (wRatio + hRatio) / 2f;
                gameObject.GetComponent<RectTransform>().sizeDelta = gameObject.GetComponent<RectTransform>().sizeDelta * avgRatio;

                //Debug.Log(gameObject.transform.parent.InverseTransformPoint(gameObject.GetComponent<RectTransform>().position));

                Vector3 newPosition
                    = gameObject.transform.parent.InverseTransformPoint(gameObject.GetComponent<RectTransform>().position) * avgRatio;
                gameObject.GetComponent<RectTransform>().position = gameObject.transform.parent.TransformPoint(newPosition);

                TMP_Text text = gameObject.GetComponentInChildren<TMP_Text>();

                if (text != null)
                {
                    text.fontSize = (int)(text.fontSize * avgRatio);
                }
            }
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
