using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiplayerLobby : MonoBehaviourPunCallbacks
{
    public Transform panels;

    [Header("Login Panel")]
    public InputField playerName;


    [Header("Create Room Panel")]
    public InputField roomName;

    [Header("Inside Room")]
    public GameObject roomNameText;
    public GameObject textPrefab;
    public Transform InsideRoomContent;
    public GameObject StartGameButton;

    [Header("List Room Panel")]
    public GameObject RoomEntryPrefab;
    public Transform ListRoomsContent;

    Dictionary<string, RoomInfo> cachedRoomList;

    [Header("Chat")]
    public Chat chat;

    private void Awake()
    {
        cachedRoomList = new Dictionary<string, RoomInfo>();
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    private void Start()
    {
        playerName.text = "Player" + Random.Range(1, 10000);
    }

    #region AUXILLIARY FUNCTIONS
    public void ActivatePanel(string panelName)
    {
        DeactivatePanels();


        foreach (Transform panel in panels)
        {
            if(panel.name == panelName)
            {
                panel.gameObject.SetActive(panel);
            }
        }
    }

    private void DeactivatePanels()
    {
        foreach (Transform panel in panels)
        {
            panel.gameObject.SetActive(false);
        }
    }

    private void DeleteChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    private void UpdateLeaderboardUsername()
    {
        var request = new UpdateUserTitleDisplayNameRequest()
        {
            DisplayName = playerName.text
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request,
            result => { Debug.Log("Sucess!!"); },
            error => { Debug.Log("Error While Updating Username" + error.ErrorMessage); });
    }

    #endregion

    #region BUTTON CLICKS

    public void BackButtonClicked()
    {
        SceneManager.LoadScene("Main");
    }

    public void LoginButtonClicked()

    {
        PhotonNetwork.LocalPlayer.NickName = playerName.text;
        PhotonNetwork.ConnectUsingSettings();
        UpdateLeaderboardUsername();
    }



    public void JoinRandomRoomClicked()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void CreateRoomClicked()
    {

        if(string.IsNullOrEmpty(roomName.text))
        {
            Debug.Log("Room needs a name");
        }
        else
        {
            var roomOption = new RoomOptions();
            roomOption.MaxPlayers = 4;
            PhotonNetwork.CreateRoom(roomName.text,roomOption);
        }
    }
    public void StartGameClicked()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.LoadLevel("MultiplayerScene");
        }
        else
        {
            Debug.Log("Must be more than 1 player");
        }
    }

    public void LeaveRoomClicked()
    {
        PhotonNetwork.LeaveRoom();
        DeleteChildren(InsideRoomContent);
        ActivatePanel("Selection");
    }

    public void ListRoomsClicked()
    {if(!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
            ActivatePanel("ListRoom");
        }
        else
        {
            Debug.Log("You are In a lobby");
        }

    }

    public void LeaveLobbyClicked()
    {
        if(PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
            ActivatePanel("Selection");
        }
        else
        {
            Debug.Log("Must Be In Lobby");
        }
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
        ActivatePanel("Login");
    }
    #endregion

    #region PUN CALLBACKS
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected To Master");

        ActivatePanel("Selection");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected");
    }

    public override void OnCreatedRoom()
    {
        roomNameText.GetComponent<Text>().text = roomName.text;
        Debug.Log("Room Created");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Room Joined");

        chat.username = PhotonNetwork.LocalPlayer.NickName;

        chat.ChatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "", new Photon.Chat.AuthenticationValues(chat.username));

        ActivatePanel("InsideRoom");
        StartGameButton.SetActive(PhotonNetwork.IsMasterClient);
        roomNameText.GetComponent<Text>().text = PhotonNetwork.CurrentRoom.Name;

        foreach (var player in PhotonNetwork.PlayerList)
        {
            var newPlayerRoomEntry = Instantiate(textPrefab, InsideRoomContent);
            newPlayerRoomEntry.name = player.NickName;
            newPlayerRoomEntry.GetComponent<Text>().text = player.NickName;
        }

    }

    public override void OnLeftRoom()
    {
        Debug.Log("Room Left!");
        chat.ChatClient.Disconnect();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room Creation Failed");
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Lobby Joined");
    }

    public override void OnLeftLobby()
    {
        Debug.Log("Lobby Left");
        cachedRoomList.Clear();
        DeleteChildren(ListRoomsContent);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("OnRoomListUpdate: " + roomList.Count);

        DeleteChildren(ListRoomsContent);

        UpdateCacheRoomList(roomList);

        foreach (var room in cachedRoomList)
        {
            var newRoomEntry = Instantiate(RoomEntryPrefab, ListRoomsContent);
            var roomEntryScript = newRoomEntry.GetComponent<RoomEntry>();
            roomEntryScript.roomName = room.Value.Name;
            roomEntryScript.roomText.text = $"[{room.Value.Name}] - ({room.Value.PlayerCount} / {room.Value.MaxPlayers})";
        }
    }


    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        var newPlayerRoomEntry = Instantiate(textPrefab, InsideRoomContent);
        newPlayerRoomEntry.name = newPlayer.NickName;
        newPlayerRoomEntry.GetComponent<Text>().text = newPlayer.NickName;
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        foreach (Transform child in InsideRoomContent)
        {
            if(child.name == otherPlayer.NickName)
            {
                Destroy(child.gameObject);
            }    
        }
    }

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        StartGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    private void UpdateCacheRoomList(List<RoomInfo> roomList)
    {
        foreach (var room in roomList)
        {
            //remove from cache

            if(!room.IsOpen|| !room.IsVisible || room.RemovedFromList)
            {
                if(cachedRoomList.ContainsKey(room.Name))
                {
                    cachedRoomList.Remove(room.Name);
                }
                continue;
            }

            //Create new
            cachedRoomList[room.Name] = room;
            
        }
    }
    #endregion
}
