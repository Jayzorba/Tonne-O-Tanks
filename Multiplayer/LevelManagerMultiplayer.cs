using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManagerMultiplayer : MonoBehaviourPunCallbacks
{
    public int maxKills = 3;
    public GameObject GameOverPopUp;
    public Text winnerText;
    public Text timerText;
    PhotonView photonView;
    public float timeLimit = 0;
    public float counter = 90;
    string winner;
    int highestScore = 0;

    // Start is called before the first frame update
    void Start()
    {

        PhotonNetwork.Instantiate("PlayerMultiplayer", new Vector3(UnityEngine.Random.Range(3, 20), 0, UnityEngine.Random.Range(-32, -15)), Quaternion.identity);

        photonView = GetComponent<PhotonView>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {

        if(counter <= timeLimit)
        {

            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (player.GetScore() > highestScore)
                {
                    winner = player.NickName;
                }
                else if(highestScore == 0)
                {
                    winnerText.text = "No one";
                }
                

            }
            winnerText.text = winner;
            GameOverPopUp.SetActive(true);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            counter -= Time.deltaTime;
            timerText.text = counter.ToString("F0");
        }
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if(targetPlayer.GetScore() == maxKills)
        {
            GameOver(targetPlayer);
        }
    }

    private void GameOver(Photon.Realtime.Player winner)
    {
        winnerText.text = winner.NickName;
        GameOverPopUp.SetActive(true);

        StorePersonalBest();
        StoreGamesWon(winner);
        StoreWeeklyKills();
        

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void StorePersonalBest()
    {
        var username = PhotonNetwork.LocalPlayer.NickName;
        var score = PhotonNetwork.LocalPlayer.GetScore();
        var totalPlayersInTheGame = PhotonNetwork.CurrentRoom.PlayerCount;
        var roomName = PhotonNetwork.CurrentRoom.Name;
        var playerData = GameManager.Instance.playerData;
        

        if(score >= playerData.bestScore)
        {
            playerData.username = username;
            playerData.bestScore = score;
            playerData.date = DateTime.UtcNow;
            playerData.totalPlayersInGame = totalPlayersInTheGame;
            playerData.roomName = roomName;
            playerData.isPlayerDataUpdated = true;

            GameManager.Instance.globalLeaderBoard.SubmitScore(score);
            GameManager.Instance.SavePlayerData();
        }

       // var totalKills = playerData.totalKills + score;
        
    }

    private void StoreGamesWon(Photon.Realtime.Player winner)
    {
        var playerData = GameManager.Instance.playerData;

        if(winner.NickName == PhotonNetwork.LocalPlayer.NickName)
        {
            var gamesWon = GameManager.Instance.playerData.gamesWon++;
            GameManager.Instance.gamesWonLeaderboard.SubmitScore(gamesWon);
            GameManager.Instance.SavePlayerData();
        }
    }

    private void StoreWeeklyKills()
    {
        var kills = PhotonNetwork.LocalPlayer.GetScore();

        GameManager.Instance.weeklyKillsLeaderboard.SubmitScore(kills);
    }

    public void LeaveGameClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void PlayAgainClicked()
    {
        if(PhotonNetwork.PlayerList.Length >1)
        {
            photonView.RPC("RestartMatch", RpcTarget.AllViaServer);
        }
        else
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene("Lobby");
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            winnerText.text = PhotonNetwork.LocalPlayer.NickName;
            GameOverPopUp.SetActive(true);


            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    [PunRPC]
    void RestartMatch()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LoadLevel("MultiplayerScene");
    }
}
