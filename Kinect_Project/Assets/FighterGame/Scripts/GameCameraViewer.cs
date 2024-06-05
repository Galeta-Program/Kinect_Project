using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCameraViewer : MonoBehaviour
{
    public GameManagerSF gameManager;
    public GameObject cameraOnBGGO;
    public GameObject background;

    float minDistance = 2.3f;
    float maxDistance = 5.9f;
    Vector3 oriPos;

    // Start is called before the first frame update
    void Start()
    {
        oriPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        GameObject player1 = gameManager.GetOpponent("Player2");
        GameObject player2 = gameManager.GetOpponent("Player1");

        float nowDistance = Vector3.Distance(player1.transform.position, player2.transform.position);
        
        Vector3 tmpPos = transform.position;
        float newScaleRatio = 0;

        if (nowDistance > minDistance)
        {
            newScaleRatio = (nowDistance - minDistance) * (1.0f / (maxDistance - minDistance));
            // Debug.Log("ScaleRatio" + newScaleRatio + "distance" + nowDistance);
            transform.position = new Vector3(oriPos.x, oriPos.y + newScaleRatio * 0.2f, oriPos.z - newScaleRatio * 1f);            
        }       

        float bgx = background.transform.position.x;
        float camBGGOx = cameraOnBGGO.transform.position.x;
        cameraOnBGGO.GetComponent<Canvas>().planeDistance = Mathf.Abs(background.transform.position.z - transform.position.z);

        if ((player1.transform.position.x + player2.transform.position.x) / 2 + cameraOnBGGO.GetComponent<RectTransform>().sizeDelta.x / 1000 <= bgx + background.GetComponent<RectTransform>().sizeDelta.x / (1100 + newScaleRatio * 500) &&
            (player1.transform.position.x + player2.transform.position.x) / 2 - cameraOnBGGO.GetComponent<RectTransform>().sizeDelta.x / 1000 >= bgx - background.GetComponent<RectTransform>().sizeDelta.x / (1100 + newScaleRatio * 500))
        {
            transform.position = new Vector3((player1.transform.position.x + player2.transform.position.x) / 2, transform.position.y, transform.position.z);
        }
        else
        {
            transform.position = tmpPos;
        }

        cameraOnBGGO.GetComponent<Canvas>().planeDistance = Mathf.Abs(background.transform.position.z - transform.position.z);
    }
}

