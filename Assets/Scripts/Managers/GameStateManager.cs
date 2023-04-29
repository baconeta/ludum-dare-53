using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;
    [Serializable]
    public enum GameStates
    {
        Start,
        Ferrying,
        Returning,
        Pause,
        End,
    }
    
    
    [SerializeField] private GameStates _currentState;
    public GameStates CurrentState
    {
        get { return _currentState; }
        set {ChangeState(value); }
    }
    
    //Event Actions for GameStates
    //Subscribe to these to launch other mechanics
    //Subscribe via `GameState.OnStartEnter += MethodToRun`
    
    //Game Start - Run intros/Dialogue etc
    public static event Action OnStartEnter;
    //Starting to ferry, fill up on Souls, Run Dialogue
    public static event Action OnFerryingEnter;
    //Drop off souls, Repair ship, Run Dialogue
    public static event Action OnReturningEnter;
    //Game Paused
    public static event Action OnPauseEnter;
    //Game Ended / Ship Destroyed
    public static event Action OnEndEnter;
    

    private void Awake()
    {
        if (!Instance) Instance = this;
        else Destroy(this);
    }

    private void Start()
    {
        //TODO Remove for actual game start.
        CurrentState = GameStates.Ferrying;
    }


    void ChangeState(GameStates newState)
    {
        //Filter double executions
        if (newState == _currentState) return;
        
        //Update state
        _currentState = newState;
        
        //Invoke GameState event.
        switch (_currentState)
        {
            //Initial State, start of game.
            case GameStates.Start:
                OnStartEnter?.Invoke();
                break;
            //Upon reaching the shore of Gaia/Over-world/Living/Left/Pickup
            case GameStates.Ferrying:
                OnFerryingEnter?.Invoke();
                break;
            //Upon reaching the shore of Underworld/Right/Dropoff
            case GameStates.Returning:
                OnReturningEnter?.Invoke();
                break;
            //Upon reaching the shore of Underworld/Right/Dropoff
            case GameStates.Pause:
                OnPauseEnter?.Invoke();
                break;
            //Upon reaching the shore of Underworld/Right/Dropoff
            case GameStates.End:
                OnEndEnter?.Invoke();
                break;
        }
    }

    public bool IsGameActive()
    {
        //Active game modes are Ferrying and Returning
        //Inactive Game Modes are Start, Pause, and End.
        return _currentState == GameStates.Ferrying || _currentState == GameStates.Returning;
    }
}
