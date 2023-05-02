using System.Collections;
using UnityEngine;

namespace Spawnables {
    public class Yeet : MonoBehaviour {
        private GameObject _target;
        private Vector3 _direction;
        private Vector3 _end;
        private bool _triggered;
        public float disabledDuration;
        private CircleCollider2D _circleCollider2D;

        private void Start() {
            _circleCollider2D = GetComponent<CircleCollider2D>();
            _target = GameObject.FindWithTag("Ferry");
            _direction = _target.transform.position - transform.position;
            _end = _target.transform.position;
        }

        // Update is called once per frame
        private void Update() {
            if (!_triggered) return;
            
            // Move the object towards the target
            transform.position = Vector3.MoveTowards(_direction, _end, 0.1f);
        }

        private IEnumerator enableCollision()
        {
            yield return new WaitForSeconds(disabledDuration);
            _circleCollider2D.enabled = true;
            yield return null;
        }

        public void Yeeteth() {
            _triggered = true;
            StartCoroutine(enableCollision());
            // Calculate end once based on where the target is
        }

        
    }
}
