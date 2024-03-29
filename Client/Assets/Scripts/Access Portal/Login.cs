using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    public TMP_InputField usernameInputField, passwordInputField;
    public Text label;
    public UIManager UImanager;
    //public Button b;

    void Start()
    {
        usernameInputField.onValueChanged.AddListener(delegate { HideErrorMessage(); });
        passwordInputField.onValueChanged.AddListener(delegate { HideErrorMessage(); });
    }
    void Update() { }

    public void CallLogin()
    {
        string username = usernameInputField.text; // Access text from InputField
        string password = passwordInputField.text; // Access text from InputField
        LoginData loginData = new LoginData(username, password);
        string jsonData = JsonUtility.ToJson(loginData);
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            label.text = "Please fill in the required INFO";//SHOW IN LABEL
        else ServerRequestHandler.Instance.SendRequest("/login", jsonData, HandleResponse);
    }
    private void HandleResponse(string responseText)
    {
        if (responseText == null)
        {
            // Handle error
            return;
        }
        else if (responseText == "Login successful")
        {

            // Save the username in PlayerPrefs
            PlayerPrefs.SetString("Username", usernameInputField.text);
            PlayerPrefs.Save();
            UImanager.ShowLobbyPanel();
        }
        else
            label.text = responseText;//SHOW IN LABEL

        // Process the response
        // Additional response handling logic...
    }


    /*
    IEnumerator LoginCoroutine(string username, string password)
    {

        LoginData loginData = new LoginData (username,password );
        string jsonData = JsonUtility.ToJson(loginData);
        //Debug.Log(jsonData);

        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);

        using (UnityWebRequest www = new UnityWebRequest(GlobalConfig.ServerUrl + "/login", "POST"))
        {
            www.certificateHandler = new AcceptAllCertificates(); // TEMPORARY - until we use CA
            www.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            // Handle different cases of request result
            switch (www.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("Error: " + www.error);
                    // Handle connection and data processing errors (e.g., display a message to the user)
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("HTTP Error: " + www.error);
                    // Handle HTTP protocol errors (e.g., HTTP response code indicates an error)
                    break;
            }

            if (www.result != UnityWebRequest.Result.Success) // If the connection is not successful
            {

                //label.enabled = true;
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);

                if (www.downloadHandler.text == "Login successful")
                {
                    // Handle successful login
                    UImanager.ShowLobbyPanel();
                    // ------------> GO TO THE LOBBY 
                }
                else
                    label.text = www.downloadHandler.text;//SHOW IN LABEL

            }
        }
    }
    */

    public void HideErrorMessage()
    {
        label.text = "";
    }

}
