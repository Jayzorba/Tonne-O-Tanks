using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerInGameScore : MonoBehaviourPunCallbacks
{
    public GameObject playerScorePrefab;
    public Transform Panel;

    Dictionary<int, GameObject> playerListEntries;

    // Start is called before the first frame update
    void Start()
    {
        playerListEntries = new Dictionary<int, GameObject>();
        
        foreach (var player in PhotonNetwork.PlayerList)
        {
            var playerScoreObject = Instantiate(playerScorePrefab, Panel);
            var textScore = playerScoreObject.GetComponent<Text>();
            player.SetScore(0);
            textScore.text = $"{player.NickName}\nScore: {player.GetScore()}";

            playerListEntries[player.ActorNumber] = playerScoreObject;
        }

        Debug.Log(playerListEntries.Count);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
        playerListEntries.Remove(otherPlayer.ActorNumber);


    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if(playerListEntries.ContainsKey(targetPlayer.ActorNumber))
        {
            var playerScoreObject = playerListEntries[targetPlayer.ActorNumber];
            var textScore = playerScoreObject.GetComponent<Text>();
            textScore.text = $"{targetPlayer.NickName}\nScore: {targetPlayer.GetScore()}";
        }
    }
}
