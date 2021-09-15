using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalLeaderBoard : MonoBehaviour
{
    public int MaxNumberOfResults = 5;
    public LeaderboardPopUp LeaderboardPopUp;

    public void SubmitScore(int playerScore)
    {
        var request = new UpdatePlayerStatisticsRequest()
        {
            Statistics = new List<StatisticUpdate>()
            {
                new StatisticUpdate()
                {
                    StatisticName = "Most Kills",
                    Value = playerScore
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
            StatisticName = "Most Kills"
        };
        PlayFabClientAPI.GetLeaderboard(request,
            result => { 
                Debug.Log("Leaderboard successfully fetched");
                LeaderboardPopUp.UpdateUI(result.Leaderboard);
            },
            error => { Debug.Log("Error while fetching leaderboard" + error.ErrorMessage); });
    }

}
