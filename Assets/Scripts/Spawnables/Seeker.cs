using UnityEngine;

namespace Spawnables
{
    public class Seeker : Obstacle
    {
        private GameObject _target;
        private const float RotationSpeed = 5.0f;

        private void OnEnable() {
            // Get a lock on the player so we can follow them
            _target = GameObject.FindWithTag("Ferry");
        }
        private void Update() {
            
            if (!GameStateManager.Instance.IsGameActive()) return;
            
            // TODO: only change direction every so often
            
            // If the target is null, find it
            _target ??= GameObject.FindWithTag("Ferry");
            
            // Rotate to face the target
            var dir = (_target.transform.position - transform.position).normalized;
            // Look in the direction of the target
            var rot = Quaternion.LookRotation(dir, Vector3.back);
            // Smoothly rotate towards the target
            rot = Quaternion.Lerp(transform.rotation, rot, RotationSpeed * Time.deltaTime);
            // Set the rotation on the z axis only (because 2D)
            transform.eulerAngles = new Vector3(0, 0, rot.eulerAngles.z);

            // TODO: only do this every so often
            // Take a step in the direction of the target
            var step = currentSpeed * Time.deltaTime;

            dir += Vector3.down;
            // Slowly move the obstacle down the screen
            transform.position += dir * step;
        }
    }
}
