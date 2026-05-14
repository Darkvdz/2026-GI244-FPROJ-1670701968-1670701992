using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviourPunCallbacks
{

    public static UIManager Instance;

    public TextMeshProUGUI blueName;
    public TextMeshProUGUI score;
    public GameObject blueDetails;
    public TextMeshProUGUI blueNon;

    public TextMeshProUGUI yellowName;
    public TextMeshProUGUI yellowScore;
    public GameObject yellowDetails;
    public TextMeshProUGUI yellowNon;

    public TextMeshProUGUI greenName;
    public TextMeshProUGUI greenScore;
    public GameObject greenDetails;
    public TextMeshProUGUI greenNon;

    public TextMeshProUGUI redName;
    public TextMeshProUGUI redScore;
    public GameObject redDetails;
    public TextMeshProUGUI redNon;

    public Button resumeButton;
    public Button settingButton;
    public Button leaveButton;

    public Button closeButton;

    public GameObject meneUI;
    public GameObject settingUI;



    private string deathText = "Dead";


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        resumeButton.onClick.AddListener(() =>
        {
            meneUI.SetActive(false);
        });

        settingButton.onClick.AddListener(() =>
        {
            settingUI.SetActive(true);
        });

        leaveButton.onClick.AddListener(() =>
        {
            meneUI.SetActive(false);
            PhotonNetwork.LeaveRoom();
        });

        closeButton.onClick.AddListener(() =>
        {
            settingUI.SetActive(false);
        });

    }

    public override void OnLeftRoom()
    {
        if (GameManager.instance != null)
        {
            Destroy(GameManager.instance.gameObject);
        }
        

        PhotonNetwork.LoadLevel("MainMenu");
    }

    private void Start()
    {
        print("update UI start");
        StartCoroutine(UpdateUI());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            if (settingUI.activeSelf)
            {
                settingUI.SetActive(false);
                meneUI.SetActive(false);
            }
            else
            {
                meneUI.SetActive(!meneUI.activeSelf);
            }
        }
    }

    public void CallUpdateUI() 
    {
        StartCoroutine(UpdateUI());
    }

    /*IEnumerator UpdateUI()
    {
        yield return new WaitForSeconds(1f);

        var players = PhotonNetwork.PlayerList
            .OrderBy(p => p.ActorNumber)
            .ToArray();

        ResetAll();

        for (int i = 0; i < players.Length; i++)
        {
            Player p = players[i];

            string name = p.NickName;

            int score = p.CustomProperties.ContainsKey("score")
                ? (int)p.CustomProperties["score"]
                : 0;

            bool dead = p.CustomProperties.ContainsKey("dead")
                ? (bool)p.CustomProperties["dead"]
                : false;

            SetPlayerUI(i, name, score, dead);
        }
    }*/

    IEnumerator UpdateUI()
    {
        yield return new WaitForSeconds(1f);

        var players = PhotonNetwork.PlayerList
            .OrderBy(p => p.ActorNumber)
            .ToArray();

        ResetAll();

        for (int i = 0; i < players.Length; i++)
        {
            Player p = players[i];

            int slot = p.CustomProperties.ContainsKey("slot")
                ? (int)p.CustomProperties["slot"]
                : -1;

            string name = p.NickName;

            int score = p.CustomProperties.ContainsKey("score")
                ? (int)p.CustomProperties["score"]
                : 0;

            bool dead = p.CustomProperties.ContainsKey("dead")
                ? (bool)p.CustomProperties["dead"]
                : false;

            SetPlayerUI(slot, name, score, dead);
        }
    }
    public void SetPlayerUI(int index, string name, int scoreValue, bool dead)
    {
        switch (index)
        {
            case 0:

                if (dead)
                {
                    blueNon.text = deathText;
                    blueDetails.SetActive(false);
                    blueNon.gameObject.SetActive(true);
                }
                else
                {

                    blueName.text = name;
                    score.text = scoreValue.ToString();
                    blueDetails.SetActive(true);
                    blueNon.gameObject.SetActive(false);
                }
                break;

            case 1:

                if (dead)
                {
                    yellowNon.text = deathText;
                    yellowDetails.SetActive(false);
                    yellowNon.gameObject.SetActive(true);
                }
                else
                {
                    yellowName.text = name;
                    yellowScore.text = scoreValue.ToString();
                    yellowDetails.SetActive(true);
                    yellowNon.gameObject.SetActive(false);
                }

                break;

            case 2:

                if (dead)
                {
                    greenNon.text = deathText;
                    greenDetails.SetActive(false);
                    greenNon.gameObject.SetActive(true);
                }
                else
                {
                    greenName.text = name;
                    greenScore.text = scoreValue.ToString();
                    greenDetails.SetActive(true);
                    greenNon.gameObject.SetActive(false);
                }

                break;

            case 3:

                if (dead)
                {
                    redNon.text = deathText;
                    redDetails.SetActive(false);
                    redNon.gameObject.SetActive(true);
                }
                else
                {
                    redName.text = name;
                    redScore.text = scoreValue.ToString();
                    redDetails.SetActive(true);
                    redNon.gameObject.SetActive(false);
                }

                break;
        }
    }

    public void ResetAll()
    {
        blueDetails.SetActive(false);
        yellowDetails.SetActive(false);
        greenDetails.SetActive(false);
        redDetails.SetActive(false);

        blueNon.gameObject.SetActive(true);
        yellowNon.gameObject.SetActive(true);
        greenNon.gameObject.SetActive(true);
        redNon.gameObject.SetActive(true);
    }


}
