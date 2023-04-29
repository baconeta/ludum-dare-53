using System;
using UnityEngine;

namespace Spawnables
{
    public class Obstacle : MonoBehaviour
    {
        // TODO: decide on appropriate values for these fields
        [SerializeField] private float speed = 0.1f;
        [SerializeField] public float damage = 5.0f;

        private void Update()
        {
            // Slowly move the obstacle down the screen
            transform.position += Vector3.down * (speed * Time.deltaTime);
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                // TODO: determine game state and damage player appropriately (is this for the player to do?)
                Debug.Log("Player collided with obstacle");
            }
        }
    }
}
