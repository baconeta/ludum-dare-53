using UnityEngine;

namespace Spawnables
{
    public class Seeker : Obstacle
    {
        private GameObject _target;
        private const float RotationSpeed = 5.0f;

        private void OnEnable() {
            _target = GameObject.FindWithTag("Ferry");
        }
        private void Update() {
            
            _target ??= GameObject.FindWithTag("Ferry");
            
            // Rotate to face the target
            var dir = (_target.transform.position - transform.position).normalized;
            // Look in the direction of the target
            var rot = Quaternion.LookRotation(dir, Vector3.back);
            // Smoothly rotate towards the target
            rot = Quaternion.Lerp(transform.rotation, rot, RotationSpeed * Time.deltaTime);
            // Set the rotation on the z axis only (because 2D)
            transform.eulerAngles = new Vector3(0, 0, rot.eulerAngles.z);

            if (!GameStateManager.Instance.IsGameActive()) return;

            // Slowly move the obstacle down the screen
            transform.position += Vector3.down * (currentSpeed * Time.deltaTime);
        }
    }
}
