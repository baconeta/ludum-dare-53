using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    
    // ATM, a simple function to launch the game scene
    public void PlayGame()
    {
        SceneManager.LoadScene("Scenes/BrayScene");
    }

}
