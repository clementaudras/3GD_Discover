using UnityEngine;
using Opsive.UltimateCharacterController.Events;
using Opsive.UltimateCharacterController.Game;
using Opsive.UltimateCharacterController.Input;
using Opsive.UltimateCharacterController.SurfaceSystem;
using Opsive.UltimateCharacterController.Utility;
using Opsive.UltimateCharacterController.Camera;

namespace Opsive.UltimateCharacterController.Character.Abilities
{
    /// <summary>
    /// The Grappling ability allows the character to move toward a target point into the air. Grapple is only active when the character has pressed a button.
    /// </summary>
    [DefaultInputName("Grapple")]
    [DefaultStartType(AbilityStartType.ButtonDown)]
    [DefaultStopType(AbilityStopType.Automatic)]
    [DefaultAbilityIndex(1)]
    [DefaultUseRootMotionPosition(AbilityBoolOverride.False)]
    [DefaultUseRootMotionRotation(AbilityBoolOverride.False)]

    public class Grapple : Ability
    {
        public float distToHookMax = 1;
        public Transform m_targetTr;
        public Transform m_playerTr;
        public GameObject hookHelper;

        public float m_speed = 20.0f;

        public GrappleCalculation m_grappleCalculation;
        /// <summary>
        /// Initialize the default values.
        /// </summary>
        public override void Awake()
        {
            base.Awake();
            m_playerTr = m_GameObject.GetCachedComponent<Transform>();

        }



        protected override void AbilityStarted()
        {
            Debug.Log("Grapple");
            ShootGrapple();

            base.AbilityStarted();
        }

        public override void UpdatePosition()
        {
            //m_playerTr.position = Vector3.MoveTowards(m_playerTr.position, _targetTr.transform.position, Time.deltaTime * _speed);
            ShootGrapple();

        }


        private void ShootGrapple()
        {
            m_playerTr.position = Vector3.MoveTowards(m_playerTr.position, m_targetTr.transform.position, Time.deltaTime * m_speed);
            float distanceToHook = Vector3.Distance(m_playerTr.position, m_targetTr.transform.position);

            if (distanceToHook < distToHookMax)
            {
                if (m_grappleCalculation.GetComponent<GrappleCalculation>().grounded == false)
                {
                    //AddForce(new Vector3(0.0f, 1.0f, 1.0f));
                    //AddForce(Vector3.forward *2f);
                    AddForce(Vector3.up*2f);
                    //m_playerTr.Translate(Vector3.forward * Time.deltaTime * 15f);
                    //m_playerTr.Translate(Vector3.up * Time.deltaTime * 20f);
                }
                m_grappleCalculation.GetComponent<GrappleCalculation>().StartCoroutine("Climb");
            }
        }
    }
}