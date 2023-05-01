using Managers;
using Unity.VisualScripting;
using UnityEngine;

namespace Spawnables
{
    public class Seeker : Obstacle
    {
        private GameObject _target;
        private const float RotationSpeed = 5.0f;

        [Tooltip("The amount of time in seconds to spend seeking the target.")] [SerializeField]
        private float seekDuration = 1.0f;
        
        [Tooltip("The amount of time in seconds to spend idly going with the river flow")] [SerializeField]
        private float idleDuration = 1.0f;

        private new void Start() {
            base.Start();
            // Get a lock on the player so we can follow them
            _target = GameObject.FindWithTag("Ferry");
        }
        private void Update() {
            
            if (!GameStateManager.Instance.IsGameActive()) return;
            
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
        }

        /**
         * Move toward the target in a stepwise fashion.
         */
        private void Move(Vector3 dir) {
            // Figure out amount to move this frame
            var step = currentSpeed * Time.deltaTime;
            if (Time.time % (idleDuration + seekDuration) < idleDuration)
            {
                // Slowly move the obstacle down the screen with the river flow
                transform.position += Vector3.down * step;
            }
            else
            {
                // Move toward the target
                dir += Vector3.down;
                transform.position += dir * step;
            }
        }
    }
    
}
