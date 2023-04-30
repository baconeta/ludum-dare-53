using System;
using System.Collections;
using System.Collections.Generic;
using Spawnables;
using UnityEngine;
using UnityEngine.UIElements;

public class BoatController : MonoBehaviour
{
    
    private BoatMovement _boatMovement;
    private BoatCapacity _boatCapacity;
    private Animator _animator;
    public Transform currentDock;
    
    public Transform leftDock;
    public Transform rightDock;

    public KeyCode voyageStartKey;
 

    // Start is called before the first frame update
    void Awake()
    {
        _boatMovement = GetComponent<BoatMovement>();
        _boatCapacity = GetComponent<BoatCapacity>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        currentDock = leftDock;
    }
    
    private void OnEnable()
    {
        //Subscribe to Loss Condition Events
        BoatCapacity.OnBoatDestroyed += VoyageLost;
        BoatCapacity.OnAllSoulsLost += VoyageLost;
    }
    
    private void OnDisable()
    {
        //Subscribe to Loss Condition Events
        BoatCapacity.OnBoatDestroyed -= VoyageLost;
        BoatCapacity.OnAllSoulsLost -= VoyageLost;
    }
    
    void Update()
    {
        //If game is inactive, return.
        if (!GameStateManager.Instance.IsGameActive()) return;

        UpdateAnimations();
        
        //If Not docked, don't check for launches.
        if (currentDock is null) return;
        
        if (Input.GetKeyDown(voyageStartKey))
        {
            StartVoyage();
        }
    }

    private void UpdateAnimations()
    {
        //TODO FIX! .EulerAngles returns mathematically different value to the inspector.
        //It works for now and seems to be okay.
        float angle = transform.rotation.eulerAngles.z;
        
        float angleNormalized = 0;
        
        //If in a negative angle
        if (angle >= 360 - Mathf.Abs(_boatMovement.rotationLimits.y))
        {
            angleNormalized = (angle - 360) / Mathf.Abs(_boatMovement.rotationLimits.y);
        }
        else angleNormalized = angle / _boatMovement.rotationLimits.x;
        
        _animator.SetFloat("BoatAngleNormalized", angleNormalized);
    }

    void StartVoyage()
    {
        //Starts Going from current dock
        currentDock = null;
        
        //Enable movement
        _boatMovement.EnableMovement();
    }
    
    void CompleteVoyage()
    {
        //Completes the current voyage
        
        //Informs the GameState that it has reached its destination
        switch (GameStateManager.Instance.CurrentState)
        {
            //Completed Ferrying - Dropped off all Souls.
            case GameStateManager.GameStates.Ferrying:
                currentDock = rightDock;
                GameStateManager.Instance.CurrentState = GameStateManager.GameStates.Returning;
                break;
            
            //Completed Returning - Picking up new Souls.
            case GameStateManager.GameStates.Returning:
                currentDock = leftDock;
                GameStateManager.Instance.CurrentState = GameStateManager.GameStates.Ferrying;
                break;
        }
        
        //Disable movement
        _boatMovement.DisableMovement();
        
    }

    void VoyageLost()
    {
        //When a lose condition is met.
        GameStateManager.Instance.CurrentState = GameStateManager.GameStates.End;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        //If not docked docked, then check for collisions
        if (currentDock is null)
        {
            //Arrived at the shore!
            if (other.gameObject.CompareTag("Shore"))
            {
                CompleteVoyage();
            }
            else if (other.gameObject.CompareTag("Obstacle"))
            {
                _boatCapacity.DealDamageToBoat(other.GetComponent<Obstacle>().Damage);
            }
        }
        
    }
}
