using CustomEditor;
using Managers;
using ObjectPooling;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Spawnables
{
    public class Obstacle : MonoBehaviour, IPoolableExecution
    {
        [SerializeField] protected float initialSpeed = 3.0f;
        [SerializeField] private int damage = 5;

        private float _bottomBound;
        private Poolable _poolable;
        private float _length;
        [ReadOnly] public float currentSpeed;

        protected void Start()
        {
            currentSpeed = initialSpeed;
            _bottomBound = GameObject.Find("BottomLimit").transform.position.y;
            _poolable = GetComponent<Poolable>();
            _length = GetComponent<Renderer>().bounds.size.y;

            SceneManager.activeSceneChanged += (_, _) => RemoveFromScene(); //Hack to force removal at end of game
        }

        private void Update()
        {
            if (!GameStateManager.Instance.IsGameActive()) return;

            // Slowly move the obstacle down the screen
            transform.position += Vector3.down * (currentSpeed * Time.deltaTime);
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
            currentSpeed *= multiplier;
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

        protected void RemoveFromScene()
        {
            currentSpeed = initialSpeed;
            damage = 1;
            transform.position = new Vector3(-1000, -1000);
            if (_poolable)
                _poolable.Recycle();
            else if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }
    }
}