using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PlayerData playerData;
    public string filePath;
    public GlobalLeaderBoard globalLeaderBoard;
    public GamesWonLeaderboard gamesWonLeaderboard;
    public WeeklyKillsLeaderboard weeklyKillsLeaderboard;
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }


    void Start()
    {
        LoadPlayerData();
        LoginToPlayFab();
    }

    private void LoginToPlayFab()
    {
        var request = new LoginWithCustomIDRequest()
        {
            CustomId = playerData.id,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request, PlayFabLoginResult, PlayFabLoginError);
    }

    private void PlayFabLoginResult(LoginResult loginResult)
    {
        Debug.Log("PlayFab - Login: Successful" + loginResult.ToJson());
    }

    private void PlayFabLoginError(PlayFabError playFabError)
    {
        Debug.Log("PlayFab - Login: Failed" + playFabError.ErrorMessage);

    }

    public void LoadPlayerData()
    {

        try
        {
            if (!File.Exists(filePath))
            {
                playerData = new PlayerData();
                SavePlayerData();
            }
            var fileContents = File.ReadAllBytes(filePath);
            var decryptedData = AESEcryption.Decrypt(fileContents);
            playerData = JsonConvert.DeserializeObject<PlayerData>(decryptedData);

        }
        catch (System.Exception e)
        {
            Debug.Log("An Error occured while loading data from a file: " + e.Message);
        }

    }

    public void SavePlayerData()
    {
        try
        {
            var serializedData = JsonConvert.SerializeObject(playerData);
            var encryptedData = AESEcryption.Encrypt(serializedData);
            File.WriteAllBytes(filePath, encryptedData);
        }
        catch (System.Exception e)
        {

            Debug.Log("An error coccured while saving data to a file: " + e.Message);
        }

    }
}
