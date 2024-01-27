using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ServerRequestHandler : MonoBehaviour
{
    // Singleton instance
    public static ServerRequestHandler Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SendRequest(string endpoint, string jsonData, Action<string> onCompleted)
    {
        StartCoroutine(PerformRequest(endpoint, jsonData, onCompleted));
    }

    private IEnumerator PerformRequest(string endpoint, string jsonData, Action<string> onCompleted)
    {
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);

        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(GlobalConfig.ServerUrl + endpoint, "POST"))
        {
            www.certificateHandler = new AcceptAllCertificates(); // TEMPORARY - until we use CA
            www.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + www.error);
                onCompleted?.Invoke(null);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                onCompleted?.Invoke(www.downloadHandler.text);
            }
        }
    }
}
