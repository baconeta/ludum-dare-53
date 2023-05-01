using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Managers;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    //Has to set in Editor because of execution orders.
    [SerializeField] private PlayerInput playerInput;
    [SerializeField]private string currentInput;
    [SerializeField]private float _steering;
    public float Steering
    {
        get { return _steering; }
        private set { _steering = value; }
    }
    private Vector2 tapPosition;
    public float tapSize = 1;

    public static event Action onDialogueNext;
    public static event Action onLaunchVoyage;
    public static event Action<float> onSteering;
    public static event Action onPause;
    public static event Action<bool> onControlSchemeChange;


    private void Awake()
    {
        if (!Instance) Instance = this;
        else Destroy(this);
    }

    private void Start()
    {
        OnControlsChanged();
    }

    private void Update()
    {
    }

    //KBM via InputSystem. Mobile via OnTapLogic
    void OnDialogueNext()
    {
        if(DialogueManager.Instance.isDialogueActive)
            onDialogueNext?.Invoke();
    }

    //KBM via InputSystem. Mobile via OnTapLogic
    void OnLaunchVoyage()
    {
        onLaunchVoyage?.Invoke();
    }
    
    //Updates Steering from InputSystem
    void OnSteering(InputValue input)
    {
        Steering = input.Get<float>();
        onSteering?.Invoke(Steering);
    }

    void OnSteeringMobile(InputValue input)
    {
        Vector2 joystick = input.Get<Vector2>();
        Steering = joystick.y;
        onSteering?.Invoke(Steering);
    }

    //Unimplemented. Pause via button only right now. Add bindings to controlScheme
    void OnPause()
    {
        onPause?.Invoke();
    }
    
    //OnTap is called if the touch was shorter than minDuration. Ie, runs after down and up finger.
    void OnTap()
    {
        //Only on successful tap
        TapLogic();
    }

    //Updated via input system
    void OnTapPosition(InputValue input)
    {
        tapPosition = input.Get<Vector2>();
    }
    
    //Tap checks against UI > Dialogue > Ship Launch > Other Objects.
    void TapLogic()
    {
        //Is tapping
        
        //Dialogue Inputs
        if(DialogueManager.Instance.isDialogueActive) OnDialogueNext();
        else //World Inputs
        {
            Vector3 tapWorldPos = Camera.main.ScreenToWorldPoint(tapPosition);
            //Is there a collider at the tap position?
            RaycastHit2D hit2D = Physics2D.CircleCast(tapWorldPos, tapSize, Vector2.zero);

            if (hit2D)
            {
                if (hit2D.collider.CompareTag("Ferry"))
                {
                    //Tapped Ferry, Launch. (Self checks for valid launches)
                    OnLaunchVoyage();
                }
                //Other objects, Interactable Obstacles?
            }
        }
    }

    //Called via input system & UI Button.
    //TODO Force Control Validation - Fix this. It will take you to the "correct" control scheme, but what if it has the wrong answer?
    public void OnControlsChanged()
    {
        currentInput = playerInput.currentControlScheme;
        onControlSchemeChange?.Invoke(currentInput == "Mobile");
    }
}
