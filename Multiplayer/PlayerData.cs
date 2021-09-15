using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public string id;
    public string username;
    public int gamesWon;
    public int bestScore;
    public int totalKills;
    public DateTime date;
    public int totalPlayersInGame;
    public string roomName;
    public Material tankColour;
    public bool isPlayerDataUpdated;

    public PlayerData()
    {
        id = Guid.NewGuid().ToString();
    }
}
