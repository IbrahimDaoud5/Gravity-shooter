using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Lobby : MonoBehaviour
{

    public UIManager UImanager;

    // Example data to send with the POST request
    // Modify this class according to what data you need to send
    [System.Serializable]
    public class LobbyData
    {
        public string playerId;
        // Add other fields as needed
    }

    void Start()
    {
        // Initialization if needed
    }

    void Update()
    {
        // Update logic if needed
    }

    public void CallLobby(string playerId)
    {
        LobbyData lobbyData = new LobbyData { playerId = "ds" };
        string jsonData = JsonUtility.ToJson(lobbyData);

        StartCoroutine(LobbyCoroutine(jsonData));
    }

    IEnumerator LobbyCoroutine(string jsonData)
    {
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);

        using (UnityWebRequest www = new UnityWebRequest(GlobalConfig.ServerUrl + "/lobby", "POST"))
        {
            www.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            switch (www.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("Error: " + www.error);
                    // Handle errors
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("HTTP Error: " + www.error);
                    // Handle HTTP errors
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log("Response: " + www.downloadHandler.text);
                    // Process the successful response
                    // For example, update the UI based on lobby data
                    break;
            }
        }
    }

    // Additional methods if needed
}
