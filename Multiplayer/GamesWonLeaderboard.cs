using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamesWonLeaderboard : MonoBehaviour
{
    public int MaxNumberOfResults = 10;
    public GamesWonPopUp LeaderboardPopUp;

    public void SubmitScore(int gamesWon)
    {
        var request = new UpdatePlayerStatisticsRequest()
        {
            Statistics = new List<StatisticUpdate>()
            {
                new StatisticUpdate()
                {
                    StatisticName = "Games Won",
                    Value = gamesWon
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, 
            result => { Debug.Log("Successfully Updated"); },
            error => { Debug.Log("Update Failed: " + error.ErrorMessage); });
    }

    public void RequestLeaderboard()
    {
        var request = new GetLeaderboardRequest()
        {
            MaxResultsCount = MaxNumberOfResults,
            StatisticName = "Games Won"
        };
        PlayFabClientAPI.GetLeaderboard(request,
            result => { 
                Debug.Log("Leaderboard successfully fetched");
                LeaderboardPopUp.UpdateUI(result.Leaderboard);
            },
            error => { Debug.Log("Error while fetching leaderboard" + error.ErrorMessage); });
    }

}
