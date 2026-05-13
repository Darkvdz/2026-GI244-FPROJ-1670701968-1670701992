using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{

    public static GameManager instance;

    private int currentPlayer = 0;
    public int CurrentPlayer
    {
        get { return currentPlayer; }
        set { currentPlayer = value; }
    }

    private int roomPlayer = 0;
    public int RoomPlayer
    {
        get { return roomPlayer; }
        set { roomPlayer = value; }
    }


    [SerializeField] private string[] maps; //this is error because it singleton why u use SerializeField!!!!!!

    private bool IsDead(Player player)
    {
        return player.CustomProperties.ContainsKey("dead") &&
               (bool)player.CustomProperties["dead"];
    }


    private void Awake()
    {
        if (instance != null) 
        {
                
            Destroy(gameObject);
            //print("1s");
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        roomPlayer = PhotonNetwork.PlayerList.Length;



    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.V))
        {
            ShowAllPlayerData();
        }
    }


    private void AddProperties() 
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props["score"] = 0;
        props["dead"] = false;

        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    void ShowAllPlayerData()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            int score = player.CustomProperties.ContainsKey("score")
                ? (int)player.CustomProperties["score"]
                : 0;

            bool dead = player.CustomProperties.ContainsKey("dead")
                ? (bool)player.CustomProperties["dead"]
                : false;

            Debug.Log(
                "Name: " + player.NickName +
                " | Score: " + score +
                " | Dead: " + dead
            );
        }
    }


    public void StartCheckLastPlayer()
    { 
        StartCoroutine(CheckLastPlayer());
    }

    IEnumerator CheckLastPlayer()
    {

        print("start check player");
        yield return new WaitForSeconds(3f);

        var alivePlayers = PhotonNetwork.PlayerList
           .Where(p => !IsDead(p))   
           .ToList();

        print(alivePlayers.Count);

        if (alivePlayers.Count == 1)
        {
            Player winner = alivePlayers[0];

            Debug.Log("Winner: " + winner.NickName);

            AddScore(winner);
            GameNetWorkManager.instance.CheckGameEnd(winner);
        }

        yield return null;
    }

    void AddScore(Player player)
    {

        int current = player.CustomProperties.ContainsKey("score")
        ? (int)player.CustomProperties["score"]
        : 0;

        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props["score"] = current + 1;

        player.SetCustomProperties(props);

    }


    public void ResetDeadStatus()
    {
        foreach (var p in PhotonNetwork.PlayerList)
        {
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
            props["dead"] = false;

            p.SetCustomProperties(props);
        }
    }

    public void EndGame(string winnerName)
    {
        Debug.Log("Game Winner: " + winnerName);

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("MainMenu"); 
        }

        Destroy(gameObject);
    }
        
    public string GetRandomScene()
    {
        return maps[UnityEngine.Random.Range(0, maps.Length)];
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
           StartCoroutine(CheckLastPlayer());
        }

    }





    /*[PunRPC]
    void SyncScore(int[] scores, bool[] deaths, bool[] active, int deathCount)
    {
        playerScore = scores;
        playerDeath = deaths;
        playerActive = active;
        death = deathCount;

        Debug.Log("Sync data from host");
    }*/








}