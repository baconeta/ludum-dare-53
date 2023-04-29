using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState _instance;
    public enum GameStates
    {
        Start,
        Ferrying,
        Returning,
        Pause,
        End,
    }

    public delegate void OnStartEnter();
    public static event  OnStartEnter onStartEnter;
    
    public delegate void OnFerryingEnter();
    public static event  OnFerryingEnter onFerryingEnter;
    
    public delegate void OnReturningEnter();
    public static event  OnReturningEnter onReturningEnter;
    
    public delegate void OnPauseEnter();
    public static event  OnPauseEnter onPauseEnter;
    
    public delegate void OnEndEnter();
    public static event  OnEndEnter onEndEnter;
    
    private GameStates _currentState;

    private void Awake()
    {
        if (!_instance) _instance = this;
        else Destroy(this);
    }

    public GameStates CurrentState
    {
        get { return _currentState; }
        private set {ChangeState(value); }
    }

    void ChangeState(GameStates newState)
    {
        //Filter double executions
        if (newState == _currentState) return;
        
        //Update state
        _currentState = newState;
        //Changes current from A to B
        switch (_currentState)
        {
            //Initial State, start of game.
            case GameStates.Start:
                onStartEnter?.Invoke();
                break;
            //Upon reaching the shore of Gaia/Over-world/Living/Left/Pickup
            case GameStates.Ferrying:
                onFerryingEnter?.Invoke();
                break;
            //Upon reaching the shore of Underworld/Right/Dropoff
            case GameStates.Returning:
                onReturningEnter?.Invoke();
                break;
            //Upon reaching the shore of Underworld/Right/Dropoff
            case GameStates.Pause:
                onPauseEnter?.Invoke();
                break;
            //Upon reaching the shore of Underworld/Right/Dropoff
            case GameStates.End:
                onEndEnter?.Invoke();
                break;
        }
    }
}
