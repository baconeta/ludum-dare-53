using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using Managers;
using Spawnables;
using UnityEngine;
using UnityEngine.UIElements;

public class BoatController : MonoBehaviour
{
    [Header("Components")]
    public GameObject boatGameObject;
    public GameObject scriptsGameObject;
    public Transform seekerAttach;
    private BoatMovement _boatMovement;
    private BoatCapacity _boatCapacity;
    
    [Header("Boat Animation")]
    private Animator _boatAnimator;
    private SpriteRenderer _boatSpriteRenderer;
    
    [Header("Charon Animation")]
    public GameObject charonGameObject;
    [SerializeField] private float charonRowSpeedMultiplier = 1;
    [SerializeField] private AnimationCurve charonRowSpeedCurve;
    private Animator _charonAnimator;
    private SpriteRenderer _charonSpriteRenderer;
    
    [Header("Dock/Shore Information")]
    public Transform currentDock;
    public Transform leftShore;
    public Transform rightDock;

    public static event Action OnVoyageStart;
    public static event Action OnVoyageComplete;

    public static event Action OnDamageTaken;
    public static event Action OnBorderHit;
    
    void Awake()
    {
        _boatMovement = GetComponentInChildren<BoatMovement>();
        _boatCapacity = GetComponentInChildren<BoatCapacity>();
        
        _boatAnimator = boatGameObject.GetComponent<Animator>();
        _boatSpriteRenderer = boatGameObject.GetComponent<SpriteRenderer>();
        
        _charonAnimator = charonGameObject.GetComponent<Animator>();
        _charonSpriteRenderer = charonGameObject.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        currentDock = leftShore;
    }
    
    private void OnEnable()
    {
        //Subscribe to Loss Condition Events
        BoatCapacity.OnBoatDestroyed += VoyageLost;
        BoatCapacity.OnAllSoulsLost += VoyageLost;
        //Subscribe to launch voyage input
        InputManager.onLaunchVoyage += StartVoyage;
    }
    
    private void OnDisable()
    {
        BoatCapacity.OnBoatDestroyed -= VoyageLost;
        BoatCapacity.OnAllSoulsLost -= VoyageLost;
        InputManager.onLaunchVoyage -= StartVoyage;

    }
    
    void Update()
    {
        //If game is inactive, return.
        if (!GameStateManager.Instance.IsGameActive()) return;

        UpdateAnimations();
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
            _charonSpriteRenderer.flipX = true;
            _boatSpriteRenderer.flipX = true;
            //Flip angle to correct sprite
            angleNormalized *= -1;
        }
        else
        {
            _charonSpriteRenderer.flipX = false;
            _boatSpriteRenderer.flipX = false;

        }
        
        //Update animator float so the animations change based on the boat angle.
        _boatAnimator.SetFloat("BoatAngleNormalized", angleNormalized);
        
        //Update charon's animation speed based on rowing speed.
        float animSpeed = charonRowSpeedCurve.Evaluate(_boatMovement.currentSpeed / _boatMovement.maxSpeed);
        _charonAnimator.SetFloat("RowSpeed", (animSpeed * charonRowSpeedMultiplier) * _boatMovement.currentSpeed);
    }

    void StartVoyage()
    {
        if (DialogueManager.Instance.isDialogueActive) return;
        
        //Not docked, cannot launch from nothing!
        if (currentDock is null) return;

        //Enable current dock collision
        StartCoroutine(EnableDockCollisionAfterDuration(currentDock));

        //Starts Going from current dock
        currentDock = null;
        
        //Enable movement
        _boatMovement.EnableMovement();
        
        OnVoyageStart?.Invoke();
    }

    IEnumerator EnableDockCollisionAfterDuration(Transform dock)
    {
        yield return new WaitForSeconds(5f);
        dock.GetComponent<PolygonCollider2D>().enabled = true;
        yield return null;
    }
    
    void CompleteVoyage()
    {
        
        //Completes the current voyage
        
        //Informs the GameState that it has reached its destination
        switch (GameStateManager.Instance.CurrentState)
        {
            //Completed Ferrying - Dropped off all Souls.
            case GameStateManager.GameStates.Ferrying:
                // Play delivery sound
                switch (_boatCapacity.CurrentLoad)
                {
                    case 1:
                        AudioWrapper.Instance.PlaySound("delivery-many-souls");
                        break;
                    case < 50: // TODO remove hardcoded value
                        AudioWrapper.Instance.PlaySound("deliver-single-soul");
                        break;
                    default:
                        AudioWrapper.Instance.PlaySound("deliver-all-dem-souls");
                        break;
                }
                
                currentDock = rightDock;
                GameStateManager.Instance.CurrentState = GameStateManager.GameStates.Returning;
                break;
            
            //Completed Returning - Picking up new Souls.
            case GameStateManager.GameStates.Returning:
                // Play collection sound
                AudioWrapper.Instance.PlaySound("soul-collection");
                currentDock = leftShore;
                GameStateManager.Instance.CurrentState = GameStateManager.GameStates.Ferrying;
                break;
        }
        //Disable current dock collision
        currentDock.GetComponent<PolygonCollider2D>().enabled = false;
        
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

    public void OnCollisionEnter2D(Collision2D other)
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
                // Play obstacle hit sound
                AudioWrapper.Instance.PlaySound("ship-hit");
                
                _boatCapacity.DealDamageToBoat(other.gameObject.GetComponent<Obstacle>().Damage);
                OnDamageTaken?.Invoke();

                var seeker = other.gameObject.GetComponent<Seeker>();
                if (seeker != null)
                    seeker.StartAttackAnimation(seekerAttach);

                var skull = other.gameObject.GetComponent<Yeet>();
                if(skull != null)
                {
                    skull.DestroySkull();
                }

            }
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("OutOfBounds"))
        {
            OnBorderHit?.Invoke();
            OnDamageTaken?.Invoke();
            _boatCapacity.DealDamageToBoat(1);
        }
    }
}
