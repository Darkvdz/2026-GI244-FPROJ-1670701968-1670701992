using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
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


    public void CheckGameEnd(Player player)
    {
        print("player " + player);

        int score = player.CustomProperties.ContainsKey("score")
            ? (int)player.CustomProperties["score"]
            : 0;

        print("Score " + score);

        if (PhotonNetwork.PlayerList.Length <= 1)
        {
            photonView.RPC("EndGame", RpcTarget.All, player.NickName);
            return;
        }

        if (score >= 3)
        {
            photonView.RPC("EndGame", RpcTarget.All, player.NickName);
        }
        else
        {
            photonView.RPC("StartNextRoundCall", RpcTarget.All);
        }


    }


    [PunRPC]
    private void StartNextRoundCall()
    {
        StartCoroutine(StartNextRound());
    }

    IEnumerator StartNextRound() 
    {
        Debug.Log("Next Round");

        if (PhotonNetwork.IsMasterClient)
        {
            GameManager.instance.ResetDeadStatus();
            yield return new WaitForSeconds(2f);
            PhotonNetwork.LoadLevel(GameManager.instance.GetRandomScene());
        }

    }


    [PunRPC]
    void EndGame(string winnerName)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameManager.instance.EndGame(winnerName);
        }

    }
}
