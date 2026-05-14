using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using System.Collections;

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

    private string deathText = "Dead";


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        print("update UI start");
        StartCoroutine(UpdateUI());
    }

    public void CallUpdateUI() 
    {
        StartCoroutine(UpdateUI());
    }

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

            string name = p.NickName;

            int score = p.CustomProperties.ContainsKey("score")
                ? (int)p.CustomProperties["score"]
                : 0;

            bool dead = p.CustomProperties.ContainsKey("dead")
                ? (bool)p.CustomProperties["dead"]
                : false;

            SetPlayerUI(i, name, score, dead);
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
