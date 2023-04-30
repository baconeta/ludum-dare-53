using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    
    // Switch to the game scene
    public void PlayGame()
    {
        SceneManager.LoadScene("Scenes/GameScene");
    }
    
    // Switch to the main menu scene
    public void NavigateHome()
    {
        SceneManager.LoadScene("Scenes/MainMenu");
    }

}
