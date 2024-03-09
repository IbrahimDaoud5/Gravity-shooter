using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu1 : MonoBehaviour
{
    public static bool gameIsPaused = false;
    public GameObject pauseMenuUI;
    public UIManager UImanager;
    public string destinationSceneName;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;

    }
    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
    }
    public void LoadMenu()
    {
        //MUST SEND TO SERVER A REQUEST TO SET STATUS "ISINGAME" = FALSE 
        Time.timeScale = 1f;
        destinationSceneName = "MainScene";
        LoadSceneAndShowPanel("Lobby");
        Resume();//change the flag so when entering again the shooting is not paused 
        TargetHit.SetTargets(0);

    }
    public void QuitGame()
    {
        //take out from logged in list 
        LoginData lobbyData = new LoginData(PlayerPrefs.GetString("Username", "Guest"), "");
        string jsonData = JsonUtility.ToJson(lobbyData);
        ServerRequestHandler.Instance.SendRequest("/lobby/logout", jsonData, HandleResponse);
    }

    private void HandleResponse(string responseText)
    {
        if (responseText == null)
        {
            // Handle error
            return;
        }
        else if (responseText == "logged out successfully")
        {
            Application.Quit();
        }
    }
    public void LoadSceneAndShowPanel(string panelName)
    {
        // Pass the panel name as a parameter to the new scene
        PlayerPrefs.SetString("PanelToShow", panelName);
        SceneManager.LoadScene(destinationSceneName);
    }
}
