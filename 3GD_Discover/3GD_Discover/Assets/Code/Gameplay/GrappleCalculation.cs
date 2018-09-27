using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Opsive.UltimateCharacterController.Character.Abilities;
using Opsive.UltimateCharacterController.Character;

public class GrappleCalculation : MonoBehaviour {

    //public ObjectWeight m_objWeightScript;

    //public GameObject m_obiRopeRenderer;

    public float distToHookMax = 1;
    public GameObject hook;
    public GameObject hookHolder;


    public float hookTravelSpeed;
    public float hookRange = 10.0f;
    public float playerTravelSpeed;

    public bool hookFired;
    public bool hooked;
    public GameObject hookedObj;

    public GameObject grapplingLr;
    public GameObject hookHelper;

    public float maxDistance;
    private float currentDistance;
    public bool grounded;
    public bool ceilling;

    public Camera fpsCam;

    public UltimateCharacterLocomotion m_uclScript;

    // Use this for initialization
    void Start()
    {
        m_uclScript.GetAbility<Grapple>().Enabled = false;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        CheckIfCeilling();

        if (hookFired)
        {
            hook.transform.position = hookHelper.transform.position;
            hook.transform.parent = null;
            grapplingLr.GetComponent<LineRenderer>().enabled = true;
            //m_obiRopeRenderer.SetActive(true);
        }
        else
        {
            hook.transform.parent = hookHolder.transform;
            grapplingLr.GetComponent<LineRenderer>().enabled = false;
            //m_obiRopeRenderer.SetActive(false);
        }



        // firing the hook
        if (Input.GetKeyDown(KeyCode.Mouse1) && hookFired == false)
        {


            this.transform.Translate(Vector3.up * Time.deltaTime * 30);


            StartCoroutine("JumpShootGrappling");
        }

        if (hookFired == true && hooked == false)
        {
            currentDistance = Vector3.Distance(transform.position, hookHelper.transform.position);

            if (currentDistance >= maxDistance)
                ReturnHook();

        }

        if (hooked == true && hookFired == true)
        {
            hook.transform.parent = hookedObj.transform;
            m_uclScript.GetAbility<Grapple>().Enabled = true;
            m_uclScript.GetAbility<Fall>().Enabled = false;
            m_uclScript.StickToGround = false;

            //transform.position = Vector3.MoveTowards(transform.position, hookHelper.transform.position, Time.deltaTime * playerTravelSpeed);
            float distanceToHook = Vector3.Distance(transform.position, hookHelper.transform.position);

            if (distanceToHook < distToHookMax)
            {
                if (grounded == false)
                {
                    //this.transform.Translate(Vector3.forward * Time.deltaTime * 15f);
                    //this.transform.Translate(Vector3.up * Time.deltaTime * 20f);
                }
                StartCoroutine("Climb");
            }
        }
        else
        {

        }
    }

    public IEnumerator Climb()
    {
        yield return new WaitForSeconds(0.1f);
        ReturnHook();
    }

    IEnumerator JumpShootGrappling()
    {
        yield return new WaitForSeconds(0.1f);
        ShotGrappling();
    }

    void ShotGrappling()
    {
        hookFired = true;

        // Create a vector at the center of our camera's viewport
        Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.2f));

        // Declare a raycast hit to store information about what our raycast has hit
        RaycastHit hit;

        if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, hookRange))
        {
            hookHelper.transform.position = hit.point;

            //If raycast hit a hookable surface, the hook is hooked
            if (hit.transform.tag == "Hookable")
            {
                hooked = true;
                //Debug.Log("Grappling hooked to " + hit.transform.name);
                hookedObj = hit.transform.gameObject;
            }
            else
            {
                hooked = false;
                hookHelper.transform.position += fpsCam.transform.forward * hookRange;
                StartCoroutine("Climb");
            }
        }
        else
        {
            StartCoroutine("Climb");
        }
    }

    public void ReturnHook()
    {
        hook.transform.rotation = hookHolder.transform.rotation;
        hook.transform.position = hookHolder.transform.position;
        hookFired = false;
        hooked = false;
        m_uclScript.GetAbility<Grapple>().Enabled = false;
        m_uclScript.GetAbility<Fall>().Enabled = true;
        m_uclScript.StickToGround = true;
    }

    void CheckIfGrounded()
    {
        RaycastHit hit;
        float distance = 1f;
        Vector3 dir = new Vector3(0, -1);
        Debug.DrawRay(transform.position, dir, Color.magenta);

        if (Physics.Raycast(transform.position, dir, out hit, distance))
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }
    }

    void CheckIfCeilling()
    {
        RaycastHit hit;
        float distance = 1f;
        Vector3 dir = new Vector3(0, 2.0f, 0);
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 2.0f, transform.position.z);
        Debug.DrawRay(pos, dir, Color.magenta);

        if (Physics.Raycast(pos, dir, out hit, distance))
        {
            if (hit.transform.CompareTag("Hookable"))
            {
                ceilling = true;
            }
            else
            {
                ceilling = false;
            }
        }
        else
        {
            ceilling = false;
        }
    }
}

