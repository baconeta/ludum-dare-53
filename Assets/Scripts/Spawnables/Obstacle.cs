using CustomEditor;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Spawnables
{
    public class Obstacle : MonoBehaviour
    {
        [SerializeField] protected float initialSpeed = 3.0f;
        [SerializeField] private int damage = 5;

        private float _bottomBound;
        private float _length;
        [ReadOnly] public float currentSpeed;
        protected bool active;

        protected virtual void Start()
        {
            currentSpeed = initialSpeed;
            _bottomBound = GameObject.Find("BottomLimit").transform.position.y;
            _length = GetComponent<Renderer>().bounds.size.y;
            active = true;
        }

        protected virtual void OnEnable()
        {
            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged; //Hack to force removal at end of game
        }

        protected virtual void OnDisable()
        {
            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged; //Hack to force removal at end of game
        }

        private void Update()
        {
            if (!GameStateManager.Instance.IsGameActive() || !active) return;

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


        protected void RemoveFromScene()
        {
            currentSpeed = initialSpeed;
            damage = 1;
            transform.position = new Vector3(-1000, -1000);
            active = false;
            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }
        private void SceneManagerOnActiveSceneChanged(Scene scene, Scene scene1)
        {
            RemoveFromScene();
        }
        
    }
}