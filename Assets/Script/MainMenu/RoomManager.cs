using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI[] slotsName;

    //UI
    public TMP_InputField nameInput;
    public TMP_InputField roomInput;
    public Button joinButton;
    public Button createButton;
    public Button startButton;

    public GameObject MainMenu;
    public GameObject RoomMenu;

    private void Awake()
    {
        joinButton.onClick.AddListener(() => JoinRoom());
        createButton.onClick.AddListener(() => CreateRoom());
        startButton.onClick.AddListener(() =>
        {
            if (!PhotonNetwork.IsMasterClient) return;

            Debug.Log("Game Starting...");

            //PhotonNetwork.LoadLevel("GameScene");
        });
    }

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon");
    }

    public void SetPlayerName()
    {
        if (string.IsNullOrEmpty(nameInput.text))
        {
            Debug.Log("no name");
            PhotonNetwork.NickName = "No_Name";
            return;
        }

        PhotonNetwork.NickName = nameInput.text;
    }

    public void CreateRoom()
    {
        string roomName = roomInput.text;

        if (string.IsNullOrEmpty(roomName))
            roomName = "Room" + Random.Range(0, 1000);

        PhotonNetwork.CreateRoom(roomName);

    }

    public override void OnCreatedRoom()
    {
        SetPlayerName();
        startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);

        MainMenu.SetActive(false);
        RoomMenu.SetActive(true);
    }



    public void JoinRoom()
    {
        joinButton.interactable = false;
        SetPlayerName();
        PhotonNetwork.JoinRoom(roomInput.text);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        joinButton.interactable = true;

        Debug.Log("Room not found");
    }

    public override void OnJoinedRoom()
    {
        //SetPlayerName();

        MainMenu.SetActive(false);
        RoomMenu.SetActive(true);
        startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);

        UpdatePlayerList();
    }



    public void UpdatePlayerList()
    {
        var sortedPlayers = PhotonNetwork.PlayerList
            .OrderByDescending(p => p.IsMasterClient)
            .ThenBy(p => p.ActorNumber)
            .ToList();
        print(sortedPlayers);

        for (int i = 0; i < slotsName.Length; i++)
        {
            if (i < sortedPlayers.Count)
            {
                slotsName[i].gameObject.SetActive(true);

                var player = sortedPlayers[i];

                string name = player.NickName;

                if (player.IsMasterClient)
                    name += " (Host)";

                slotsName[i].text = name;
            }
            else
            {

                slotsName[i].gameObject.SetActive(i < sortedPlayers.Count); ;
            }
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer) => UpdatePlayerList();
    public override void OnPlayerLeftRoom(Player otherPlayer) => UpdatePlayerList();
    public override void OnMasterClientSwitched(Player newMasterClient) => UpdatePlayerList();

}

