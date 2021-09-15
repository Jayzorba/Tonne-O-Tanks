using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public string SceneName;
    public void LoadScene()
    {
        SceneManager.LoadScene(SceneName);
    }
}
