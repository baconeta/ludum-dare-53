using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Spawnables
{
    public class StoneObstacle : Obstacle
    {
        public float distanceBetweenRocks;
        protected override void Start()
        {
            initialSpeed = 0;
            base.Start();
        }
    }
}
