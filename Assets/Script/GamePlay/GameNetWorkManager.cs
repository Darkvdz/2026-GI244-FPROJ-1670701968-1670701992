using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameNetWorkManager : MonoBehaviourPunCallbacks
{

    public static GameNetWorkManager instance;

    private void Awake()
    {
        instance = this;
    }


    public void CheckGameEnd(int idPlayer)
    {
        int score = GameManager.instance.PlayerScore[idPlayer - 1];
        string Winner = PlayerManager.instance.GetPlayerName(idPlayer);

        if (score >= 3)
        {
            photonView.RPC("EndGame", RpcTarget.All, Winner);
        }
        else
        {
            photonView.RPC("StartNextRound", RpcTarget.All);
        }
    }

    [PunRPC]
    private void StartNextRound()
    {
        Debug.Log("Next Round");

        if (PhotonNetwork.IsMasterClient)
        {
            GameManager.instance.ResetDeadStatus();
        }

        PhotonNetwork.LoadLevel(GameManager.instance.GetRandomScene());
    }
}
