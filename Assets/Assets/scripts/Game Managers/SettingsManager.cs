using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public bool settingsToggle = false;
    public GameObject settingsScreen;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            settingsToggle = !settingsToggle;
        }

        //if settings is on
        if (settingsToggle)
        {
            settingsScreen.SetActive(true);
        }
        else
        {
            settingsScreen.SetActive(false);
        }
    }
    public void MenuButton()
    {
        settingsToggle = !settingsToggle;
    }

    public void SettingsButton()
    {
        //settings
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
