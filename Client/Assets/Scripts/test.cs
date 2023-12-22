using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class test : MonoBehaviour
{
    public TMP_InputField usernameInputField, passwordInputField;
    public Button b;


    public void CallLogin()
    {
        string username = usernameInputField.text; // Access text from InputField
        string password = passwordInputField.text; // Access text from InputField
        StartCoroutine(LoginCoroutine(username, password));
    }

    IEnumerator LoginCoroutine(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        string loginUrl = GlobalConfig.ServerUrl + "/login";

        using (UnityWebRequest www = UnityWebRequest.Post(loginUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Login success: " + www.downloadHandler.text);
                // Handle successful login
            }
        }
    }
}
