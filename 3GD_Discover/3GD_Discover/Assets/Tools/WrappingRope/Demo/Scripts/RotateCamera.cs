using UnityEngine;
using System.Collections;

public class RotateCamera : MonoBehaviour {

    public Transform target; 
    public float distance = 5.0f; 

    private Vector2 _startPosition;

    void Start()
    {
        var angles = transform.eulerAngles;
        _startPosition.x = angles.y;
        _startPosition.y = angles.x;

    }

    void LateUpdate()
    { 
        if (target)
        {
            var currentPosition = MovementManager.Instance.CurrentPosition;
            var rotation = Quaternion.Euler(currentPosition.y + _startPosition.y, 0, currentPosition.x + _startPosition.x);
            transform.rotation = rotation;

            var position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;
            transform.position = position;
        }
    }

    void FixedUpdate()
    {
    }
}
