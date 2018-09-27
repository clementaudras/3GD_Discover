using UnityEngine;
using System.Collections;

public class MouseCamera : MonoBehaviour {


	// Update is called once per frame
	void Update () {
    }


    //public Transform target;
    public float distance = 5.0f;

    private Vector2 _startPosition;
    private Vector2 _currentPosition = Vector2.zero;

    void Start()
    {
        var angles = transform.eulerAngles;
        _startPosition.x = angles.y;
        _startPosition.y = angles.x;

    }

    void LateUpdate()
    {
        _currentPosition.x = _currentPosition.x + Input.GetAxis("Mouse X") * 1;
        _currentPosition.y = _currentPosition.y - Input.GetAxis("Mouse Y") * 1;




        var currentPosition = _currentPosition;
        //var rotation = Quaternion.Euler(currentPosition.y + _startPosition.y, currentPosition.x + _startPosition.x, currentPosition.x + _startPosition.x);
        var rotation = Quaternion.Euler(currentPosition.y + _startPosition.y, currentPosition.x + _startPosition.x, 0);


        transform.rotation = rotation;

        //if (target)
        //{
        //    var currentPosition = MovementManager.Instance.CurrentPosition;
        //    var rotation = Quaternion.Euler(currentPosition.y + _startPosition.y, currentPosition.x + _startPosition.x, currentPosition.x + _startPosition.x);
        //    transform.rotation = rotation;

        //    //var position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;
        //    //transform.position = position;
        //}
    }

    void FixedUpdate()
    {
    }



}
