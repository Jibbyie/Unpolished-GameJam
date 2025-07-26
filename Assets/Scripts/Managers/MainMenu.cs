using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Prototype");
    }

    public void QuitGame()
    {
        // This preprocessor directive checks if we are running in the Unity Editor.
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;

        #elif UNITY_WEBGL
                     Application.Quit();
        #else
                    Application.Quit();
        #endif
    }
}