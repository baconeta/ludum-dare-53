using System;
using System.Collections;
using System.Collections.Generic;
using Spawnables;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    
    private BoatMovement _boatMovement;
    private BoatCapacity _boatCapacity;
    public Transform currentDock;
    
    public Transform leftDock;
    public Transform rightDock;

    public KeyCode voyageStartKey;

    public static event Action OnVoyageStart;
    public static event Action OnVoyageComplete;
 

    // Start is called before the first frame update
    void Awake()
    {
        _boatMovement = GetComponent<BoatMovement>();
        _boatCapacity = GetComponent<BoatCapacity>();
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
        
        //If Not docked, don't check for launches.
        if (currentDock is null) return;
        
        if (Input.GetKeyDown(voyageStartKey))
        {
            StartVoyage();
        }
    }

    void StartVoyage()
    {
        //Starts Going from current dock
        currentDock = null;
        
        //Enable movement
        _boatMovement.EnableMovement();
        
        OnVoyageStart?.Invoke();
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
        OnVoyageComplete?.Invoke();

    }

    void VoyageLost()
    {
        //When a lose condition is met.
        //Calls the OnEndEnter event Action. 
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
