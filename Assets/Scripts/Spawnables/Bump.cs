using UnityEngine;

namespace Spawnables
{
    /**
     * Bump objects out of the way when they get stuck on static obstacles!
     */
    public class Bump : MonoBehaviour
    {
        [SerializeField] private float bumpForce = 2f;
        
        private void OnCollisionStay2D(Collision2D collision) {
            if (!collision.gameObject.CompareTag("Obstacle")) return;
            
            // Bump the other object away
            var bumpDirection = (collision.transform.position - transform.position).normalized;
            bumpDirection = new Vector3(bumpDirection.x, 0);

            collision.gameObject.transform.position += bumpDirection * Time.deltaTime * bumpForce;
        }
    }
}
