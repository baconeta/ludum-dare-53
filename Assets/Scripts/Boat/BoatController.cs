using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    
    private BoatMovement _boatMovement;
    public Transform currentDock;
    
    public Transform leftDock;
    public Transform rightDock;

    public KeyCode voyageStartKey;
 

    // Start is called before the first frame update
    void Awake()
    {
        _boatMovement = GetComponent<BoatMovement>();
    }

    private void Start()
    {
        currentDock = leftDock;
    }
    
    void Update()
    {
        //If game is inactive, return.
        if (!GameState.Instance.IsGameActive()) return;
        
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
    }
    
    void CompleteVoyage()
    {
        //Completes the current voyage
        
        //Informs the GameState that it has reached its destination
        switch (GameState.Instance.CurrentState)
        {
            //Completed Ferrying - Dropped off all Souls.
            case GameState.GameStates.Ferrying:
                currentDock = rightDock;
                GameState.Instance.CurrentState = GameState.GameStates.Returning;
                break;
            
            //Completed Returning - Picking up new Souls.
            case GameState.GameStates.Returning:
                currentDock = leftDock;
                GameState.Instance.CurrentState = GameState.GameStates.Ferrying;
                break;
        }
        
        //Disable movement
        _boatMovement.DisableMovement();
        
    }

    void VoyageLost()
    {
        //When a lose condition is met.
        GameState.Instance.CurrentState = GameState.GameStates.End;
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
        }
        
    }
}
