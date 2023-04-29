using ObjectPooling;
using Spawnables;
using UnityEngine;
using Utils;

namespace Managers
{
    public sealed class SpawnManager : EverlastingSingleton<SpawnManager>
    {
        [SerializeField][Min(5)] private int simultaneousObstacleLimit = 20;
        [SerializeField][Min(5)] private float spawnInterval = 3f;
        [Tooltip("The obstacle that is be spawned in the game scene.")]
        [SerializeField] private GameObject obstacleObject;
        
        [SerializeField] private float _speedMultiplier = 1f;
        [SerializeField] private float _damageMultiplier = 1f;

        private ObjectPool _obstacles;

        private float _topLimit;
        private float _leftLimit;
        private float _rightLimit;

        protected override void Awake()
        {
            base.Awake();
            // Setup object pooling for obstacles
            _obstacles = ObjectPool.Build(obstacleObject, simultaneousObstacleLimit, simultaneousObstacleLimit);
        }

        private void Start()
        {
            _topLimit = GameObject.Find("TopLimit").transform.position.y;
            _leftLimit = GameObject.Find("Left Shore").transform.position.x;
            _rightLimit = GameObject.Find("Right Shore").transform.position.x;

            StartSpawn(spawnInterval, _damageMultiplier, _speedMultiplier);
        }

        /**
         * Spawn a random object from the list of spawnable objects.
         */
        public void StartSpawn(float interval, float damageMultiplier, float speedMultiplier)
        {
            _damageMultiplier = damageMultiplier;
            _speedMultiplier = speedMultiplier;

            InvokeRepeating(nameof(Spawn), 0.0f, interval);
        }

        private void Spawn()
        {
            if (!GameStateManager.Instance.IsGameActive()) return; //TODO: This will mean an immediate spawn after pausing as the timer keeps running.

            var obstacle = _obstacles.GetRecyclable();

            var length = obstacle.GetComponent<Renderer>().bounds.size.y;
            var topOffset = _topLimit + (length / 2);

            obstacle.transform.position = new Vector3(Random.Range(_leftLimit, _rightLimit), topOffset, 0.0f);

            obstacle.GetComponent<Obstacle>().MultiplySpeed(_speedMultiplier);
            obstacle.GetComponent<Obstacle>().MultiplyDamage(_damageMultiplier);
        }

        /**
         * Stop spawning objects.
         */
        public void StopSpawn()
        {
            CancelInvoke(nameof(Spawn));
        }
    }
}
