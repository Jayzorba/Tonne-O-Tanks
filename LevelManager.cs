using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    int enemiesKilled = 0;
    public GameObject winScreen;
    public Text enemiesKilledText;
    private void Awake()
    {
        Enemy.OnEnemyKilled += OnEnemyKilled;
        enemiesKilledText.text = $"{enemiesKilled} / 5";
    }

    private void OnEnemyKilled()
    {
       // Debug.Log("Enemy Died!");
        enemiesKilled++;
        enemiesKilledText.text = $"{enemiesKilled} / 5";
    }

    private void Update()
    {
        if(enemiesKilled == 5)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            winScreen.SetActive(true);
        }
    }

    public void LeaveGameClicked()
    {
        Enemy.OnEnemyKilled -= OnEnemyKilled;
        SceneManager.LoadScene("Main");
    }

    public void RestartGameClicked()
    {
        Enemy.OnEnemyKilled -= OnEnemyKilled;
        SceneManager.LoadScene("SinglePlayer");
    }
}
