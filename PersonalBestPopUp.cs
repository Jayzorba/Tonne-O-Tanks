using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonalBestPopUp : MonoBehaviour
{
    public GameObject ScoreHolder;
    public GameObject NoScore;
    public Text Username;
    public Text BestScore;
    public Text Date;
    public Text TotalPlayers;
    public Text RoomName;

    private void OnEnable()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        var playerData = GameManager.Instance.playerData;
        
        if (playerData.isPlayerDataUpdated)
        {
            Username.text = playerData.username;
            BestScore.text = playerData.bestScore.ToString();
            Date.text = playerData.date.ToString();
            TotalPlayers.text = playerData.totalPlayersInGame.ToString();
            RoomName.text = playerData.roomName;


            ScoreHolder.SetActive(true);
            NoScore.SetActive(false);
        }
        else
        {
            ScoreHolder.SetActive(false);
            NoScore.SetActive(true);
        }
    }
}
