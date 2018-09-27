using UnityEngine;
using WrappingRopeLibrary.Customization;

namespace WrappingRope.Demo
{
    public class CharacterRopeInteraction : DefaultRopeInteraction
    {
        public Man1Controller _manController;
        public float _strengthOfForce;

        public override void AddForceAtPosition(Vector3 force, Vector3 position, ForceMode mode)
        {
            if (_manController != null)
            {
                // For simplify use only the value of force
                _manController.AddForce(force * _strengthOfForce);
            }
        }
    }
}