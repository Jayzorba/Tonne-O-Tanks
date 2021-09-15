using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TankColourChange : MonoBehaviour
{
    public Color materialColour;
    public Material tankMaterial;

    private void Start()
    {
        
    }

    public void OnBlueClicked()
    {
        materialColour = new Color(0.1803922f, 0.454902f, 0.5450981f);
        tankMaterial.color = materialColour;
    }

    public void OnRedClicked()
    {
        materialColour = new Color(0.9f, 0.1f, 0.1f);
        tankMaterial.color = materialColour;
    }

    public void OnGreenClicked()
    {
        materialColour = new Color(0.1469724f, 0.6320754f, 0.122241f);
        tankMaterial.color = materialColour;
    }

    public void OnYellowClicked()
    {
        materialColour = new Color(0.891258f, 0.8962264f, 0.07186722f);
        tankMaterial.color = materialColour;
    }


    public void OnSaveClicked()
    {
        GameManager.Instance.playerData.tankColour = tankMaterial;
        //GameManager.Instance.SavePlayerData();
        SceneManager.LoadScene("Main");
    }
}
