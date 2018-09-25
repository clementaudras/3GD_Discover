/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

using UnityEngine;
using Opsive.UltimateCharacterController.Camera.ViewTypes;

namespace Opsive.UltimateCharacterController.FirstPersonController.Camera.ViewTypes
{
    /// <summary>
    /// The TransformLook ViewType is a first person view type that will have the camera look in the forward direction relative to the target transform.
    /// </summary>
    public class TransformLook : ViewType
    {
        [Tooltip("The object that determines the position of the camera.")]
        [SerializeField] protected Transform m_MoveTarget;
        [Tooltip("The object that determines the rotation of the camera.")]
        [SerializeField] protected Transform m_RotationTarget;
        [Tooltip("The offset relative to the move target.")]
        [SerializeField] protected Vector3 m_Offset = new Vector3(0, 0.2f, 0.2f);
        [Tooltip("The speed at which the camera should move.")]
        [SerializeField] protected float m_MoveSpeed = 10;
        [Tooltip("The speed at which the view type should rotate towards the target rotation.")]
        [Range(0, 1)] [SerializeField] protected float m_RotationalLerpSpeed = 0.9f;

        public Vector3 Offset { get { return m_Offset; } set { m_Offset = value; } }
        public float MoveSpeed { get { return m_MoveSpeed; } set { m_MoveSpeed = value; } }
        public float RotationalLerpSpeed { get { return m_RotationalLerpSpeed; } set { m_RotationalLerpSpeed = value; } }

        public override Quaternion CharacterRotation { get { return m_CharacterTransform.rotation; } }
        public override bool FirstPersonPerspective { get { return true; } }
        public override float LookDirectionDistance { get { return m_Offset.magnitude; } }
        public override float Pitch { get { return 0; } }
        public override float Yaw { get { return 0; } }

        /// <summary>
        /// Rotates the camera to face in the same direction as the target.
        /// </summary>
        /// <param name="horizontalMovement">-1 to 1 value specifying the amount of horizontal movement.</param>
        /// <param name="verticalMovement">-1 to 1 value specifying the amount of vertical movement.</param>
        /// <param name="immediateUpdate">Should the camera be updated immediately?</param>
        /// <returns>The updated rotation.</returns>
        public override Quaternion Rotate(float horizontalMovement, float verticalMovement, bool immediateUpdate)
        {
            var rotation = m_RotationTarget.rotation;
            if (!immediateUpdate) {
                rotation = Quaternion.Slerp(m_Transform.rotation, rotation, m_RotationalLerpSpeed);
            }
            return rotation;
        }

        /// <summary>
        /// Moves the camera to be in the target position.
        /// </summary>
        /// <param name="immediateUpdate">Should the camera be updated immediately?</param>
        /// <returns>The updated position.</returns>
        public override Vector3 Move(bool immediateUpdate)
        {
            return Vector3.MoveTowards(m_Transform.position, m_MoveTarget.TransformPoint(m_Offset), immediateUpdate ? float.MaxValue : Time.fixedDeltaTime * m_MoveSpeed);
        }

        /// <summary>
        /// Returns the direction that the character is looking.
        /// </summary>
        /// <param name="characterLookDirection">Is the character look direction being retrieved?</param>
        /// <returns>The direction that the character is looking.</returns>
        public override Vector3 LookDirection(bool characterLookDirection)
        {
            return m_CharacterTransform.forward;
        }

        /// <summary>
        /// Returns the direction that the character is looking.
        /// </summary>
        /// <param name="lookPosition">The position that the character is looking from.</param>
        /// <param name="characterLookDirection">Is the character look direction being retrieved?</param>
        /// <param name="layerMask">The LayerMask value of the objects that the look direction can hit.</param>
        /// <param name="useRecoil">Should recoil be included in the look direction?</param>
        /// <returns>The direction that the character is looking.</returns>
        public override Vector3 LookDirection(Vector3 lookPosition, bool characterLookDirection, int layerMask, bool useRecoil)
        {
            return m_CharacterTransform.forward;
        }
    }
}