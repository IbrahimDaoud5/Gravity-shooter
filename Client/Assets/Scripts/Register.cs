using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Register : MonoBehaviour
{
    public TMP_InputField usernameInputField, passwordInputField, confirmPasswordInputField;
    public Button createButton;

    void Start() { }
    void Update() { }

    public void CallRegister()
    {
        string username = usernameInputField.text;
        string password = passwordInputField.text;
        string confirmPassword = confirmPasswordInputField.text;

        if (password == confirmPassword)
        {
            StartCoroutine(RegisterCoroutine(username, password));
        }
        else
        {
            Debug.Log("Password and confirm password do not match.");
            // You might want to show this message to the user in the UI.
        }
    }

    IEnumerator RegisterCoroutine(string username, string password)
    {
        LoginData loginData = new LoginData { username = username, password = password };
        string jsonData = JsonUtility.ToJson(loginData);

        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);

        using (UnityWebRequest www = new UnityWebRequest(GlobalConfig.ServerUrl + "/register", "POST"))
        {
            www.certificateHandler = new AcceptAllCertificates(); // TEMPORARY - until we use CA
            www.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                // Handle successful registration
                // Maybe add a confirmation message or redirect to login
            }
        }
    }
}