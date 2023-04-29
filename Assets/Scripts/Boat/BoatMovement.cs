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
    
    public KeyCode rotateLeftKeyCode;
    public KeyCode rotateRightKeyCode;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Only move if the current state is ferrying or returning
        if (isMoving)
        {
            CalculateBoatRotation();
            CalculateBoatMovement();
        }
    }

    public void EnableMovement()
    {
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
            
            transform.rotation = quaternion.Euler(0,0,0);
            
            
        }
        
    }

    public void DisableMovement()
    {
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
    }

    private void CalculateBoatRotation()
    {
        //Rotation From
        Quaternion currentRotation = transform.rotation;
        //Rotation to
        Quaternion targetRotation = Quaternion.identity;
        
        //Rotate Left/Port/Downwards
        if (Input.GetKey(rotateLeftKeyCode))
        {
            //Flip rotations based on direction, this ensures that controls stay the same depending on direction.
            if(currentDirection == Vector3.right)
                targetRotation = Quaternion.Euler(0f, 0f, rotationLimits.x);
            else
                targetRotation = Quaternion.Euler(0f, 0f, rotationLimits.y);
        }

        //Rotate Right/Starboard/Upwards
        if (Input.GetKey(rotateRightKeyCode))
        {
            //Flip rotations based on direction, this ensures that controls stay the same depending on direction.
            if(currentDirection == Vector3.right)
                targetRotation = Quaternion.Euler(0f, 0f, rotationLimits.y);
            else
                targetRotation = Quaternion.Euler(0f, 0f, rotationLimits.x);
        }
        
        //Set rotation
        transform.rotation = Quaternion.RotateTowards(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);;
    }
}
