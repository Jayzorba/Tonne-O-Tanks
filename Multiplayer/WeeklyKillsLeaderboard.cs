using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeeklyKillsLeaderboard : MonoBehaviour
{
    public int MaxNumberOfResults = 15;
    public WeeklyKillsPopUp LeaderboardPopUp;

    public void SubmitScore(int playerKills)
    {
        var request = new UpdatePlayerStatisticsRequest()
        {
            Statistics = new List<StatisticUpdate>()
            {
                new StatisticUpdate()
                {
                    StatisticName = "Total Kills",
                    Value = playerKills
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
            StatisticName = "Total Kills"
        };
        PlayFabClientAPI.GetLeaderboard(request,
            result => { 
                Debug.Log("Leaderboard successfully fetched");
                LeaderboardPopUp.UpdateUI(result.Leaderboard);
            },
            error => { Debug.Log("Error while fetching leaderboard" + error.ErrorMessage); });
    }

}
