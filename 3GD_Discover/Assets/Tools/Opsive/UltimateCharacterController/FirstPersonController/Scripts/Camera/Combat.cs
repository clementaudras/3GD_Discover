/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

using UnityEngine;

namespace Opsive.UltimateCharacterController.FirstPersonController.Camera.ViewTypes
{
    /// <summary>
    /// The Combat ViewType a first person prespective that allows the camera and character to rotate together.
    /// </summary>
    [UltimateCharacterController.Camera.ViewTypes.RecommendedMovementType(typeof(Character.MovementTypes.Combat))]
    public class Combat : FirstPerson
    {
        /// <summary>
        /// Rotates the camera according to the horizontal and vertical movement values.
        /// </summary>
        /// <param name="horizontalMovement">-1 to 1 value specifying the amount of horizontal movement.</param>
        /// <param name="verticalMovement">-1 to 1 value specifying the amount of vertical movement.</param>
        /// <param name="immediatePosition">Should the camera be positioned immediately?</param>
        /// <returns>The updated rotation.</returns>
        public override Quaternion Rotate(float horizontalMovement, float verticalMovement, bool immediatePosition)
        {
            m_Yaw += horizontalMovement;

            return base.Rotate(horizontalMovement, verticalMovement, immediatePosition);
        }
    }
}