using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    public Transform[] spawnPoints;

    void Start()
    {
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        int index = (PhotonNetwork.LocalPlayer.ActorNumber - 1) % spawnPoints.Length;

        Transform spawnPoint = spawnPoints[index];

        PhotonNetwork.Instantiate("Player/Player2D", spawnPoint.position, spawnPoint.rotation);

    }
}