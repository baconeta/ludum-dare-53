using System;
using System.Collections;
using UnityEngine;

namespace Managers
{
    public class GameStateManager : MonoBehaviour
    {
        public static GameStateManager Instance;

        [Serializable]
        public enum GameStates
        {
            Start,
            Dialogue,
            Ferrying,
            Returning,
            Pause,
            End,
        }

        [SerializeField] private GameStates _currentState;
        public GameStates CurrentState
        {
            get { return _currentState; }
            set { ChangeState(value); }
        }

        public bool firstFerryCompleted = false;

        [SerializeField] private GameStates _previousState;
        public GameStates PreviousState { get { return _previousState; } }

        //Event Actions for GameStates
        //Subscribe to these to launch other mechanics
        //Subscribe via `GameState.OnStartEnter += MethodToRun`

        // When the scene is loaded
        public static event Action OnStartEnter;

        // Showing a dialogue overlay
        public static event Action OnDialogueEnter;

        //Starting to ferry, fill up on Souls, Run Dialogue
        public static event Action OnFerryingEnter;

        public static event Action OnFirstFerry;

        //Drop off souls, Repair ship, Run Dialogue
        public static event Action OnReturningEnter;

        //Game Paused
        public static event Action OnPauseEnter;
        public static event Action OnPauseExit;

        //Game Ended / Ship Destroyed
        public static event Action OnEndEnter;
        
        private void Awake()
        {
            if (!Instance) Instance = this;
            else Destroy(this);
        }

        private void OnEnable()
        {
            DialogueManager.OnDialogueEnd += VoyageBegins;
            InputManager.onPause += Pause;
        }

        private void OnDisable()
        {
            DialogueManager.OnDialogueEnd -= VoyageBegins;
            InputManager.onPause -= Pause;
        }

        private void Start()
        {
            StartCoroutine(DelayedStart());
        }

        private IEnumerator DelayedStart()
        {
            yield return new WaitForSeconds(1);
            OnStartEnter?.Invoke();
        }

        private void VoyageBegins()
        {
            if (_currentState == GameStates.Start)
                ChangeState(GameStates.Ferrying);
        }

        public void Pause()
        {
            ChangeState(GameStates.Pause);
        }


        void ChangeState(GameStates newState)
        {
            //Filter double executions
            if (newState == _currentState && _currentState != GameStates.Start) return;

            Debug.Log($"Transitioning from {_currentState} to {newState}");

            //Update state
            _previousState = _currentState;
            _currentState = newState;


            if (_previousState == GameStates.Pause)
            {
                OnPauseExit?.Invoke();
                return;
            }

            //Invoke GameState event.
            switch (_currentState)
            {
                //Initial State, start of game.
                case GameStates.Start:
                    //OnStartEnter.Invoke Occurs in GameStateManager Start()
                    break;
                //Upon reaching the shore of Gaia/Over-world/Living/Left/Pickup
                case GameStates.Ferrying:
                    OnFerryingEnter?.Invoke();
                    break;
                //Upon reaching the shore of Underworld/Right/Dropoff
                case GameStates.Returning:
                    //Calls OnFirstFerry for the first time a ferry trip is successful.
                    OnReturningEnter?.Invoke();
                    if (_previousState == GameStates.Ferrying && !firstFerryCompleted)
                    {
                        firstFerryCompleted = true;
                        OnFirstFerry?.Invoke();
                    }
                    //Increment successful ferries.
                    Debug.Log(PlayerPrefs.GetInt("Successful Ferries"));
                    PlayerPrefs.SetInt("Successful Ferries", PlayerPrefs.GetInt("Successful Ferries") + 1);
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


        public void Resume()
        {
            if (_currentState != GameStates.Pause)
            {
                Debug.LogError($"Cannot resume playing from the {_currentState} state");
                return;
            }

            ChangeState(_previousState);
        }

        public bool IsGameActive()
        {
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
