using System.Collections.Generic;
using UnityEngine;

namespace WrappingRope.Demo
{
    public class Man1Controller : MonoBehaviour
    {
        public float stepSpeed = 1f;
        public float rotateSpeed = 1f;

        private CharacterController _controller;
        private Animator _anim;

        private float g = 10.0f;
        private float jf = 3.0f;
        private float vv;

        private List<Vector3> _externalForces;

        // Use this for initialization
        void Start()
        {
            _controller = GetComponent<CharacterController>();
            _anim = GetComponent<Animator>();
            _externalForces = new List<Vector3>();
        }


        // Update is called once per frame
        void Update()
        {
            var moveDirection = new Vector3(0, 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= stepSpeed;
            transform.Rotate(Vector3.down * Input.GetAxis("Horizontal") * (-1), Space.World);
            if (_controller.isGrounded)
            {
                vv = -g * Time.deltaTime;
            }
            else
            {
                vv -= g * Time.deltaTime;
            };
            moveDirection += GetSuperpositionOfExternalForces();
            _controller.Move((moveDirection + new Vector3(0, vv, 0)) * Time.deltaTime);
            if (_controller.isGrounded && Input.GetKeyDown(KeyCode.Space))
            {
                vv = jf;
            }
            _controller.Move((new Vector3(0, vv, 0)) * Time.deltaTime);
            _externalForces.Clear();
        }

        void FixedUpdate()
        {


            if (Input.GetAxis("Vertical") > 0)
            {
                _anim.SetFloat("IdleWalk", 1, 0.1f, Time.deltaTime);

            }
            else
                _anim.SetFloat("IdleWalk", -1, 0.1f, Time.deltaTime);

            if (Input.GetAxis("Horizontal") > 0)
            {
                _anim.SetFloat("Rotate", 1, 0.1f, Time.deltaTime);

            }
            else if (Input.GetAxis("Horizontal") < 0)
                _anim.SetFloat("Rotate", -1, 0.1f, Time.deltaTime);
            else if (Input.GetAxis("Horizontal") == 0)
                _anim.SetFloat("Rotate", 0, 0.1f, Time.deltaTime);
        }


        private Vector3 GetSuperpositionOfExternalForces()
        {
            var superForce = Vector3.zero;
            foreach (var force in _externalForces)
            {
                superForce += force;
            }
            return superForce;
        }

        public void AddForce(Vector3 force)
        {
            _externalForces.Add(force);
        }

    }
}