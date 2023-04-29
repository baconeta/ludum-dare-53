using ObjectPooling;
using Spawnables;
using UnityEngine;
using Utils;

namespace Managers
{
    public sealed class SpawnManager : EverlastingSingleton<SpawnManager>
    {
        [SerializeField] [Min(5)] private int simultaneousObstacleLimit = 20;
        [Tooltip("The obstacle that is be spawned in the game scene.")]
        [SerializeField] private GameObject obstacleObject;
        
        private float _speedMultiplier = 1.0f;
        private float _damageMultiplier = 1.0f;

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
            _leftLimit = GameObject.Find("LeftLimit").transform.position.x;
            _rightLimit = GameObject.Find("RightLimit").transform.position.x;
            
            StartSpawn(1.0f, 1.0f, 1.0f);
        }

        /**
         * Spawn a random object from the list of spawnable objects.
         */
        public void StartSpawn(float spawnInterval, float damageMultiplier, float speedMultiplier)
        {
            _damageMultiplier = damageMultiplier;
            _speedMultiplier = speedMultiplier;
            
            InvokeRepeating(nameof(Spawn), 0.0f, spawnInterval);
        }

        private void Spawn()
        {
            var obstacle = _obstacles.GetRecyclable();
            obstacle.transform.position = new Vector3(Random.Range(_leftLimit, _rightLimit), _topLimit, 0.0f);
            
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
