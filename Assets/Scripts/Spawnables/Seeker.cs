using System;
using Managers;
using Unity.VisualScripting;
using UnityEngine;

namespace Spawnables
{
    public class Seeker : Obstacle
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Animator animator;
        private GameObject _target;
        private Transform attackAttach;
        private const float RotationSpeed = 5.0f;
        [SerializeField] private bool isAttacking;
        [SerializeField]private int attackSortingOrder;
        private int defaultSortingOrder;

        [Tooltip("The amount of time in seconds to spend seeking the target.")] [SerializeField]
        private float seekDuration = 1.0f;
        
        [Tooltip("The amount of time in seconds to spend idly going with the river flow")] [SerializeField]
        private float idleDuration = 1.0f;

        private new void Start()
        {
            base.Start();
            //cache sorting order.
            defaultSortingOrder = spriteRenderer.sortingOrder;
            if (!animator) animator = GetComponentInChildren<Animator>();
            if (!spriteRenderer) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            
            // Get a lock on the player so we can follow them
            _target = GameObject.FindWithTag("Ferry");
        }
        private void Update() {
            
            if (!GameStateManager.Instance.IsGameActive()) return;
            if (isAttacking)
            {
                transform.position = attackAttach.position;
                transform.rotation = Quaternion.identity;
            }
            else //Not attacking - Move/Rotate
            {
                // If the target is null, find it
                //_target ??= GameObject.FindWithTag("Ferry");

                var dir = Vector3.forward;
                if (_target is not null)
                {
                    // Rotate to face the target
                    dir = (_target.transform.position - transform.position).normalized;
                }
            
                RotateTowardTarget(dir);
                Move(dir);
            }
            
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

            if (rot.eulerAngles.z > 135 || rot.eulerAngles.z < -45) spriteRenderer.flipX = true;
            else spriteRenderer.flipX = false;

        }

        /**
         * Move toward the target in a stepwise fashion.
         */
        private void Move(Vector3 dir) {
            
            // Figure out amount to move this frame
            var step = currentSpeed * Time.deltaTime;
            float animSpeed = 0;
            
            
            if (Time.time % (idleDuration + seekDuration) < idleDuration)
            {
                // Slowly move the obstacle down the screen with the river flow
                transform.position += Vector3.down * step;
            }
            else
            {
                // Move toward the target
                //dir += Vector3.down;
                transform.position += dir * step;
                animSpeed = 1;
            }
            animator.SetFloat("SwimSpeed", animSpeed);
        }

        public void StartAttackAnimation(Transform attach)
        {
            isAttacking = true;
            GetComponent<CircleCollider2D>().enabled = false;
            spriteRenderer.sortingOrder = attackSortingOrder;
            animator.SetBool("IsAttacking", true);
            animator.SetFloat("AttackSpeed", 1);
            attackAttach = attach;
        }

        public void EndAttackAnimation()
        {
            //Reset anims
            isAttacking = false;
            GetComponent<CircleCollider2D>().enabled = true;
            spriteRenderer.sortingOrder = defaultSortingOrder;
            animator.SetBool("IsAttacking", false);
            animator.SetFloat("AttackSpeed", 0);
            
            RemoveFromScene();
        }
    }
    
}
