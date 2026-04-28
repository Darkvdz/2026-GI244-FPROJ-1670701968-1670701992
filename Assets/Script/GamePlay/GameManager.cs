using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public Transform[] spawnPoints;

    public static GameManager instance;

    private void Awake()
    {
        if (instance != null) 
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("Spawn")
            .Select(obj => obj.transform)
            .ToArray();

        SpawnPlayer();

        if (PhotonNetwork.IsMasterClient)
        {
            ResetDeadStatus();
        }
    }

    void SpawnPlayer()
    {
        if (PhotonNetwork.LocalPlayer.TagObject != null) return;

        int index = (PhotonNetwork.LocalPlayer.ActorNumber - 1) % spawnPoints.Length;

        Transform spawnPoint = spawnPoints[index];

        GameObject player = PhotonNetwork.Instantiate(
            "Player/Player2D",
            spawnPoint.position,
            spawnPoint.rotation
        );

        PhotonNetwork.LocalPlayer.TagObject = player;
    }

    public void PlayerDied()
    {
        print("die");
        Hashtable props = new Hashtable();
        props["dead"] = true;

        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        photonView.RPC("CheckLastPlayerRPC", RpcTarget.MasterClient);
    }

    [PunRPC]
    void CheckLastPlayerRPC()
    {
        CheckLastPlayer();
    }

    void CheckLastPlayer()
    {
        print("check");
        var alivePlayers = PhotonNetwork.PlayerList
            .Where(p => !IsDead(p))
            .ToList();

        print(alivePlayers);
        if (alivePlayers.Count == 1)
        {
            print("win");
            Player winner = alivePlayers[0];

            Debug.Log("Winner: " + winner.NickName);

            AddScore(winner);

            CheckGameEnd(winner);
        }
    }

    bool IsDead(Player player)
    {
        return player.CustomProperties.ContainsKey("dead") &&
               (bool)player.CustomProperties["dead"];
    }

    void AddScore(Player player)
    {
        int current = player.CustomProperties.ContainsKey("score")
            ? (int)player.CustomProperties["score"]
            : 0;

        Hashtable props = new Hashtable();
        props["score"] = current + 1;

        player.SetCustomProperties(props);
    }

    void CheckGameEnd(Player player)
    {
        int score = (int)player.CustomProperties["score"];

        if (score >= 3)
        {
            photonView.RPC("EndGame", RpcTarget.All, player.NickName);
        }
        else
        {
            photonView.RPC("StartNextRound", RpcTarget.All);
        }
    }

    [PunRPC]
    void StartNextRound()
    {
        Debug.Log("Next Round");

        if (PhotonNetwork.IsMasterClient)
        {
            ResetDeadStatus();
        }

        PhotonNetwork.LoadLevel(GetRandomScene());
    }

    void ResetDeadStatus()
    {
        foreach (var p in PhotonNetwork.PlayerList)
        {
            Hashtable props = new Hashtable();
            props["dead"] = false;

            p.SetCustomProperties(props);
        }
    }

    [PunRPC]
    void EndGame(string winnerName)
    {
        Debug.Log("Game Winner: " + winnerName);

        // TODO: ไปหน้า result หรือ menu
    }
        
    string GetRandomScene()
    {
        string[] maps = { "GameScene"};
        return maps[Random.Range(0, maps.Length)];
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            CheckLastPlayer();
        }
    }


}