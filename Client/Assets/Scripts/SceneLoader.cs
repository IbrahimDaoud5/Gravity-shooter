using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // This method can be called to load a new scene
    public void LoadScene()
    {
        //add the player to be in game as its status . send to server 
        SceneManager.LoadScene("FirstLevelScene");
    }
}
