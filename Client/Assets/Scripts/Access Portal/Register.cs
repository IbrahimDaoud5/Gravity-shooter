using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class Register : MonoBehaviour
{
    public TMP_InputField usernameInputField, passwordInputField, confirmPasswordInputField;
    public Text label;
    public Button createButton;

    void Start()
    {
        usernameInputField.onValueChanged.AddListener(delegate { HideErrorMessage(); });
        passwordInputField.onValueChanged.AddListener(delegate { HideErrorMessage(); });
        confirmPasswordInputField.onValueChanged.AddListener(delegate { HideErrorMessage(); });
    }
    void Update()
    {

    }

    public void CallRegister()
    {
        string username = usernameInputField.text;
        string password = passwordInputField.text;
        string confirmPassword = confirmPasswordInputField.text;
        label.text = "";
        label.color = Color.red;


        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
        {
            Debug.Log("Required INFO MISSING !");
            label.text = "Please fill in the required INFO";//SHOW IN LABEL
        }
        else if (password != confirmPassword)
        {
            string msg = "Passwords do NOT match!";
            Debug.Log(msg);
            label.text = msg;//SHOW IN LABEL

        }
        else StartCoroutine(RegisterCoroutine(username, password));
    }

    IEnumerator RegisterCoroutine(string username, string password)
    {
        LoginData loginData = new LoginData (username, password);
        string jsonData = JsonUtility.ToJson(loginData);

        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);

        using (UnityWebRequest www = new UnityWebRequest(GlobalConfig.ServerUrl + "/register", "POST"))
        {
            www.certificateHandler = new AcceptAllCertificates(); // TEMPORARY - until we use CA
            www.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success) // If the connection is not successful
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                if (www.downloadHandler.text != "Registration successful")
                    label.text = www.downloadHandler.text;//SHOW IN LABEL

                else  // ---> Registration successful
                {
                    string s = "You've successfully signed up, Please Log in";
                    label.color = Color.green;
                    label.text = s;//SHOW IN LABEL
                }
            }
        }
    }
    public void HideErrorMessage()
    {
        label.text = "";
    }
}