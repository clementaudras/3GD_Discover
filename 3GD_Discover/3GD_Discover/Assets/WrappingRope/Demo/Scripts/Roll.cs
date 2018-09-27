using UnityEngine;
using System.Collections;

public class Roll : MonoBehaviour {

    private Rigidbody rb;

    public RollDirection Direction;

    private float velocity;

    public bool Invert;
    void Start()
    {
        velocity = 1f;
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
    }


    public void FixedUpdate()
    {
        float inv = 1;
        if (Invert)
            inv = -1;

        switch (Direction)
        {
            case RollDirection.X:
                rb.AddRelativeTorque(new Vector3(velocity * inv, 0, 0), ForceMode.Force);
                break;
            case RollDirection.Y:
                rb.AddRelativeTorque(new Vector3(0, velocity * inv, 0), ForceMode.Force);
                break;
            case RollDirection.Z:
                rb.AddRelativeTorque(new Vector3(0, 0, velocity * inv), ForceMode.Force);
                break;

        }
    }

    public enum RollDirection
    {
        X,Y,Z
    }
}
