using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SwordGameManager : MonoBehaviour
{
    [SerializeField] public SwrodJointsCatcher jointsCatcher;
    [SerializeField] public SwordCharacter player1;
    [SerializeField] public SwordCharacter player2;
    [SerializeField] public Canvas canvas;
    [SerializeField] public GameObject MenuUI;
    [SerializeField] public GameObject SwordGameObject;
    [SerializeField] public AudioSource countdownAudio;
    [SerializeField] public AudioSource winningAudio;

    public Dictionary<string, int> mode = new Dictionary<string, int>();
    public int currentMode = 0;
    private bool starting = false;
    private bool ending = false;
    private float countdown = 0f;

    // Start is called before the first frame update
    void Start()
    {
        mode["ready"] = 0;
        mode["playing"] = 1;
        mode["end"] = 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (starting)
        {
            countdown += Time.deltaTime;
            if(countdown > 1.1f)
            {
                countdown -= 1f;
                TextMeshProUGUI ready = canvas.transform.Find("ready").gameObject.GetComponent<TextMeshProUGUI>();
                if (ready.text == "Ready")
                {
                    ready.text = "3";
                    countdownAudio.Play();
                }
                else if(ready.text == "3")
                {
                    ready.text = "2";
                }
                else if (ready.text == "2")
                {
                    ready.text = "1";
                }
                else if (ready.text == "1")
                {
                    canvas.gameObject.SetActive(false);
                    ready.text = "Ready";

                    currentMode = mode["playing"];
                    starting = false;
                }
            }
            return;
        }

        if (currentMode == mode["ready"])
        {
            if (jointsCatcher.jointSpeeds[0].tracked && jointsCatcher.jointSpeeds[1].tracked) starting = true;

            for (int i = 0; i < 2; i++)
            {
                GameObject ready = canvas.transform.Find("ready").GetChild(i).gameObject;
                ready.SetActive(true);
                if (jointsCatcher.jointSpeeds[i].tracked)
                {
                    ready.transform.Find("check").gameObject.SetActive(true);
                    ready.transform.Find("uncheck").gameObject.SetActive(false);
                }
                else
                {
                    ready.transform.Find("check").gameObject.SetActive(false);
                    ready.transform.Find("uncheck").gameObject.SetActive(true);
                }
            }
        }
        else if(currentMode == mode["playing"])
        {
            if(ending)
            {
                countdown += Time.deltaTime;
                if(countdown > 2f)
                {
                    countdown = 0;
                    currentMode = mode["end"];
                    ending = false;
                    winningAudio.Play();
                }
                return;
            }

            if (player1.fall || player2.fall)
            {
                ending = true;
                countdown = 0;
            }
        }
        else if(currentMode == mode["end"])
        {
            canvas.gameObject.SetActive(true);
            canvas.transform.Find("ready").gameObject.SetActive(false);
            Transform winorlose = canvas.transform.Find("winorlose");
            winorlose.gameObject.SetActive(true);
            if (player1.fall)
            {
                winorlose.GetChild(0).Find("lose").gameObject.SetActive(true);
                winorlose.GetChild(1).Find("win").gameObject.SetActive(true);
            }
            else if (player2.fall)
            {
                winorlose.GetChild(0).Find("win").gameObject.SetActive(true);
                winorlose.GetChild(1).Find("lose").gameObject.SetActive(true);
            }

            countdown += Time.deltaTime;
            if(countdown > 5f)
            {
                countdown = 0;
                currentMode = mode["ready"];
                canvas.transform.Find("ready").gameObject.SetActive(true);
                winorlose.gameObject.SetActive(false);
                player1.resetStatus();
                player2.resetStatus();
                MenuUI.SetActive(true);
                SwordGameObject.SetActive(false);
            }
        }
    }
}
