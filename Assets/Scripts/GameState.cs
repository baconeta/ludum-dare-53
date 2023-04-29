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
    
    public static event Action OnStartEnter;
    public static event Action OnFerryingEnter;
    public static event Action OnReturningEnter;
    public static event Action OnPauseEnter;
    public static event Action OnEndEnter;
    
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
}
