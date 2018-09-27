using UnityEngine;

namespace WrappingRope.Demo
{
    public class TargetCamera : MonoBehaviour
    {
        public Transform target;

        void Update()
        {
            transform.LookAt(target);
        }
    }
}