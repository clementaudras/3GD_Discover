/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

#if UNITY_WEBGL || UNITY_IOS || UNITY_ANDROID || UNITY_WII || UNITY_PS3 || UNITY_PS4 || UNITY_XBOXONE || UNITY_WSA
using UnityEngine;
using Opsive.UltimateCharacterController.StateSystem;

namespace Opsive.UltimateCharacterController.ThirdPersonController.StateSystem
{
    // See Opsive.UltimateCharacterController.StateSystem.AOTLinker for an explanation of this class.
    public class AOTLinker : MonoBehaviour
    {
        public void Linker()
        {
#pragma warning disable 0219
#if THIRD_PERSON_CONTROLLER
            var genericObjectDeathVisiblityDelegate = new Preset.GenericDelegate<Character.PerspectiveMonitor.ObjectDeathVisiblity>();
#endif
#pragma warning restore 0219
        }
    }
}
#endif