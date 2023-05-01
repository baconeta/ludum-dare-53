using UnityEngine;

namespace Spawnables {
    public class Yeet : MonoBehaviour {
        private GameObject _target;
        private Vector3 _end;
        private bool _triggered;

        private void Start() {
            _target = GameObject.FindWithTag("Ferry");
        }

        // Update is called once per frame
        private void Update() {
            if (!_triggered) return;
            
            // Move the object towards the target
            transform.position = Vector3.MoveTowards(transform.position, _end, 0.1f);
        }

        public void Yeeteth() {
            _triggered = true;
            // Calculate end once based on where the target is
            _end = _target.transform.position;
        }
    }
}
