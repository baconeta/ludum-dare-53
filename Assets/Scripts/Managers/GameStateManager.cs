using System;
using UnityEngine;

namespace Managers {
    public class GameStateManager : MonoBehaviour {
        public static GameStateManager Instance;

        [Serializable]
        public enum GameStates {
            Start,
            Dialogue,
            Ferrying,
            Returning,
            Pause,
            End,
        }


        [SerializeField] private GameStates _currentState;

        public GameStates CurrentState {
            get { return _currentState; }
            set { ChangeState(value); }
        }

        [SerializeField] private GameStates _previousState;

        public GameStates PreviousState {
            get { return _previousState; }
        }

        //Event Actions for GameStates
        //Subscribe to these to launch other mechanics
        //Subscribe via `GameState.OnStartEnter += MethodToRun`

        //Game Start - Run intros/Dialogue etc
        public static event Action OnStartEnter;

        // Showing a dialogue overlay
        public static event Action OnDialogueEnter;

        //Starting to ferry, fill up on Souls, Run Dialogue
        public static event Action OnFerryingEnter;

        //Drop off souls, Repair ship, Run Dialogue
        public static event Action OnReturningEnter;

        //Game Paused
        public static event Action OnPauseEnter;

        //Game Ended / Ship Destroyed
        public static event Action OnEndEnter;

        private void Awake() {
            if (!Instance) Instance = this;
            else Destroy(this);
        }

        private void Start() {
            //TODO Remove for actual game start.
            CurrentState = GameStates.Ferrying;
        }


        void ChangeState(GameStates newState) {
            //Filter double executions
            if (newState == _currentState) return;

            Debug.Log($"Transitioning from {_currentState} to {newState}");

            //Update state
            _previousState = _currentState;
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
                // Whenever the dialogue overlay is displayed
                case GameStates.Dialogue:
                    OnDialogueEnter?.Invoke();
                    break;
                //Upon reaching the shore of Underworld/Right/Dropoff
                case GameStates.Returning:
                    OnReturningEnter?.Invoke();
                    break;
                //Upon UI pause
                case GameStates.Pause:
                    OnPauseEnter?.Invoke();
                    break;
                //Upon game over
                case GameStates.End:
                    OnEndEnter?.Invoke();
                    break;
            }
        }

        public void Resume() {
            if (_currentState != GameStates.Pause)
            {
                Debug.LogError($"Cannot resume playing from the {_currentState} state");
                return;
            }

            ChangeState(_previousState);
        }

        public bool IsGameActive() {
            //Active game modes are Ferrying and Returning
            //Inactive Game Modes are Start, Pause, and End.
            bool isActive = _currentState == GameStates.Ferrying || _currentState == GameStates.Returning;

            //TODO Do we want this here? This causes obstacles to pause during dialogue (The river still flows)
            if (isActive)
            {
                //Check if dialogue is open, if so, pause game.
                if (DialogueManager.instance.isDialogueActive) isActive = false;
            }

            return isActive;
        }
    }
}
