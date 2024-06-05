using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Windows.Kinect;
using System;
using TMPro;

public class UIManagerSF : MonoBehaviour
{
    public GameObject mainMenuGO;
    public GameObject gameGO;

    public GameObject uiInterface;
    public GameObject mainInterface;

    public Canvas mainMenuCanvas;
    public Canvas settingMenuCanvas;
    public Canvas seleteGameCanvas;

    public GameManagerSF gameManager;

    public GameObject[] settingGOs;
    private int settingGOIndex = 0;

    public Button _1PlayerBtn;
    public Button _2PlayerBtn;
    public Button settingBtn;
    public Button exitBtn;

    [Range(10, 1000)]
    public int magnification;
    public int player1_roleIndex = 1;
    public int player2_roleIndex = 1;
    public Sprite[] seleteSprites;
    public GameObject[] seleteBtns;
    public GameObject[] seleteBtns2;

    private List<KeyStateSF> mouseStates = new List<KeyStateSF>();

    private KinectSensor kinectSensor;
    private BodyFrameReader bodyFrameReader;
    private Body[] bodies;
    private int seletionIndex = 0; // 0 = 1 player, 1 = 2 player, 2 = Setting, 3 = Exit
    private List<Vector3> currentPosesRH = new List<Vector3>();

    //const float oriWidth = 673f;
    //const float oriHeight = 438f;
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

        foreach (GameObject btn in seleteBtns)
        {
            btn.SetActive(false);
            btn.GetComponent<Image>().color = new Color(1f, 1f, 1f);
        }

        foreach (GameObject btn in seleteBtns2)
        {
            //Debug.Log("Color:" + btn.GetComponent<Image>().color.ToString());
            btn.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
            btn.SetActive(false);
            
        }

        currentPosesRH.Add(new Vector3(0, 0, 0));
        currentPosesRH.Add(new Vector3(0, 0, 0));
        mouseStates.Add(KeyStateSF.Null);
        mouseStates.Add(KeyStateSF.Null);
    }

    // Update is called once per frame
    void Update()
    {
        int seletionSize = Mathf.Min(seleteBtns.Length, seleteBtns2.Length);

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
                    //Debug.Log(bodies.Length + " OK:" + countOK);

                    if (countOK > mouseStates.Count)
                    {
                        for (int i = 0; i < countOK - mouseStates.Count; i++)
                        {
                            mouseStates.Add(KeyStateSF.Null);
                        }
                    }

                    if (countOK > currentPosesRH.Count)
                    {
                        for (int i = 0; i < countOK - currentPosesRH.Count; i++)
                        {
                            currentPosesRH.Add(new Vector3(0, 0, 0));
                        }
                    }

                    for (int i = 0, j = 0; i < bodies.Length; i++)
                    {
                        if (bodies[i] == null || !bodies[i].IsTracked)
                        {
                            continue;
                        }

                        var handPosition = bodies[i].Joints[JointType.HandRight].Position;
                        Vector3 targetPosition = new Vector3(handPosition.X * magnification, handPosition.Y * magnification, 0);
                        targetPosition = transform.parent.TransformPoint(targetPosition);
                        //Debug.Log(targetPosition);

                        UpdataMouseState(bodies[i], j);

                        Debug.Log(i + " mouseStates" + (int)mouseStates[j]);

                        //Debug.Log((int)mouseState);

                        if (mainMenuCanvas.gameObject.activeSelf)
                        {
                            if (targetPosition.y - currentPosesRH[0].y > 1.5 || targetPosition.y - currentPosesRH[0].y < -1.5)
                            {
                                Color color;

                                switch (seletionIndex)
                                {
                                    case 0:
                                        color = _1PlayerBtn.GetComponent<Image>().color;
                                        _1PlayerBtn.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 1);
                                        break;
                                    case 1:
                                        color = _2PlayerBtn.GetComponent<Image>().color;
                                        _2PlayerBtn.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 1);
                                        break;
                                    case 2:
                                        color = settingBtn.GetComponent<Image>().color;
                                        settingBtn.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 1);
                                        break;
                                    case 3:
                                        color = exitBtn.GetComponent<Image>().color;
                                        exitBtn.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 1);
                                        break;
                                }

                                if (targetPosition.y - currentPosesRH[0].y > 1.5)
                                {
                                    seletionIndex = (seletionIndex - 1 + 4) % 4;
                                    currentPosesRH[0] = targetPosition;
                                }
                                else if (targetPosition.y - currentPosesRH[0].y < -1.5)
                                {
                                    seletionIndex = (seletionIndex + 1) % 4;
                                    currentPosesRH[0] = targetPosition;
                                }
                            }

                            _1PlayerBtn.GetComponent<ButtonHover>().isActive = false;
                            _2PlayerBtn.GetComponent<ButtonHover>().isActive = false;
                            settingBtn.GetComponent<ButtonHover>().isActive = false;
                            exitBtn.GetComponent<ButtonHover>().isActive = false;

                            switch (seletionIndex)
                            {
                                case 0:
                                    _1PlayerBtn.GetComponent<ButtonHover>().isActive = true;
                                    break;
                                case 1:
                                    _2PlayerBtn.GetComponent<ButtonHover>().isActive = true;
                                    break;
                                case 2:
                                    settingBtn.GetComponent<ButtonHover>().isActive = true;
                                    break;
                                case 3:
                                    exitBtn.GetComponent<ButtonHover>().isActive = true;
                                    break;
                            }

                            if (mouseStates[0] == KeyStateSF.Up)
                            {
                                switch (seletionIndex)
                                {
                                    case 0:
                                        _1PlayerBtn.onClick.Invoke();
                                        break;
                                    case 1:
                                        _2PlayerBtn.onClick.Invoke();
                                        break;
                                    case 2:
                                        settingBtn.onClick.Invoke();
                                        break;
                                    case 3:
                                        exitBtn.onClick.Invoke();
                                        break;
                                }
                            }
                        }
                        else if (settingMenuCanvas.gameObject.activeSelf)
                        {
                            if (targetPosition.x - currentPosesRH[0].x > 2)
                            {
                                Click_NextBtn();
                                currentPosesRH[0] = targetPosition;
                            }
                            else if (targetPosition.x - currentPosesRH[0].x < -2)
                            {
                                Click_PreBtn();
                                currentPosesRH[0] = targetPosition;
                            }

                            if (mouseStates[0] == KeyStateSF.Up)
                            {
                                Click_BackBtn();
                            }
                        }
                        else if (seleteGameCanvas.gameObject.activeSelf)
                        {
                            if (targetPosition.x - currentPosesRH[0].x > 1.5)
                            {
                                player1_roleIndex++;

                                if (player1_roleIndex > seletionSize)
                                {
                                    player1_roleIndex = 1;
                                }
                                currentPosesRH[0] = targetPosition;
                            }
                            else if (targetPosition.x - currentPosesRH[0].x < -1.5)
                            {
                                player1_roleIndex--;

                                if (player1_roleIndex < 1)
                                {
                                    player1_roleIndex = seletionSize;
                                }
                                currentPosesRH[0] = targetPosition;
                            }

                            if (targetPosition.x - currentPosesRH[1].x > 1.5 && seletionIndex == 1.5)
                            {
                                player2_roleIndex++;

                                if (player2_roleIndex > seletionSize)
                                {
                                    player2_roleIndex = 1;
                                }
                                currentPosesRH[1] = targetPosition;
                            }
                            else if (targetPosition.x - currentPosesRH[1].x < -1.5 && seletionIndex == 1.5)
                            {
                                player2_roleIndex--;

                                if (player2_roleIndex < 1)
                                {
                                    player2_roleIndex = seletionSize;
                                }
                                currentPosesRH[1] = targetPosition;
                            }
                        }

                        j++;
                    }
                }
            }
        }

        if (seleteGameCanvas.gameObject.activeSelf)
        {
            foreach (GameObject btn in seleteBtns)
            {
                btn.SetActive(false);
            }

            foreach (GameObject btn in seleteBtns2)
            {
                btn.SetActive(false);
            }

            if (player1_roleIndex < 0 && player2_roleIndex < 0)
            {
                mainMenuCanvas.gameObject.SetActive(true);
                seleteGameCanvas.gameObject.SetActive(false);
                uiInterface.SetActive(false);
                mainInterface.SetActive(true);            
                gameManager.isPlayer2UseAI = seletionIndex == 0;
                //Debug.Log("p1:" + (-player1_roleIndex - 1) + " p2:" + (-player2_roleIndex - 1));
                gameManager.player1_character = (CharacterSF)(-player1_roleIndex - 1);
                gameManager.player2_character = (CharacterSF)(-player2_roleIndex - 1);
                player1_roleIndex = 1;
                player2_roleIndex = 1;
                gameManager.GameStart();               
                return;
            }

            if (seletionIndex == 0 && player2_roleIndex > 0)
            {
                player2_roleIndex = -UnityEngine.Random.Range(1, seletionSize + 1);
            }  

            if (player1_roleIndex == player2_roleIndex && player1_roleIndex > 0)
            {
                seleteBtns[player1_roleIndex - 1].SetActive(true);
                seleteBtns[player1_roleIndex - 1].GetComponent<Image>().sprite = seleteSprites[2];
            }
            else
            {
                if (player1_roleIndex > 0)
                {
                    //Debug.Log(player1_roleIndex);
                    seleteBtns[player1_roleIndex - 1].SetActive(true);
                    seleteBtns[player1_roleIndex - 1].GetComponent<Image>().sprite = seleteSprites[0];
                }

                if (player2_roleIndex > 0)
                {
                    seleteBtns[player2_roleIndex - 1].SetActive(true);
                    seleteBtns[player2_roleIndex - 1].GetComponent<Image>().sprite = seleteSprites[1];
                }
            }

            if (player1_roleIndex == player2_roleIndex && player1_roleIndex < 0)
            {
                seleteBtns[-player1_roleIndex - 1].SetActive(false);
                seleteBtns2[-player1_roleIndex - 1].SetActive(true);
                seleteBtns2[-player1_roleIndex - 1].GetComponent<Image>().sprite = seleteSprites[2];
            }
            else
            {
                if (player1_roleIndex < 0)
                {
                    //Debug.Log(player1_roleIndex);
                    if (player2_roleIndex < 0)
                        seleteBtns[-player1_roleIndex - 1].SetActive(false);
                    seleteBtns2[-player1_roleIndex - 1].SetActive(true);
                    seleteBtns2[-player1_roleIndex - 1].GetComponent<Image>().sprite = seleteSprites[0];
                }

                if (player2_roleIndex < 0)
                {
                    if (player1_roleIndex < 0)
                        seleteBtns[-player2_roleIndex - 1].SetActive(false);
                    seleteBtns2[-player2_roleIndex - 1].SetActive(true);
                    seleteBtns2[-player2_roleIndex - 1].GetComponent<Image>().sprite = seleteSprites[1];
                }
            }

            if ((mouseStates[0] == KeyStateSF.Up || Input.GetKey(KeyCode.N)) && player1_roleIndex > 0)
            {
                if (player1_roleIndex > 0)
                    player1_roleIndex = -player1_roleIndex;

                //seleteBtns[player1_roleIndex - 1].GetComponent<Button>().onClick.Invoke();
            }

            if ((mouseStates[1] == KeyStateSF.Up || Input.GetKey(KeyCode.Keypad3)) && player2_roleIndex > 0 && seletionIndex == 1)
            {
                if (player2_roleIndex > 0)
                    player2_roleIndex = -player2_roleIndex;

                //seleteBtns[player2_roleIndex - 1].GetComponent<Button>().onClick.Invoke();
            }
            //Debug.Log("1 sele" + player1_roleIndex + " " + seletionSize);

            if (player1_roleIndex > 0)
            {
                if (Input.GetKeyUp(KeyCode.D))
                {
                    player1_roleIndex++;

                    if (player1_roleIndex > seletionSize)
                    {
                        player1_roleIndex = 1;
                    }
                }
                else if (Input.GetKeyUp(KeyCode.A))
                {
                    player1_roleIndex--;

                    if (player1_roleIndex < 1)
                    {
                        player1_roleIndex = seletionSize;
                    }
                }
            }
            
            if (player2_roleIndex > 0 && seletionIndex == 1)
            {
                if (Input.GetKeyUp(KeyCode.LeftArrow))
                {
                    player2_roleIndex++;

                    if (player2_roleIndex > seletionSize)
                    {
                        player2_roleIndex = 1;
                    }
                }
                else if (Input.GetKeyUp(KeyCode.RightArrow))
                {
                    player2_roleIndex--;

                    if (player2_roleIndex < 1)
                    {
                        player2_roleIndex = seletionSize;
                    }
                }
            }           
        }
    }

    void UpdataMouseState(Body body, int index)
    {
        //Debug.Log("hand:" + (int)body.HandRightState + " mouse:" + (int)mouseStates[index]);
        if (body.HandRightState == HandState.Closed && mouseStates[index] == KeyStateSF.Null)
        {
            mouseStates[index] = KeyStateSF.Down;
        }
        else if (body.HandRightState == HandState.Open && mouseStates[index] == KeyStateSF.Down)
        {
            mouseStates[index] = KeyStateSF.Up;
        }
        else if (mouseStates[index] != KeyStateSF.Down)
        {
            mouseStates[index] = KeyStateSF.Null;
        }
    }

    void OnDestroy()
    {
        if (kinectSensor != null)
        {
            kinectSensor.Close();
        }
    }

    public void Click_1PlayerBtn()
    {
        mainMenuCanvas.gameObject.SetActive(false);
        seleteGameCanvas.gameObject.SetActive(true);
        seletionIndex = 0;

        for (int i = 0; i < mouseStates.Count; i++)
        {
            mouseStates[i] = KeyStateSF.Null;
        }
    }

    public void Click_2PlayerBtn()
    {
        mainMenuCanvas.gameObject.SetActive(false);
        seleteGameCanvas.gameObject.SetActive(true);
        seletionIndex = 1;

        for (int i = 0; i < mouseStates.Count; i++)
        {
            mouseStates[i] = KeyStateSF.Null;
        }
    }

    public void Click_SettingBtn()
    {
        mainMenuCanvas.gameObject.SetActive(false);
        settingMenuCanvas.gameObject.SetActive(true);

        for (int i = 0; i < mouseStates.Count; i++)
        {
            mouseStates[i] = KeyStateSF.Null;
        }
    }

    public void Click_ExitBtn()
    {
        mainMenuGO.SetActive(true);
        uiInterface.SetActive(true);
        mainInterface.SetActive(false);
        gameGO.SetActive(false);

        for (int i = 0; i < mouseStates.Count; i++)
        {
            mouseStates[i] = KeyStateSF.Null;
        }
    }

    public void Click_SeletionBtn()
    {
       
    }

    public void Click_NextBtn()
    {
        if (settingGOs.Length != 0)
        {
            foreach (GameObject gameObject in settingGOs)
            {
                gameObject.SetActive(false);
            }

            settingGOIndex = (settingGOIndex + 1) % settingGOs.Length;
            settingGOs[settingGOIndex].SetActive(true);

            for (int i = 0; i < mouseStates.Count; i++)
            {
                mouseStates[i] = KeyStateSF.Null;
            }
        }       
    }

    public void Click_PreBtn()
    {
        if (settingGOs.Length != 0)
        {
            foreach (GameObject gameObject in settingGOs)
            {
                gameObject.SetActive(false);
            }

            settingGOIndex = (settingGOIndex + 1 + settingGOs.Length) % settingGOs.Length;
            settingGOs[settingGOIndex].SetActive(true);

            for (int i = 0; i < mouseStates.Count; i++)
            {
                mouseStates[i] = KeyStateSF.Null;
            }
        }
    }

    public void Click_BackBtn()
    {
        mainMenuCanvas.gameObject.SetActive(true);
        settingMenuCanvas.gameObject.SetActive(false);

        for (int i = 0; i < mouseStates.Count; i++)
        {
            mouseStates[i] = KeyStateSF.Null;
        }
    }
}
