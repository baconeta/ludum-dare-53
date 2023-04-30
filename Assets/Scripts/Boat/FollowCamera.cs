using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    private BoatController _boat;

    [SerializeField] private float followSpeed = 10f;

    [SerializeField] private Vector2 posOffset = new Vector2(0.5f, 0.5f);

    [SerializeField] private float leftLimit = -25f;
    [SerializeField] private float rightLimit = 25f;
    [SerializeField] private float bottomLimit = -25f;
    [SerializeField] private float topLimit = 25f;

    private float _height;
    private float _width;

    private void Start()
    {
        _boat = FindObjectOfType<BoatController>();
        Camera cam = Camera.main;
        if (cam != null)
        {
            _height = 2f * cam.orthographicSize;
            _width = _height * cam.aspect;
        }
    }

    private void FixedUpdate()
    {
        if (_boat != null)
        {
            Vector3 start = transform.position;
            Vector3 end = _boat.transform.position;

            end.x += posOffset.x;
            end.y += posOffset.y;
            end.z = transform.position.z;

            transform.position = Vector3.Lerp(start, end, followSpeed * Time.fixedDeltaTime);
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, leftLimit + _width / 2, rightLimit - _width / 2),
                Mathf.Clamp(transform.position.y, bottomLimit + _height / 2, topLimit - _height / 2),
                transform.position.z);
        }
        else
        {
            Debug.LogWarning("Boat was null");
        }
    }
}