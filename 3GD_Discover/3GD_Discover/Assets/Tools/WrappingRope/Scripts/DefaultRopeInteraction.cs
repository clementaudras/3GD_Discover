using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace WrappingRopeLibrary.Customization
{
    [ExecuteInEditMode]
    public class DefaultRopeInteraction : MonoBehaviour, IRopeInteraction
    {

        private Quaternion _prevRotation;
        private Vector3 _prevPosition;

        void Start()
        {
            _prevRotation = transform.rotation;
            _prevPosition = transform.position;
        }


        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                _prevPosition = transform.position;
                _prevRotation = transform.rotation;
            }
#endif

        }


        void FixedUpdate()
        {
            _prevPosition = transform.position;
            _prevRotation = transform.rotation;
        }

        
        public Vector3 GetPointVelocity(Vector3 point)
        {
            var veloc = new Vector3();
            var difRotation = transform.rotation * Quaternion.Inverse(_prevRotation);
            var origin = transform.position - point;
            var prevOrigin = difRotation * origin;
            veloc = (origin - prevOrigin + transform.position - _prevPosition) / Time.fixedDeltaTime;
            return veloc;
        }

        public virtual void AddForceAtPosition(Vector3 force, Vector3 position, ForceMode mode)
        {
            // Not implemented
        }
    }
}
