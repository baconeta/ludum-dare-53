using Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Spawnables
{
    public class Seeker : Obstacle
    {
        [SerializeField] private float maxSpeed;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Animator animator;
        private GameObject _target;
        private Transform _attackAttach;
        private const float RotationSpeed = 5.0f;
        [SerializeField] private bool isAttacking;
        [SerializeField] private int attackSortingOrder;
        private int _defaultSortingOrder;

        [Tooltip("The amount of time in seconds to spend seeking the target.")] [SerializeField]
        private float seekDuration = 1.0f;

        [Tooltip("The amount of time in seconds to spend idly going with the river flow")] [SerializeField]
        private float idleDuration = 1.0f;

        private const float MaxJitter = 0.7f;
        private float _jitter;

        private static readonly int SwimSpeed = Animator.StringToHash("SwimSpeed");
        private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
        private static readonly int AttackSpeed = Animator.StringToHash("AttackSpeed");

        private new void Start()
        {
            base.Start();
            //cache sorting order.
            _defaultSortingOrder = spriteRenderer.sortingOrder;
            if (!animator) animator = GetComponentInChildren<Animator>();
            if (!spriteRenderer) spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            // Get a lock on the player so we can follow them
            _target = GameObject.FindWithTag("Ferry");

            // Add random jitter to stop all instance from moving in sync
            _jitter = Random.Range(0.0f, MaxJitter);
        }

        private void Update()
        {
            if (!GameStateManager.Instance.IsGameActive() || !active) return;
            //Is boat is docked, dont attack.
            if (isAttacking)
            {
                transform.position = _attackAttach.position;
                transform.rotation = Quaternion.identity;
            }
            else //Not attacking - Move/Rotate
            {
                if (_target is null) return;

                Vector3 dir = (_target.transform.position - transform.position).normalized;
                if (_target.GetComponent<BoatController>().currentDock != null)
                    dir = Vector3.down;
                RotateTowardTarget(dir);
                Move(dir);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            BoatController.OnVoyageComplete += IncreaseSpeed;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            BoatController.OnVoyageComplete -= IncreaseSpeed;
        }

        /**
         * Rotate to look toward the target.
         */
        private void RotateTowardTarget(Vector3 dir)
        {
            // Look in the direction of the target
            var rot = Quaternion.LookRotation(dir, Vector3.back);
            // Smoothly rotate towards the target
            rot = Quaternion.Lerp(transform.rotation, rot, RotationSpeed * Time.deltaTime);
            // Set the rotation on the z axis only (because 2D)
            transform.eulerAngles = new Vector3(0, 0, rot.eulerAngles.z);

            // Flip the sprite to face the right way
            spriteRenderer.flipX = rot.eulerAngles.z is > 135 or < -45;
        }

        /**
         * Move toward the target in a stepwise fashion.
         */
        private void Move(Vector3 dir)
        {
            // Figure out amount to move this frame
            var step = currentSpeed * Time.deltaTime;
            var animSpeed = 0.0f;

            if ((Time.time + _jitter) % (idleDuration + seekDuration) < idleDuration)
            {
                // Slowly move the obstacle down the screen with the river flow
                transform.position += Vector3.down * step;
            }
            else
            {
                // Move toward the target
                transform.position += dir * step;
                animSpeed = 1.0f;
            }

            animator.SetFloat(SwimSpeed, animSpeed);
        }

        public void StartAttackAnimation(Transform attach)
        {
            isAttacking = true;
            GetComponent<CircleCollider2D>().enabled = false;
            spriteRenderer.sortingOrder = attackSortingOrder;
            animator.SetBool(IsAttacking, true);
            animator.SetFloat(AttackSpeed, 1);
            _attackAttach = attach;
        }

        public void EndAttackAnimation()
        {
            //Reset anims
            isAttacking = false;
            GetComponent<CircleCollider2D>().enabled = true;
            spriteRenderer.sortingOrder = _defaultSortingOrder;
            animator.SetBool(IsAttacking, false);
            animator.SetFloat(AttackSpeed, 0);

            RemoveFromScene();
        }

        private void IncreaseSpeed()
        {
            currentSpeed += 0.075f * GameStateManager.Instance.voyagesCompletedThisRound;
            if (currentSpeed >= maxSpeed)
            {
                currentSpeed = maxSpeed;
            }
        }
    }
}