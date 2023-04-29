using ObjectPooling;
using UnityEngine;

namespace Spawnables
{
    public class Obstacle : MonoBehaviour, IPoolableExecution, IRecyclable
    {
        // TODO: decide on appropriate values for these fields
        [SerializeField] private float speed = 1f;
        //Changed to integer as souls are always whole numbers.
        [SerializeField] private int damage = 1;

        private float _bottomBound;
        private Poolable _poolable;
        private float _length;

        private void Start()
        {
            _bottomBound = GameObject.Find("BottomLimit").transform.position.y;
            _poolable = GetComponent<Poolable>();
            _length = GetComponent<Renderer>().bounds.size.y;
        }

        private void Update()
        {
            // Slowly move the obstacle down the screen
            transform.position += Vector3.down * (speed * Time.deltaTime);
            // if the obstacle goes off screen, recycle it
            if (transform.position.y + _length < _bottomBound)
            {
                RemoveFromScene();
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

        /**
         * Change the speed of this obstacle by the given multiplier.
         */
        public void MultiplySpeed(float multiplier)
        {
            speed *= multiplier;
        }

        /**
         * Change the amount of damage this obstacle inflicts by the given multiplier.
         */
        public void MultiplyDamage(float multiplier)
        {
            damage *= Mathf.FloorToInt(multiplier);
        }

        /**
         * The amount of damage this obstacle inflicts.
         */
        public int Damage => damage;
        
        public void PoolableExecution(Poolable p)
        {
            _poolable = p;
        }

        public void Recycle()
        {
            speed = 1;
            damage = 1;
            RemoveFromScene();
        }

        private void RemoveFromScene()
        {
            if (_poolable)
                _poolable.Recycle();
            else
                Destroy(gameObject);
        }
    }
}
