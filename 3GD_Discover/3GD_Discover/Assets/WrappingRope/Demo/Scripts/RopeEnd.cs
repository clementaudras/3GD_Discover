using UnityEngine;

public class RopeEnd : MonoBehaviour {

    // Use this for initialization
    private Rigidbody _rigidBody;


    void Start () {
         _rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update () {
    }


    void FixedUpdate()
    {
        if (_rigidBody)
        {
            var currentPosition = MovementManager.Instance.CurrentPosition;
            _rigidBody.AddForce(new Vector3(currentPosition.x * 4, 0, currentPosition.y * -4));

        }
    }
}
