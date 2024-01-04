using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    public TMP_InputField usernameInputField, passwordInputField;
    public Button b;

    void Start() { }
    void Update() { }

    public void CallLogin()
    {
        string username = usernameInputField.text; // Access text from InputField
        string password = passwordInputField.text; // Access text from InputField
        StartCoroutine(LoginCoroutine(username, password));
    }

    IEnumerator LoginCoroutine(string username, string password)
    {

        LoginData loginData = new LoginData { username = username, password = password };
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

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                // Handle successful login
                // Maybe insert to LoggedIn List
                // ------------> GO TO THE GAME
            }
        }
    }

}
