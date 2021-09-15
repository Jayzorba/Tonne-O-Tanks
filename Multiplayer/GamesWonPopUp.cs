using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamesWonPopUp : MonoBehaviour
{
    public GameObject ScoreHolder;
    public GameObject NoScore;
    public GameObject LeaderboardItem;

    private void OnEnable()
    {
        GameManager.Instance.gamesWonLeaderboard.RequestLeaderboard();
    }

    public void UpdateUI(List<PlayerLeaderboardEntry> entries)
    {
        if(entries.Count >0)
        {
            DeleteChildren(ScoreHolder.transform);

            for (int i = 0; i < entries.Count; i++)
            {
                var newItem = Instantiate(LeaderboardItem, Vector3.zero, Quaternion.identity, ScoreHolder.transform);
                newItem.GetComponent<LeaderboardItem>().SetValues(i + 1, entries[i].DisplayName, entries[i].StatValue);
            }

            ScoreHolder.SetActive(true);
            NoScore.SetActive(false);
        }
        else
        {
            ScoreHolder.SetActive(false);
            NoScore.SetActive(true);
        }
    }

    private void DeleteChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }
}
