using System;
using System.Collections;
using System.Collections.Generic;
using Spawnables;
using UnityEngine;
using UnityEngine.UIElements;

public class BoatController : MonoBehaviour
{
    [Header("Components")]
    private BoatMovement _boatMovement;
    private BoatCapacity _boatCapacity;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    public GameObject scriptsGameObject;
    public GameObject animatorGameObject;

    [Header("Dock/Shore Information")]
    public Transform currentDock;
    public Transform leftDock;
    public Transform rightDock;

    [Header("Temp")]
    public KeyCode voyageStartKey;
 

    // Start is called before the first frame update
    void Awake()
    {
        _boatMovement = GetComponentInChildren<BoatMovement>();
        _boatCapacity = GetComponentInChildren<BoatCapacity>();
        _animator = GetComponentInChildren<Animator>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
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
        //Get the angle of the SCRIPTS game object. This is separate from Animations to avoid rotational visual glitches.
        float angle = scriptsGameObject.transform.rotation.eulerAngles.z;
        
        float angleNormalized;
        
        //If in a negative angle
        if (angle >= 360 - Mathf.Abs(_boatMovement.rotationLimits.y))
        {
            angleNormalized = (angle - 360) / Mathf.Abs(_boatMovement.rotationLimits.y);
        }
        else angleNormalized = angle / _boatMovement.rotationLimits.x;
        
        //Flip sprite depending on current heading.
        //TODO Animator bool is currently unused.
        if (_boatMovement.currentDirection == Vector3.left)
        {
            _animator.SetBool("FlipX", true);
            _spriteRenderer.flipX = true;
            //Flip angle to correct sprite
            angleNormalized *= -1;
        }
        else
        {
            _animator.SetBool("FlipX", false);
            _spriteRenderer.flipX = false;
        }
        
        //Update animator float so the animations change based on the boat angle.
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
