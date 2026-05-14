using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    public static PlayerManager instance;

    public Transform[] spawnPoints;

    private bool hasSpawned = false;


    private void Awake()
    {
        instance = this;
        print("reset_awake");
    }


    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameManager.instance.ResetDeadStatus();
        }

        //PhotonNetwork.LocalPlayer.TagObject = null;
        SpawnPlayer();
    }

    private void Update()
    {
        //print("test_Up" + this.gameObject.GetInstanceID());
    }



    public void SpawnPlayer()
    {
        print("Work");

        if (hasSpawned) return;

        hasSpawned = true;

        int index = (int)photonView.Owner.CustomProperties["slot"];

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
        Hashtable props = new Hashtable();
        props["dead"] = true;

        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        photonView.RPC("CheckLastPlayerRPC", RpcTarget.MasterClient);
    }

    [PunRPC]
    void CheckLastPlayerRPC()
    {
        print("host check");
        GameManager.instance.StartCheckLastPlayer();
    }






    public string GetPlayerName(int id) 
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.ActorNumber == id)
            {
                return player.NickName;
            }
        }
        return "Non";
    }



}
