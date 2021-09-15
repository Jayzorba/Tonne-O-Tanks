using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviour, IChatClientListener
{
    [HideInInspector]
    public ChatClient ChatClient;
    public string username;

    public InputField inputField;
    public Text ChannelMessagesText;
    public Text PrivateMessagesText;

    public GameObject ChatSelect;
    public List<Button> ChatSelectList;
    public Transform ChatPanel;

    public GameObject PrivateChatPanel;
    public GameObject PublicChatPanel;

    public bool isInPrivateChat;
    public string targetChatPlayer;

    // Start is called before the first frame update
    void Start()
    {
        ChatClient = new ChatClient(this);
        ChatSelectList = new List<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if(ChatClient != null)
        {
            ChatClient.Service();
        }

    }
    #region Auxilliary Methods
    public void SendMessage()
    {
        if(inputField.text == "")
        {
            return;
        }

        if(isInPrivateChat)
        {
            ChatClient.SendPrivateMessage(targetChatPlayer, inputField.text);
            inputField.text = "";
        }
        else
        {
            ChatClient.PublishMessage(PhotonNetwork.CurrentRoom.Name, inputField.text);
            inputField.text = "";
        }

    }

    public void OnChatButtonClicked()
    {

        var roomSelect = Instantiate(ChatSelect, ChatPanel);
        var roomSelectText = roomSelect.GetComponent<Text>();
        roomSelectText.text = PhotonNetwork.CurrentRoom.Name;
        ChatSelectList.Add(roomSelect.GetComponentInChildren<Button>());
        ChatSelectList[0].onClick.AddListener(() =>
        {
            PublicChatPanel.SetActive(true);
            PrivateChatPanel.SetActive(false);
            isInPrivateChat = false;
        });

        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            if (PhotonNetwork.PlayerList[i].NickName != PhotonNetwork.LocalPlayer.NickName)
            {
                var chatSelect = Instantiate(ChatSelect, ChatPanel);
                var chatSelectText = chatSelect.GetComponent<Text>();
                chatSelectText.text = PhotonNetwork.PlayerList[i].NickName;

                ChatSelectList.Add(chatSelect.GetComponentInChildren<Button>());

                var chatSelectIndex = ChatSelectList.IndexOf(chatSelect.GetComponentInChildren<Button>());

                ChatSelectList[chatSelectIndex].onClick.AddListener(() =>
                {
                    PublicChatPanel.SetActive(false);
                    PrivateChatPanel.SetActive(true);
                    isInPrivateChat = true;
                    targetChatPlayer = chatSelectText.text;
                    ChatClient.SendPrivateMessage(targetChatPlayer, "Joined");
                    Debug.Log(targetChatPlayer);
                });
            }

        }

    }


    public void OnLeaveChatClicked()
    {
        DeleteChildren(ChatPanel);
        ChatSelectList.Clear();
    }

    private void DeleteChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }


    #endregion

    #region Callbacks
    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log($"Chat: [{level}] - {message}");
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log($"Chat: OnChatStateChange: {state}");
    }

    public void OnConnected()
    {
        Debug.Log($"Chat: User {username} connected to Chat!");
        ChatClient.Subscribe(PhotonNetwork.CurrentRoom.Name, creationOptions: new ChannelCreationOptions() { PublishSubscribers = true });

    }

    public void OnDisconnected()
    {
        Debug.Log($"Chat: User {username} disconnected from Chat!");
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        ChatChannel channel = null;

        if(ChatClient.TryGetChannel(channelName, out channel))
        {
            ChannelMessagesText.text = channel.ToStringMessages();
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        ChatChannel channel = this.ChatClient.PrivateChannels[channelName];

        if (ChatClient.TryGetChannel(channelName, out channel))
        {
            PrivateMessagesText.text = "(PRIVATE: )"+channel.ToStringMessages();
        }
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        for (int i = 0; i < channels.Length; i++)
        {
            if(results[i])
            {
                Debug.Log($"Chat: Subcribed to {channels[i]} channel!");
                ChatClient.PublishMessage(PhotonNetwork.CurrentRoom.Name,  "says Hi");
            }
        }
    }

    public void OnUnsubscribed(string[] channels)
    {
        
    }

    public void OnUserSubscribed(string channel, string user)
    {

    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        
    }
    #endregion
}
