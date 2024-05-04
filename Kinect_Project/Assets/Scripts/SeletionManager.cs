using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeletionManager : MonoBehaviour
{
    [System.Serializable]
    public class GameSeletion
    {
        public Camera gameCamera;
        public Texture2D gameImage;
        public string detail;
    }

    private class ButtonWithID
    {
        public int buttonID;
        public Button button;
    }

    public Font textFont;

    [Range(0.1f, 50.0f)]
    public float seleteSpeed;
    public GameSeletion[] gameSeletions;
    
    private List<ButtonWithID> seletionBtns = new List<ButtonWithID>();

    private bool leftMoving = false;
    private bool rightMoving = false;

    const float oriWidth = 673f;
    const float oriHeight = 438f;
    float avgRatio = 1;

    void Start()
    {
        //seleteSpeed = 5;

        float wRatio = gameObject.transform.parent.transform.parent.GetComponent<RectTransform>().sizeDelta.x / oriWidth;
        float hRatio = gameObject.transform.parent.transform.parent.GetComponent<RectTransform>().sizeDelta.y / oriHeight;
        avgRatio = (wRatio + hRatio) / 2f;

        int index = 0;
        foreach (GameSeletion seletion in gameSeletions)
        {
            ButtonWithID button = CreateButton(index++, seletion.gameCamera, seletion.gameImage, seletion.detail);
            seletionBtns.Add(button);
        }

    }

    void Update()
    {
        if (leftMoving && !rightMoving)
        {
            int seletionIndex = 0;

            for (int i = 0; i < seletionBtns.Count; i++)
            {
                if (seletionBtns[i].buttonID == 0)
                {
                    seletionBtns[i].button.GetComponent<RectTransform>().sizeDelta
                        = Vector2.Lerp(seletionBtns[i].button.GetComponent<RectTransform>().sizeDelta, new Vector2(150, 100) * avgRatio, Time.deltaTime * seleteSpeed);
                }
                else if (seletionBtns[i].buttonID == 1)
                {
                    seletionIndex = i;
                    seletionBtns[i].button.GetComponent<RectTransform>().sizeDelta
                        = Vector2.Lerp(seletionBtns[i].button.GetComponent<RectTransform>().sizeDelta, new Vector2(300, 200) * avgRatio, Time.deltaTime * seleteSpeed);
                }

                Vector3 targetPosition = new Vector3(250f * (seletionBtns[i].buttonID - 1), 30f, 0f) * avgRatio;
                targetPosition = seletionBtns[i].button.transform.parent.TransformPoint(targetPosition);

                seletionBtns[i].button.transform.position
                    = Vector3.Lerp(seletionBtns[i].button.transform.position, targetPosition, Time.deltaTime * seleteSpeed);
            }

            Vector3 transPosition = seletionBtns[seletionIndex].button.transform.parent.TransformPoint(new Vector3(2, 0, 0));
            float px = transPosition.x;

            if (seletionBtns[seletionIndex].button.transform.position.x >= -px && seletionBtns[seletionIndex].button.transform.position.x <= px)
            {
                Debug.Log("MoveEnd");
                leftMoving = false;

                for (int i = 0; i < seletionBtns.Count; i++)
                {
                    seletionBtns[i].button.transform.localPosition = new Vector3(250 * (seletionBtns[i].buttonID - 1), 30, 0) * avgRatio;

                    if (seletionBtns[i].buttonID == 0)
                        seletionBtns[i].button.transform.localPosition = new Vector3(-250, 30, 0) * avgRatio;

                    if (seletionBtns[i].buttonID == 1)
                        seletionBtns[i].button.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 200) * avgRatio;
                    else
                        seletionBtns[i].button.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 100) * avgRatio;
                }

                for (int i = 0; i < seletionBtns.Count; i++)
                    seletionBtns[i].buttonID = seletionBtns[i].buttonID - 1 >= 0 ? seletionBtns[i].buttonID - 1 : seletionBtns.Count - 1;
            }
        }

        if (rightMoving && !leftMoving)
        {
            int seletionIndex = 0;

            for (int i = 0; i < seletionBtns.Count; i++)
            {
                if (seletionBtns[i].buttonID == 0)
                {
                    seletionBtns[i].button.GetComponent<RectTransform>().sizeDelta
                        = Vector2.Lerp(seletionBtns[i].button.GetComponent<RectTransform>().sizeDelta, new Vector2(150, 100) * avgRatio, Time.deltaTime * seleteSpeed);
                }
                else if (seletionBtns[i].buttonID == seletionBtns.Count - 1)
                {
                    seletionIndex = i;
                    seletionBtns[i].button.GetComponent<RectTransform>().sizeDelta
                        = Vector2.Lerp(seletionBtns[i].button.GetComponent<RectTransform>().sizeDelta, new Vector2(300, 200) * avgRatio, Time.deltaTime * seleteSpeed);
                }

                int nextID = seletionBtns[i].buttonID + 1;

                if (nextID >= seletionBtns.Count)
                {
                    nextID = 0;
                }
                else if (nextID == seletionBtns.Count - 1)
                {
                    nextID = -1;
                }

                Vector3 targetPosition = new Vector3(250f * nextID, 30f, 0f) * avgRatio;
                targetPosition = seletionBtns[i].button.transform.parent.TransformPoint(targetPosition);

                seletionBtns[i].button.transform.position
                    = Vector3.Lerp(seletionBtns[i].button.transform.position, targetPosition, Time.deltaTime * seleteSpeed);
            }

            Vector3 transPosition 
                = seletionBtns[seletionIndex].button.transform.parent.InverseTransformPoint(seletionBtns[seletionIndex].button.transform.position);
            
            if (transPosition.x >= -2 && transPosition.x <= 2)
            {
                rightMoving = false;

                for (int i = 0; i < seletionBtns.Count; i++)
                {
                    int nextID = seletionBtns[i].buttonID + 1;

                    if (nextID >= seletionBtns.Count)
                    {
                        nextID = 0;
                    }
                    else if (nextID == seletionBtns.Count - 1)
                    {
                        nextID = -1;
                    }

                    seletionBtns[i].button.transform.localPosition = new Vector3(250 * nextID, 30, 0) * avgRatio;

                    if (seletionBtns[i].buttonID == seletionBtns.Count - 1)
                        seletionBtns[i].button.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 200) * avgRatio;
                    else
                        seletionBtns[i].button.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 100) * avgRatio;
                }

                for (int i = 0; i < seletionBtns.Count; i++)
                    seletionBtns[i].buttonID = seletionBtns[i].buttonID + 1 < seletionBtns.Count ? seletionBtns[i].buttonID + 1 : 0;
            }
        }
    }

    ButtonWithID CreateButton(int index, Camera gameCamera, Texture2D gameImage, string gameText)
    {        
        GameObject buttonGO = new GameObject("Button");
        buttonGO.transform.SetParent(transform);

        ButtonWithID buttonWID = new ButtonWithID();
        buttonWID.button = buttonGO.AddComponent<Button>();
        buttonWID.buttonID = index;
        buttonWID.button.onClick.AddListener(() => Switch2TargetCamera(buttonWID, gameCamera));

        Image aImage = buttonGO.AddComponent<Image>();
        if (gameImage != null) 
            aImage.sprite = Sprite.Create(gameImage, new Rect(0, 0, gameImage.width, gameImage.height), Vector2.zero);

        BoxCollider2D boxCollider2D = buttonGO.AddComponent<BoxCollider2D>();

        RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();
        buttonRect.localScale = new Vector3(1, 1, 1);
        buttonRect.localPosition = new Vector3(250 * index, 30, 0) * avgRatio;

        if (index == gameSeletions.Length - 1 && gameSeletions.Length > 2)
            buttonRect.localPosition = new Vector3(-250, 30, 0) * avgRatio;

        if (index == 0)
            buttonRect.sizeDelta = new Vector2(300, 200) * avgRatio;
        else
            buttonRect.sizeDelta = new Vector2(150, 100) * avgRatio;

        GameObject buttonTextGO = new GameObject("Text");
        buttonTextGO.transform.SetParent(buttonGO.transform);
        Text buttonTextComponent = buttonTextGO.AddComponent<Text>();
        buttonTextComponent.text = gameText;
        buttonTextComponent.font = textFont;
        buttonTextComponent.fontSize = (int)(20 * avgRatio);
        buttonTextComponent.raycastTarget = false;

        if (index != 0)
        {
            Color textColor = buttonTextComponent.color;
            textColor.a = 0f;
            buttonTextComponent.color = textColor;
        }
        
        RectTransform buttonTextRect = buttonTextGO.GetComponent<RectTransform>();
        buttonTextRect.localScale = new Vector3(1, 1, 1);
        buttonTextRect.sizeDelta = new Vector2(650, 150) * avgRatio;
        buttonTextRect.localPosition = new Vector3(0, -200, 0) * avgRatio;
        
        return buttonWID;
    }

    void Switch2TargetCamera(ButtonWithID buttonWithID, Camera targetCamera)
    {    
        int index = buttonWithID.buttonID;
        Debug.Log(index);

        if (index == 0 && targetCamera != null)
        {
            // 禁用所有相機
            GameObject[] mainCameras = GameObject.FindGameObjectsWithTag("MainCamera");

            foreach (GameObject cameraObject in mainCameras)
            {
                Camera camera = cameraObject.GetComponent<Camera>();

                if (camera != null)
                {
                    camera.gameObject.SetActive(false);
                }
            }

            for (int i = 0; i < gameSeletions.Length; i++)
            {
                if (gameSeletions[i].gameCamera != null)
                {
                    gameSeletions[i].gameCamera.gameObject.SetActive(false);
                }
            }

            // 啟用目標相機
            targetCamera.gameObject.SetActive(true);
        }
        else if (index == 1 && !leftMoving && !rightMoving)
        {
            Debug.Log("LeftMove");
            SeletionsLeftMove();
        }
        else if (index == seletionBtns.Count - 1 && !leftMoving && !rightMoving)
        {
            Debug.Log("RightMove");
            SeletionsRightMove();
        }
        
        
    }

    void SeletionsLeftMove()
    {
        for (int i = 0; i < seletionBtns.Count; i++)
        {
            if (seletionBtns[i].buttonID == 1)
            {
                Color textColor = seletionBtns[i].button.GetComponentInChildren<Text>().color;
                textColor.a = 255f;
                seletionBtns[i].button.GetComponentInChildren<Text>().color = textColor;
            }
            else if (seletionBtns[i].buttonID == 0)
            {
                Color textColor = seletionBtns[i].button.GetComponentInChildren<Text>().color;
                textColor.a = 0f;
                seletionBtns[i].button.GetComponentInChildren<Text>().color = textColor;
            }
        }
          
        leftMoving = true;       
    }

    void SeletionsRightMove()
    {
        for (int i = 0; i < seletionBtns.Count; i++)
        {
            if (seletionBtns[i].buttonID == seletionBtns.Count - 1)
            {
                Color textColor = seletionBtns[i].button.GetComponentInChildren<Text>().color;
                textColor.a = 255f;
                seletionBtns[i].button.GetComponentInChildren<Text>().color = textColor;
            }
            else if (seletionBtns[i].buttonID == 0)
            {
                Color textColor = seletionBtns[i].button.GetComponentInChildren<Text>().color;
                textColor.a = 0f;
                seletionBtns[i].button.GetComponentInChildren<Text>().color = textColor;
            }
            else if (seletionBtns[i].buttonID == 2)
            {
                seletionBtns[i].button.transform.localPosition = new Vector3(-500, 30, 0);
            }
        }

        rightMoving = true;
    }

}
