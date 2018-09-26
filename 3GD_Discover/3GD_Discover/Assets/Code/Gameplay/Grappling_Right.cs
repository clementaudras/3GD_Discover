using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Opsive.UltimateCharacterController.Events;
using Opsive.UltimateCharacterController.Game;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Character.Abilities;
using Opsive.UltimateCharacterController.Character.Abilities.Items;
using Opsive.UltimateCharacterController.Character.Effects;
using Opsive.UltimateCharacterController.StateSystem;
using Opsive.UltimateCharacterController.Utility;
using Opsive.UltimateCharacterController.FirstPersonController.Character.MovementTypes;

public class Grappling_Right : MonoBehaviour {

    public float distToHookMax = 1;
    public GameObject hook;
    public GameObject hookHolder;

    public float hookTravelSpeed;
    public float hookRange = 10.0f;
    public float playerTravelSpeed;

    public bool hookFired;
    public bool hooked;
    public bool grabbed;
    public GameObject hookedObj;
    public GameObject grabbedObj;

    //public GameObject grapplingLr;
    public GameObject hookHelper;

    public float maxDistance;
    private float currentDistance;
    private bool grounded;
    public Camera fpsCam;
    public bool a;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Update is called once per frame
    void FixedUpdate()
    {
        

        //Debug.Log(FPCtrl.Velocity.magnitude);

        if (hookFired)
        {
            hook.transform.position = hookHelper.transform.position;
            hook.transform.parent = null;
            //grapplingLr.GetComponent<LineRenderer>().enabled = true;
            //grapplingLr.enabled = false;
        }
        else
        {
            hook.transform.parent = hookHolder.transform;
            //grapplingLr.GetComponent<LineRenderer>().enabled = false;
            //grapplingLr.enabled = true;
        }



        // firing the hook
        if (Input.GetKeyDown(KeyCode.Mouse1) && hookFired == false)
        {

            //FPCtrl.AddForce(new Vector3(0.0f, 0.25f, 0.0f));
            //FPCtrl.Update
            this.transform.Translate(Vector3.up * Time.deltaTime * 30);


            StartCoroutine("JumpShootGrappling");
        }

        if (hookFired == true && hooked == false)
        {

            //hook.transform.Translate(Vector3.forward * Time.deltaTime * hookTravelSpeed);
            //hook.transform.Translate(fpsCam.transform.forward * Time.deltaTime * hookTravelSpeed);
            //hook.transform.position = hookHelper.transform.position;
            currentDistance = Vector3.Distance(transform.position, hookHelper.transform.position);

            if (currentDistance >= maxDistance)
                ReturnHook();

        }

        if (hooked == true && hookFired == true)
        {
            hook.transform.parent = hookedObj.transform;
            transform.position = Vector3.MoveTowards(transform.position, hookHelper.transform.position, Time.deltaTime * playerTravelSpeed);
            float distanceToHook = Vector3.Distance(transform.position, hookHelper.transform.position);

            //this.GetComponent<Rigidbody>.useGravity = false;
            //playerRb.useGravity = false;
                //FPCtrl.GetComponent<vp_FPController>().MotorFreeFly = true;
                //FPCtrl.GetComponent<vp_FPController>().PhysicsGravityModifier = 0.0f;
            //FPCtrl.GetComponent<vp_FPController>().Velocity = Vector3.zero;
                //FPCtrl.Stop();

            if (distanceToHook < distToHookMax)
            {
                if (grounded == false)
                {
                    this.transform.Translate(Vector3.forward * Time.deltaTime * 15f);
                    this.transform.Translate(Vector3.up * Time.deltaTime * 20f);
                }
                StartCoroutine("Climb");
            }

        }
        else
        {
            //hook.transform.parent = hookHolder.transform;
            //this.GetComponent<Rigidbody>.useGravity = true;
            // playerRb.useGravity = true;

            //FPCtrl.GetComponent<vp_FPController>().PhysicsGravityModifier = 0.2f;
            //FPCtrl.GetComponent<vp_FPController>().MotorFreeFly = false;

        }
    }

    IEnumerator Climb()
    {
        yield return new WaitForSeconds(0.1f);
        ReturnHook();
    }

    IEnumerator JumpShootGrappling()
    {
        yield return new WaitForSeconds(0.1f);
        ShotGrappling();
    }

    IEnumerator WaitForA()
    {
        yield return new WaitForSeconds(0.05f);
        a = true;
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
                Debug.Log("Grappling hooked to " + hit.transform.name);
                hookedObj = hit.transform.gameObject;
            }
            else
            {
                hooked = false;
                // If we did not hit anything, set the end of the line to a position directly in front of the camera at the distance of hookRange
                hookHelper.transform.position += fpsCam.transform.forward * hookRange;
                //hook.transform.position = Vector3.MoveTowards(hook.transform.position, hook.transform.position + fpsCam.transform.forward * 10f, Time.deltaTime * 5f);
                //hook.transform.position = fpsCam.transform.forward * 10f;


                StartCoroutine("Climb");
                //If raycast hit a hookable surface, the hook is hooked

            }

            if (hit.transform.tag == "Grabbable")
            {
                grabbed = true;
                Debug.Log("Grappling has grabbed " + hit.transform.name);
                grabbedObj = hit.transform.gameObject;
                //hit.transform.gameObject.transform.position = transform.position;

            }
            else
            {
                grabbed = false;
            }

        }
        else
        {
            StartCoroutine("Climb");
        }


        if (grabbed && grabbedObj != null)
        {
            //grabbedObj.transform.position = Vector3.MoveTowards(grabbedObj.transform.position, transform.position, Time.deltaTime * 5f);
        }
    }

    void ReturnHook()
    {
        hook.transform.rotation = hookHolder.transform.rotation;
        hook.transform.position = hookHolder.transform.position;
        hookFired = false;
        hooked = false;
        grabbed = false;
    }

    void ReturnHookSmoothly()
    {
        hook.transform.rotation = hookHolder.transform.rotation;
        hook.transform.position = hookHolder.transform.position;
        hookFired = false;
        hooked = false;

        transform.position = Vector3.MoveTowards(hook.transform.position, hookHolder.transform.position, Time.deltaTime * hookTravelSpeed);
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
}
