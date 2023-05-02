using System.Collections;
using UnityEngine;

namespace Spawnables {
    public class Yeet : MonoBehaviour {
        private GameObject _target;
        public float speed;
        private Vector3 _direction;
        private Vector3 _end;
        private bool _triggered;
        public float disabledDuration;
        public CircleCollider2D _circleCollider2D;
        public Rigidbody2D _Rigidbody2D;

        private void Start() {
            _Rigidbody2D = GetComponent<Rigidbody2D>();
            _circleCollider2D = GetComponent<CircleCollider2D>();
            _target = GameObject.FindWithTag("Ferry");
            _direction = _target.transform.position - transform.position;
            _end = _target.transform.position;
        }

        // Update is called once per frame
        private void FixedUpdate() {
            if (!_triggered) return;
            
            // Move the object towards the target
            //_Rigidbody2D.MovePosition(transform.position + _direction.normalized * speed * Time.fixedDeltaTime);
        }

        private IEnumerator enableCollision()
        {
            yield return new WaitForSeconds(disabledDuration);
            _circleCollider2D.enabled = true;
            yield return null;
        }

        public void YeetethMySkull() {
            _triggered = true;
            StartCoroutine(enableCollision());

            if (!_Rigidbody2D) _Rigidbody2D = GetComponent<Rigidbody2D>();
            _Rigidbody2D.AddForce(_direction.normalized * speed, ForceMode2D.Impulse);
            // Calculate end once based on where the target is
        }

        
    }
}
