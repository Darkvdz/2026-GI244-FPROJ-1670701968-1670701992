using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{

    public static GameManager instance;

    [SerializeField] private int[] playerScore = { 0, 0, 0, 0 };
    public int[] PlayerScore
    {
        get { return playerScore; }
        set { playerScore = value; }
    }

    private bool[] playerDeath = { false, false, false, false};
    public bool[] PlayerDeath
    {
        get { return playerDeath; }
        set { playerDeath = value; }
    }

    private bool[] playerActive = { false, false, false, false };
    public bool[] PlayerActive
    {
        get { return playerActive; }
        set { playerActive = value; }
    }

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

    private int death = 0;
    public int Death
    {
        get { return death; }
        set { death = value; }
    }

    private string[] maps = { "GameScene" };


    private void Awake()
    {
        if (instance != null) 
        {
                
            Destroy(gameObject);
            print("1s");
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        roomPlayer = PhotonNetwork.PlayerList.Length;
        for (int i = 0; i < roomPlayer; i++) 
        {
            playerActive[1] = true;
        }



        print(roomPlayer);

        //SceneManager.sceneLoaded += OnSceneLoaded;

    }



    /*void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        print("1f");
        if (PhotonNetwork.IsMasterClient)
        {
            ResetDeadStatus();
        }

        print("1a");
        PlayerManager.instance.SpawnPlayer();

    }*/


    public void CheckLastPlayer(int idPlayer)
    {
        death++;
        playerDeath[idPlayer - 1] = true;


        if (death >= (currentPlayer - 1) )
        {
            print("win");

            int winnerIndex = GetWinnerIdex();

            string Winner = PlayerManager.instance.GetPlayerName(winnerIndex);

            Debug.Log("Winner: " + Winner);

            AddScore(winnerIndex);

            GameNetWorkManager.instance.CheckGameEnd(winnerIndex);
        }
    }

    public int GetWinnerIdex() 
    {
        for (int i = 0; i < roomPlayer; i++) 
        {
            if (!playerDeath[i] && playerActive[i]) 
            {
                return i + 1;
            }
        }
        return 5;
    }

    void AddScore(int idPlayer)
    {
        playerScore[idPlayer - 1] += 1;
    }


    public void ResetDeadStatus()
    {
        death = 0;
        Array.Fill(playerDeath, false);
    }

    public void EndGame(string winnerName)
    {
        Debug.Log("Game Winner: " + winnerName);

        // WIP
    }
        
    public string GetRandomScene()
    {
        return maps[UnityEngine.Random.Range(0, maps.Length)];
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            currentPlayer -= 1;

            if ((playerDeath[otherPlayer.ActorNumber - 1])) 
            {
                death += 1;
            }
            playerActive[otherPlayer.ActorNumber - 1] = false;

        }
    }









}