using ObjectPooling;
using UnityEngine;

namespace Spawnables
{
    public class Obstacle : MonoBehaviour
    {
        // TODO: decide on appropriate values for these fields
        [SerializeField] private float speed = 0.1f;
        [SerializeField] private float damage = 5.0f;

        private float _bottomBound;
        private Poolable _poolable;

        private void Start() {
            _bottomBound = GameObject.Find("BottomBound").transform.position.y;
            _poolable = GetComponent<Poolable>();
        }

        private void Update() {
            // Slowly move the obstacle down the screen
            transform.position += Vector3.down * (speed * Time.deltaTime);
            // if the obstacle goes off screen, recycle it
            if (transform.position.y < _bottomBound)
            {
                _poolable.Recycle();
            }
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ferry"))
            {
                // TODO: determine game state and damage player appropriately (is this for the player to do?)
                Debug.Log("Player collided with obstacle");
            }
        }

        public void MultiplySpeed(float multiplier)
        {
            speed *= multiplier;
        }

        public void MultiplyDamage(float multiplier)
        {
            damage *= multiplier;
        }

        public float Damage => damage;
    }
}
