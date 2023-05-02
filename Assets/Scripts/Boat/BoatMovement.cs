using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Managers;
using Unity.Mathematics;
using UnityEngine;

public class BoatMovement : MonoBehaviour
{
    [Tooltip("Boat Movement Speed in m/s")]
    public float maxSpeed;

    [ReadOnly(true)][Tooltip("The current speed the boat is moving in m/s")]
    public float currentSpeed;

    [Tooltip("Boat Acceleration in m/s")]
    public float acceleration;
    
    [Tooltip("Boat rotational speed in degrees per second.")]
    public float rotationSpeed;

    [Tooltip("Boat rotational limits in degrees. X: Top facing Limit, Y: Bottom facing Limit" +
             "\n***Different limits are currently unsupported due to flipping math.")]
    public Vector2 rotationLimits;

    public bool alwaysRotateToBottom = false;

    //A -1 to 1 value of the current onSteering<float> eventAction
    private float currentSteering;

    public Vector3 currentDirection;
    [SerializeField] private bool isMoving = false;

    [Tooltip("SET AT RUNTIME. The vertical limit on the screen in WORLD SPACE. X: LowerLimit, Y: UpperLimit")]
    public Vector2 verticalLimit;

    [Range(0f,1f)][Tooltip("What the vertical limits will be multiplied by. E.g. 90% will mean a screen space of 50 units will have a play space of 45 units.")]
    public float limitBorderPercentage = 0.9f;

    [Range(0, 10f)][Tooltip("When approaching a vertical limit, the rotational speed is multiplied by this to correct.")]
    public float limitRotationMultiplier = 2f;

    public float outOfBoundsBumpForce;

    private Camera _mainCamera;
    private float _screenHeight;
    private Rigidbody2D _rigidbody2D;

    private void Awake()
    {
    }
    
    

    // Start is called before the first frame update
    void Start()
    {
        _mainCamera = Camera.main;
        _rigidbody2D = transform.parent.GetComponent<Rigidbody2D>();
        SetVerticalBoundsBasedOnScreenSize();
    }

    private void OnEnable()
    {
        InputManager.onSteering += UpdateSteering;
        BoatController.OnBorderHit += BorderHitBump;
    }

    private void OnDisable()
    {
        InputManager.onSteering -= UpdateSteering;
        BoatController.OnBorderHit -= BorderHitBump;


    }

    void UpdateSteering(float val)
    {
        currentSteering = val;
    }

    private void SetVerticalBoundsBasedOnScreenSize()
    {
        //Abort check if screen height hasnt changed
        if (_screenHeight == Screen.height) return;

        _screenHeight = Screen.height;

        //Lower Limit
        verticalLimit.x = _mainCamera.ScreenToWorldPoint(new Vector2(0, 0)).y;

        //Upper Limit
        verticalLimit.y = _mainCamera.ScreenToWorldPoint(new Vector2(0, _screenHeight)).y;

        verticalLimit *= limitBorderPercentage;
    }

    // Update is called once per Frame
    void Update()
    {
        //TODO Remove from Update and place on an event.
        SetVerticalBoundsBasedOnScreenSize();

        //Is the game currently active? If not, break update.
        if (!GameStateManager.Instance.IsGameActive()) return;

        //Only rotate if the current state is ferrying or returning
        if (isMoving)
        {
            CalculateBoatRotation();
        }
    }

    private void FixedUpdate()
    {
        //Is the game currently active? If not, break update.
        if (!GameStateManager.Instance.IsGameActive()) return;

        //Only move if the current state is ferrying or returning
        if (isMoving)
        {
            CalculateBoatMovement();
        }
    }

    public void EnableMovement()
    {
        //This check is to avoid double calls of EnableMovement.
        if (!isMoving)
        {
            isMoving = true;
            
            //Set direction to left/right based on the current GameState
            switch (GameStateManager.Instance.CurrentState)
            {
                //Edge case of start, may as well prime it for right.
                case GameStateManager.GameStates.Start:
                    currentDirection = Vector3.right;
                    break;
                case GameStateManager.GameStates.Ferrying:
                    currentDirection = Vector3.right;
                    break;
                case GameStateManager.GameStates.Returning:
                    currentDirection = Vector3.left;
                    break;
            }

            //Reset Rotation (You've just launched!)
            transform.rotation = quaternion.Euler(0, 0, 0);
        }
    }

    public void DisableMovement()
    {
        //This check is to avoid double calls of DisableMovement.
        if (isMoving) isMoving = false;
        currentSpeed = 0;
    }

    private void CalculateBoatMovement()
    {
        //If not at max speed
        if (currentSpeed < maxSpeed)
        {
            //Add acceleration
            currentSpeed += acceleration * Time.deltaTime;
            //Clamp to maxSpeed
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
        }
        transform.localPosition = Vector3.zero;
        
        //TODO Movement is currently in transform.Translate, which does not account for collision. Change to collision.
        //Move forwards 
        Vector3 moveVector = currentDirection.normalized;

        //Rotate moveVector by current angle.
        moveVector = transform.rotation * moveVector;
        //Multiply direction by a magnitude speed
        moveVector *= currentSpeed * Time.deltaTime;
        //Zero out Z axis
        moveVector = (Vector2)moveVector;
        
        _rigidbody2D = transform.parent.GetComponent<Rigidbody2D>();

        _rigidbody2D.MovePosition(transform.position += moveVector);

        //Clamp transform.y to vertical limits
        transform.parent.position = new Vector3(transform.position.x,
        Mathf.Clamp(transform.position.y, verticalLimit.x * limitBorderPercentage, verticalLimit.y * limitBorderPercentage),
        transform.position.z);
    }

    private void CalculateBoatRotation()
    {
        Quaternion targetRotation = Quaternion.identity;
        if(alwaysRotateToBottom)
        {
            //Rotation to face down river
            if (currentDirection == Vector3.right)
                targetRotation = Quaternion.Euler(0f, 0f, rotationLimits.y);
            else
                targetRotation = Quaternion.Euler(0f, 0f, -rotationLimits.y);
        } //Else return to 0 rotation

        //Rotation Speed with Calculations
        float calculatedRotationSpeed = rotationSpeed;

        //Rotate towards bottom of screen
        if (currentSteering < 0)
        {
            //Allow this rotation if the vertical limit has not been met.
            //Vertical Limit is Multiplied by borderPercentage to avoid edge cases.
            if (transform.position.y > verticalLimit.x)
            {
                if(!alwaysRotateToBottom)
                {
                    //Flip rotations based on direction, this ensures that controls stay the same depending on direction.
                    if (currentDirection == Vector3.right)
                        targetRotation = Quaternion.Euler(0f, 0f, rotationLimits.y);
                    else
                        targetRotation = Quaternion.Euler(0f, 0f, -rotationLimits.y);
                }
                
            }
            else calculatedRotationSpeed *= limitRotationMultiplier;
        }

        //Rotate towards top of screen
        if (currentSteering > 0)
        {
            //Allow this rotation if the vertical limit has not been met
            //Vertical Limit is Multiplied by borderPercentage to avoid edge cases.
            if (transform.position.y < verticalLimit.y)
            {
                //Flip rotations based on direction, this ensures that controls stay the same depending on direction.
                if (currentDirection == Vector3.right)
                    targetRotation = Quaternion.Euler(0f, 0f, rotationLimits.x);
                else
                    targetRotation = Quaternion.Euler(0f, 0f, -rotationLimits.x);
            }
            else calculatedRotationSpeed *= limitRotationMultiplier;
        }

        //Set rotation
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, calculatedRotationSpeed * Time.deltaTime);

    }

    public void BorderHitBump()
    {
        _rigidbody2D.AddForce(Vector2.up * outOfBoundsBumpForce, ForceMode2D.Force);
    }

    public void DockNudge(Vector2 direction)
    {
        _rigidbody2D.velocity = Vector2.zero;
        _rigidbody2D.AddForce(direction * 1, ForceMode2D.Impulse);
    }
    
}
