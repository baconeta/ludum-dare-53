using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
    
    [Tooltip("Boat rotational limits in degrees. X: Clockwise Limit, Y: AntiClockwise Limit")]
    public Vector2 rotationLimits;

    public Vector3 currentDirection;
    [SerializeField] private bool isMoving = false;

    [Tooltip("SET AT RUNTIME. The vertical limit on the screen in WORLD SPACE. X: LowerLimit, Y: UpperLimit")]
    public Vector2 verticalLimit;
    
    [Range(0f,1f)][Tooltip("What the vertical limits will be multiplied by. E.g. 90% will mean a screen space of 50 units will have a play space of 45 units.")]
    public float limitBorderPercentage = 0.9f;

    [Range(0, 10f)][Tooltip("When approaching a vertical limit, the rotational speed is multiplied by this to correct.")]
    public float limitRotationMultiplier = 2f;
    
    public KeyCode rotateLeftKeyCode;
    public KeyCode rotateRightKeyCode;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        SetVerticalBoundsBasedOnScreenSize();
    }

    private void SetVerticalBoundsBasedOnScreenSize()
    {
        Vector2 screenDimensions = new Vector2(0, Screen.height);

        //Lower Limit
        verticalLimit.x = Camera.main.ScreenToWorldPoint(new Vector2(0, screenDimensions.x)).y;

        //Upper Limit
        verticalLimit.y = Camera.main.ScreenToWorldPoint(new Vector2(0, screenDimensions.y)).y;

        verticalLimit *= limitBorderPercentage;
    }

    // Update is called once per frame
    void Update()
    {
        //Is the game currently active? If not, break update.
        if (!GameStateManager.Instance.IsGameActive()) return;
        
        //Only move if the current state is ferrying or returning
        if (isMoving)
        {
            CalculateBoatMovement();
            CalculateBoatRotation();
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
                case GameStateManager.GameStates.Ferrying:
                    currentDirection = Vector3.right;
                    //UnFlip Body
                    transform.localScale = Vector3.one;
                    break;
                case GameStateManager.GameStates.Returning:
                    currentDirection = Vector3.left;
                    //Flip Body
                    Vector3 flipScale = new Vector3(-1, 1, 1);
                    transform.localScale = flipScale;
                    break;
            }
            
            //Reset Rotation (You've just launched!)
            transform.rotation = quaternion.Euler(0,0,0);

        }
        
    }

    public void DisableMovement()
    {
        //This check is to avoid double calls of DisableMovement.
        if (isMoving) isMoving = false;
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

        //Move forwards
        transform.Translate(currentSpeed * Time.deltaTime * currentDirection);

        //Clamp transform.y to vertical limits
        transform.position = new Vector3(transform.position.x,
            Mathf.Clamp(transform.position.y, verticalLimit.x, verticalLimit.y),
            transform.position.z);

    }

    private void CalculateBoatRotation()
    {
        //Rotation From
        Quaternion currentRotation = transform.rotation;
        //Rotation to
        Quaternion targetRotation = Quaternion.identity;
        //Rotation Speed with Calculations
        float calculatedRotationSpeed = rotationSpeed;
        
        //Rotate Left/Port/Downwards
        if (Input.GetKey(rotateLeftKeyCode))
        {
            //Allow this rotation if the vertical limit has not been met.
            //Vertical Limit is Multiplied by borderPercentage to avoid edge cases.
            if (transform.position.y < verticalLimit.y * limitBorderPercentage)
            {
                //Flip rotations based on direction, this ensures that controls stay the same depending on direction.
                if (currentDirection == Vector3.right)
                    targetRotation = Quaternion.Euler(0f, 0f, rotationLimits.x);
                else
                    targetRotation = Quaternion.Euler(0f, 0f, rotationLimits.y);
            }
            else calculatedRotationSpeed *= limitRotationMultiplier;
        }

        //Rotate Right/Starboard/Upwards
        if (Input.GetKey(rotateRightKeyCode))
        {
            //Allow this rotation if the vertical limit has not been met
            //Vertical Limit is Multiplied by borderPercentage to avoid edge cases.
            if (transform.position.y > verticalLimit.x * limitBorderPercentage)
            {
                //Flip rotations based on direction, this ensures that controls stay the same depending on direction.
                if (currentDirection == Vector3.right)
                    targetRotation = Quaternion.Euler(0f, 0f, rotationLimits.y);
                else
                    targetRotation = Quaternion.Euler(0f, 0f, rotationLimits.x);
            }
            else calculatedRotationSpeed *= limitRotationMultiplier;
        }
        
        //Set rotation
        transform.rotation = Quaternion.RotateTowards(currentRotation, targetRotation, calculatedRotationSpeed * Time.deltaTime);;
        
    }
}
