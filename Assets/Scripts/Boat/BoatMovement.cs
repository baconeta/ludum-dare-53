using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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

    //TODO Replace with gamestate
    [Tooltip("Is the boat currently moving? To be replaced by GameState")]
    public bool isMoving;

    public KeyCode rotateLeftKeyCode;
    public KeyCode rotateRightKeyCode;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            CalculateBoatRotation();
            CalculateBoatMovement();
        }
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

        //TODO Change direction based on current GameState
        Vector3 moveDirection = Vector3.right;

        //Move forwards
        transform.Translate(currentSpeed * Time.deltaTime * moveDirection);
    }

    private void CalculateBoatRotation()
    {
        //Rotation From
        Quaternion currentRotation = transform.rotation;
        //Rotation to
        Quaternion targetRotation = Quaternion.identity;
        
        //Rotate Left
        if (Input.GetKey(rotateLeftKeyCode))
        {
            targetRotation = Quaternion.Euler(0f, 0f, rotationLimits.x);
        }

        //Rotate Right
        if (Input.GetKey(rotateRightKeyCode))
        {
            targetRotation = Quaternion.Euler(0f, 0f, rotationLimits.y);
        }
        
        //Set rotation
        transform.rotation = Quaternion.RotateTowards(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);;
    }
}
