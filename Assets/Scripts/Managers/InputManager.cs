using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    public float Steering { get; private set; }
    private Vector2 tapPosition;
    public float tapSize = 1;

    public static event Action onDialogueNext;
    public static event Action onLaunchVoyage;
    public static event Action<float> onSteering;
    public static event Action onPause;

    private void Awake()
    {
        if (!Instance) Instance = this;
        else Destroy(this);
    }

    void OnDialogueNext()
    {
        onDialogueNext?.Invoke();
    }

    void OnLaunchVoyage()
    {
        onLaunchVoyage?.Invoke();

    }

    void OnSteering(InputValue input)
    {
        Steering = input.Get<float>();
        onSteering?.Invoke(Steering);
    }

    void OnPause()
    {
        onPause?.Invoke();
    }

    void OnTap()
    {
        TapLogic();
    }

    void OnTapPosition(InputValue input)
    {
        Debug.Log(tapPosition);
        tapPosition = input.Get<Vector2>();
    }

    void TapLogic()
    {
        if(DialogueManager.instance.isDialogueActive) onDialogueNext?.Invoke();
        else
        {
            Vector3 tapWorldPos = Camera.main.ScreenToWorldPoint(tapPosition);
            //Is there a collider at the tap position?
            RaycastHit2D hit2D = Physics2D.CircleCast(tapWorldPos, tapSize, Vector2.zero);

            if (hit2D.collider.CompareTag("Ferry"))
            {
                //Tapped Ferry, Launch. (Self checks for valid launches)
                onLaunchVoyage?.Invoke();
            }
            else
            {
                //Other objects?
            }
            
        }
    }
}
