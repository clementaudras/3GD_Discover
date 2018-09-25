/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

using UnityEngine;

namespace Opsive.UltimateCharacterController.FirstPersonController.Character.Identifiers
{
    /// <summary>
    /// Identifier component which identifies the first person base object.
    /// </summary>
    public class FirstPersonBaseObject : MonoBehaviour
    {
        [Tooltip("The unique ID of the first person base object.")]
        [SerializeField] protected int m_ID;

        [Utility.NonSerialized] public int ID { get { return m_ID; } set { m_ID = value; } }
    }
}