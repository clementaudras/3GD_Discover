using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Opsive.UltimateCharacterController.Character.Abilities;
using Opsive.UltimateCharacterController.Character;

public class ForceJump : MonoBehaviour
{
    public UltimateCharacterLocomotion m_uclScript;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_uclScript.GetAbility<SpeedChange>().Enabled = false;
        }
        else
        {
            StartCoroutine(ResetSpeedChange());
        }

    }

    public IEnumerator ResetSpeedChange()
    {
        yield return new WaitForSeconds(0.5f);
        m_uclScript.GetAbility<SpeedChange>().Enabled = true;
    }
}
