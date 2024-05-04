using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

public class Lobby1 : MonoBehaviour
{
    public UIManager UImanager;
    public Text welcomeLabel;
    public Text errorLabel;
    public Transform lobbyPanelTransform;
    public SceneLoader sceneLoader;

    void Update()
    {

    }
    async void OnEnable()
    {
        string s = "Welcome " + PlayerPrefs.GetString("Username", "Guest");
        welcomeLabel.color = Color.green;
        welcomeLabel.text = s;//SHOW IN LABEL 

        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();


    }



    public void PlaySolo()
    {
        sceneLoader.LoadScene();
    }

    public void CallLogout(string playerId)
    {

        LoginData lobbyData = new LoginData(PlayerPrefs.GetString("Username", "Guest"), "");
        string jsonData = JsonUtility.ToJson(lobbyData);
        ServerRequestHandler.Instance.SendRequest("/lobby/logout", jsonData, HandleResponse);
        var username = PlayerPrefs.GetString("Username", "Guest");
        AuthenticationService.Instance.SignOut();
        Debug.Log("Signed out!");
        PlayerPrefs.DeleteKey("Username");
        //PlayerPrefs.DeleteKey("UserToken");
        // PlayerPrefs.DeleteKey(username + "_LobbyCreated");
        //PlayerPrefs.Save();

    }

    private void HandleResponse(string responseText)
    { // for ready and for logout
        if (responseText == null)
        {
            Debug.LogError("responseText is Null");
            return;
        }
        else if (responseText == "logged out successfully")
        {
            UImanager.ShowLoginPanel();
        }

    }

    /*
        public static implicit operator Lobby1(Unity.Services.Lobbies.Models.Lobby v)
        {
            throw new NotImplementedException();
        }*/
}
