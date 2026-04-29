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

    private void Awake()
    {
        instance = this;
    }
    public void SpawnPlayer()
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

        foreach (var players in PhotonNetwork.PlayerList)
        {
            Debug.Log(players.NickName + " ID: " + players.ActorNumber);
        }

    }

    public void PlayerDied()
    {
        print("die");

        photonView.RPC("CheckLastPlayerRPC", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    [PunRPC]
    void CheckLastPlayerRPC(int idPlayer)
    {
        print("host check");
        GameManager.instance.CheckLastPlayer(idPlayer);
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
