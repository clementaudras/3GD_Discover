using UnityEngine;

public class MovementManager : Singleton <MovementManager>
{

    protected MovementManager()
    { }

    public const float XSPEED = .4f;
    public const float YSPEED = 0.4f;
    public const float MAXXDERIVATIVE = 2;
    public const float MAXYDERIVATIVE = 2;

    private Vector2 _currentPosition = Vector2.zero;


    public Vector2 CurrentPosition
    {
        get { return _currentPosition; }
    }

    void Start()
    {
    }


    void FixedUpdate()
    {
        _currentPosition.x = _currentPosition.x + Input.GetAxis("Mouse X") * XSPEED;
        _currentPosition.y = _currentPosition.y - Input.GetAxis("Mouse Y") * YSPEED;

        if (_currentPosition.x < MAXXDERIVATIVE * (-1))
            _currentPosition.x = MAXXDERIVATIVE * (-1);
        if (_currentPosition.x > MAXXDERIVATIVE)
            _currentPosition.x = MAXXDERIVATIVE;
        if (_currentPosition.y < MAXYDERIVATIVE * (-1))
            _currentPosition.y = MAXYDERIVATIVE * (-1);
        if (_currentPosition.y > MAXYDERIVATIVE)
            _currentPosition.y = MAXYDERIVATIVE;
    }
}
