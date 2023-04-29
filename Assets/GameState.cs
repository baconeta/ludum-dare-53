using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    private static GameState _instance;
    public enum GameStates
    {
        Ferrying,
        Returning,
    }

    private GameStates _currentState;

    private void Awake()
    {
        if (!_instance) _instance = this;
        else Destroy(this);
    }

    public GameStates CurrentState
    {
        get { return _currentState; }
        private set {ToggleState(); }
    }

    void ToggleState()
    {
        //Changes current from A to B
        switch (_currentState)
        {
            case GameStates.Ferrying:
                _currentState = GameStates.Returning;
                break;
            case GameStates.Returning:
                _currentState = GameStates.Ferrying;
                break;
        }
    }
}
