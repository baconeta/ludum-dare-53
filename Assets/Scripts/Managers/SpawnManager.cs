using ObjectPooling;
using Spawnables;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public sealed class SpawnManager : MonoBehaviour
    {
        [SerializeField][Min(5)] private int simultaneousObstacleLimit = 20;
        [SerializeField][Min(5)] private float spawnInterval = 5.0f;
        [Tooltip("The obstacle that is be spawned in the game scene.")]
        [SerializeField] private GameObject obstacleObject;
        
        [SerializeField] private float speedMultiplier = 1.0f;
        [SerializeField] private float damageMultiplier = 1.0f;

        private ObjectPool _obstacles;

        private float _topLimit;
        private float _leftLimit;
        private float _rightLimit;
        private const float ShoreBuffer = 10.0f;

        private void Awake() {
            _obstacles = ObjectPool.Build(obstacleObject, simultaneousObstacleLimit, simultaneousObstacleLimit);
        }

        private void Start() {
            // Get the visual limits of the game scene
            _topLimit = GameObject.Find("TopLimit").transform.position.y;
            _leftLimit = GameObject.Find("Left Shore").transform.position.x + ShoreBuffer;
            _rightLimit = GameObject.Find("Right Shore").transform.position.x - ShoreBuffer;

            // Start spawning in response to game state changes
            GameStateManager.OnFerryingEnter += StartSpawn;

            // Stop spawning while dialogue is being displayed
            GameStateManager.OnDialogueEnter += StopSpawn;

            // Pause spawning in response to game state changes
            GameStateManager.OnPauseEnter += StopSpawn;

            // Stop spawning in response to game over or restarting the game
            GameStateManager.OnEndEnter += StopSpawn;
        }

        private void OnDestroy() {
            // Deregister all of the delegates
            GameStateManager.OnFerryingEnter -= StartSpawn;
            GameStateManager.OnDialogueEnter -= StopSpawn;
            GameStateManager.OnPauseEnter -= StopSpawn;
            GameStateManager.OnEndEnter -= StopSpawn;
        }

        /**
         * Start spawning objects at set interval.
         */
        private void StartSpawn()
        {
            InvokeRepeating(nameof(Spawn), 0.0f, spawnInterval);
        }

        /**
         * Spawn an individual object at a random starting position at the top of the screen.
         */
        private void Spawn()
        {
            var obstacle = _obstacles.GetRecyclable();

            var length = obstacle.GetComponent<Renderer>().bounds.size.y;
            var topOffset = _topLimit + length / 2.0f;

            obstacle.transform.position = new Vector3(Random.Range(_leftLimit, _rightLimit), topOffset, 0.0f);

            obstacle.GetComponent<Obstacle>().MultiplySpeed(speedMultiplier);
            obstacle.GetComponent<Obstacle>().MultiplyDamage(damageMultiplier);
        }

        /**
         * Stop spawning new objects.
         */
        private void StopSpawn()
        {
            CancelInvoke(nameof(Spawn));
        }

        /**
         * Increase the speed of the spawned objects.
         */
        public void IncreaseSpeed(float multiplier)
        {
            speedMultiplier = multiplier;
        }

        /**
         * Increase frequency of the spawned objects.
         */
        public void IncreaseFrequency(float multiplier)
        {
            spawnInterval = multiplier;
        }

        /**
         * Increase the damage of the spawned objects.
         */
        public void IncreaseDamage(float multiplier)
        {
            damageMultiplier = multiplier;
        }
    }
}
