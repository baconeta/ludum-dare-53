using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Spawnables
{
    public class StoneObstacle : Obstacle
    {
        public float distanceBetweenRocks;
        private void Start()
        {
            initialSpeed = 0;
            base.Start();
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, Mathf.Max(GetComponent<Renderer>().bounds.size.x,
                GetComponent<Renderer>().bounds.size.y));
        }
    }
}
