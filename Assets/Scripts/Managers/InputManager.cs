using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    void OnDialogueNext()
    {
        Debug.Log("Next Dialogue");
    }

    void OnLaunchVoyage()
    {
        Debug.Log("Launch Voyage");
    }

    void OnSteering(InputValue input)
    {
        Debug.Log("Steering = " + input.Get<float>());
    }

    void OnPause()
    {
        Debug.Log("Pausing");
    }

    void OnTap()
    {
        Debug.Log("Tap Down");
    }
}
