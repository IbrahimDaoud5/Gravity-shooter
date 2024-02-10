using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // This method can be called to load a new scene
    public void LoadScene()
    {
        SceneManager.LoadScene("FirstLevelScene");
    }
}
