using ObjectPooling;
using Spawnables;
using System;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public sealed class SpawnManager : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField][Min(5)] private int simultaneousObstacleLimit = 20;
        [SerializeField][Min(5)] private int simultaneousSeekerLimit = 20;
        [SerializeField][Min(1)] private float spawnInterval = 5.0f;
        
        [Tooltip("The obstacle that is be spawned in the game scene.")]
        [SerializeField] private GameObject obstacleObject;
        [Tooltip("Creepy seeker that follows the player.")]
        [SerializeField] private GameObject seekerObject;
        [Tooltip("Percentage of the time that a seeker will spawn instead of an obstacle.")]
        [SerializeField][Range(0, 100)] private int seekerSpawnChance = 20;

        [Tooltip("The maximum number of stones to spawn randomly in the river upon loading the game.")]
        [SerializeField] [Range(1, 5)] private int stoneLimit = 3;
        [Tooltip("Stones cannot spawn on top of each other. Overlaps check for spriteSize * value")]
        [Min(1)]
        [SerializeField] private float stoneOverlapRangeMultiplier = 1.0f;
        [SerializeField] private GameObject[] stones;
        
        [Header("Spawn Multipliers")]
        [SerializeField] private float speedMultiplier = 1.0f;
        [SerializeField] private float damageMultiplier = 1.0f;

        private ObjectPool _obstacles;
        private ObjectPool _seekers;

        // Spawn limits
        private float _topLimit;
        private float _bottomLimit;
        private float _leftLimit;
        private float _rightLimit;
        private const float ShoreBuffer = 10.0f;
        private const float VerticalBuffer = 10.0f;

        private bool _isSpawning;

        private void Awake()
        {
            _obstacles = ObjectPool.Build(obstacleObject, simultaneousObstacleLimit, simultaneousObstacleLimit);
            _seekers = ObjectPool.Build(seekerObject, simultaneousSeekerLimit, simultaneousSeekerLimit);
        }

        private void Start()
        {
            // Get the visual limits of the game scene
            _topLimit   = GameObject.Find("TopLimit").transform.position.y - VerticalBuffer;
            _bottomLimit   = GameObject.Find("BottomLimit").transform.position.y + VerticalBuffer;
            _leftLimit  = GameObject.Find("Left Shore").transform.position.x + ShoreBuffer;
            _rightLimit = GameObject.Find("Right Shore").transform.position.x - ShoreBuffer;
            
            // Add a random number of stones to the scene
            SpawnStonesRandomly();

            // Start spawning in response to game state changes
            GameStateManager.OnFerryingEnter += StartSpawn;

            // Stop spawning while dialogue is being displayed
            GameStateManager.OnDialogueEnter += StopSpawn;

            // Pause spawning in response to game state changes
            GameStateManager.OnPauseEnter += StopSpawn;
            GameStateManager.OnPauseExit += StartSpawn;

            // Stop spawning in response to game over or restarting the game
            GameStateManager.OnEndEnter += StopSpawn;
        }

        private void OnDestroy()
        {
            // Deregister all of the delegates
            GameStateManager.OnFerryingEnter -= StartSpawn;
            GameStateManager.OnDialogueEnter -= StopSpawn;
            GameStateManager.OnPauseEnter -= StopSpawn;
            GameStateManager.OnPauseExit -= StartSpawn;
            GameStateManager.OnEndEnter -= StopSpawn;
        }

        private void SpawnStonesRandomly()
        {
            var stoneSpawnArea    = GameObject.Find("StoneSpawnArea").GetComponent<Renderer>().bounds;
            var stoneNoSpawnArea  = GameObject.Find("StoneNoSpawnArea").GetComponent<Renderer>().bounds;
            var numStones = Random.Range(1, stoneLimit + 1);
            for (var i = 0; i < numStones; ++i)
            {

                GameObject stone;
                //Always spawn 1 thrower
                if (i == 0) stone = stones[0];
                //Other stones are not throwers
                else stone = stones[Random.Range(1, stones.Length)];
                var stoneObject = Instantiate(stone);
                
                // Select a random point within the spawn area that avoids the no-spawn area
                var stoneSpawnPoint = new Vector3(
                    Random.Range(stoneSpawnArea.min.x, stoneSpawnArea.max.x),
                    Random.Range(stoneSpawnArea.min.y, stoneSpawnArea.max.y),
                    0.0f);

                //largest of spriteSize X or Y
                float overlapRadius = Mathf.Max(stoneObject.GetComponent<Renderer>().bounds.size.x,
                    stoneObject.GetComponent<Renderer>().bounds.size.y);
                
                while (stoneNoSpawnArea.Contains(stoneSpawnPoint) ||
                       !IsOverlapping(overlapRadius * stoneOverlapRangeMultiplier, stoneSpawnPoint))
                {
                    stoneSpawnPoint = new Vector3(
                        Random.Range(stoneSpawnArea.min.x, stoneSpawnArea.max.x),
                        Random.Range(stoneSpawnArea.min.y, stoneSpawnArea.max.y),
                        0.0f);
                }
                
                stoneObject.transform.position = stoneSpawnPoint;
            }    
        }

        /**
         * Returns whether or not the given position is overlapping with another object with the given radius.
         * Works best if the object you're trying to spawn has not yet been given a position in the scene.
         */
        private static bool IsOverlapping(float radius, Vector3 position)
        {
            var c = Physics2D.OverlapCircle(position, radius);
            return c is not null;
        }

        /**
         * Start spawning objects at set interval.
         */
        private void StartSpawn()
        {
            if (_isSpawning) return;

            _isSpawning = true;
            InvokeRepeating(nameof(Spawn), 0.0f, spawnInterval);
        }

        /**
         * Spawn an individual object at a random starting position at the top of the screen.
         */
        private void Spawn()
        {
            // TODO This is fragile! What happens if I pop the last object from the pool?
            var spawnable = Random.Range(0, 100) > seekerSpawnChance
                ? _obstacles.GetRecyclable()
                : _seekers.GetRecyclable();

            var length = spawnable.GetComponent<Renderer>().bounds.size.y;
            var topOffset = _topLimit + length / 2.0f;
            
            // Set the object's position to a random position at the top of the screen and make sure it isn't overlapping
            // with another object
            Vector3 position;
            do
            {
              position = new Vector3(Random.Range(_leftLimit, _rightLimit), topOffset, 0.0f);
            } while (IsOverlapping(spawnable.GetComponent<Renderer>().bounds.size.x, position));
            
            spawnable.transform.position = position;
            
            var obj = spawnable.GetComponent<Obstacle>();
            
            obj.MultiplySpeed(speedMultiplier);
            obj.MultiplyDamage(damageMultiplier);
        }

        /**
         * Stop spawning new objects.
         */
        private void StopSpawn()
        {
            if (!_isSpawning) return;

            _isSpawning = false;
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
