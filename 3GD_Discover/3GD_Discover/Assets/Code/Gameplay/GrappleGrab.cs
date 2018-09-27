using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrappleGrab : MonoBehaviour {

    public Camera fpsCam;
    public float hookRange = 50.0f;
    public Image m_crosshair;
    public Sprite m_targetObjSprite;
    public Sprite m_normalObjSprite;
    public GameObject hookHelper;
    public GameObject m_grabbededObj;
    public Transform m_playerTr;
    public bool _grabbed;
    public GameObject hook;
    public GameObject grapplingLr;
    public GameObject hookHolder;

    private float m_objTravelSpeed;
    public float m_distanceToPlayerMax = 2.5f;
    public Transform m_leftHand;
    private bool _smallObjInHand;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (_grabbed)
        {
            hook.transform.position = m_grabbededObj.transform.position;
            hook.transform.parent = m_grabbededObj.transform;
            m_grabbededObj.GetComponent<Rigidbody>().useGravity = true;
            AttractObject();
            grapplingLr.GetComponent<LineRenderer>().enabled = true;
        }
        else {
            hook.transform.parent = hookHolder.transform;
            grapplingLr.GetComponent<LineRenderer>().enabled = false;
            grapplingLr.GetComponent<LineRenderer>().enabled = false;
            m_objTravelSpeed = 0.0f;
            m_distanceToPlayerMax = 2.5f;
            
        }


        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ShootGrappleGrab();
        }

        GrappleGrabSprites();

    }

    void AttractObject()
    {
        m_grabbededObj.transform.position = Vector3.MoveTowards(m_grabbededObj.transform.position, m_playerTr.position, Time.deltaTime * m_objTravelSpeed);
        float m_distanceToPlayer = Vector3.Distance(m_grabbededObj.transform.position, m_playerTr.transform.position);
        if (m_distanceToPlayer < m_distanceToPlayerMax)
        {
            m_grabbededObj.GetComponent<Rigidbody>().useGravity = true;
            _grabbed = false;
        }
    }

    void GrappleGrabSprites()
    {
        // Create a vector at the center of our camera's viewport
        Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.2f));

        // Declare a raycast hit to store information about what our raycast has hit
        RaycastHit hit;

        Debug.DrawRay(rayOrigin, fpsCam.transform.forward * 100.0f, Color.magenta);

        if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, hookRange))
        {
            if (!hit.transform.GetComponent<ObjectWeight>())
            {
                m_crosshair.sprite = m_normalObjSprite;
                return;
            }
            m_crosshair.sprite = m_targetObjSprite;
        }
        else
        {
            m_crosshair.sprite = m_normalObjSprite;
        }
    }


    void ShootGrappleGrab()
    {
        // Create a vector at the center of our camera's viewport
        Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.2f));

        // Declare a raycast hit to store information about what our raycast has hit
        RaycastHit hit;

        Debug.DrawRay(rayOrigin, fpsCam.transform.forward * 100.0f, Color.magenta);

        if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, hookRange))
        {
            if (!hit.transform.GetComponent<ObjectWeight>())
            {
                //_grabbed = false;
                return;
            }

            //If raycast hit an object with the Weight Object script
            //If the object is light
            if (hit.transform.GetComponent<ObjectWeight>().m_light)
            {
                m_grabbededObj = hit.transform.gameObject;
                m_objTravelSpeed = 15.0f;
                _grabbed = true;
                _smallObjInHand = true;
                //Debug.Log("Light Object");
            }
            else
            {
                hookHelper.transform.position += fpsCam.transform.forward * hookRange;
            }

            //If the object is medium
            if (hit.transform.GetComponent<ObjectWeight>().m_medium)
            {
                m_grabbededObj = hit.transform.gameObject;
                m_objTravelSpeed = 10.0f;
                _grabbed = true;
                //Debug.Log("Medium Object");
            }
            else
            {
                hookHelper.transform.position += fpsCam.transform.forward * hookRange;
            }

            //If the object is heavy
            if (hit.transform.GetComponent<ObjectWeight>().m_heavy)
            {
                m_grabbededObj = hit.transform.gameObject;
                m_objTravelSpeed = 1.0f;
                _grabbed = true;
                m_distanceToPlayerMax = 5.0f;
                //Debug.Log("Heavy Object");
            }
            else
            {
                hookHelper.transform.position += fpsCam.transform.forward * hookRange;
            }
        }
        else
        {
            return;
        }
    }

    void GrabSmallObjectInHand()
    {
        m_grabbededObj.transform.position = m_leftHand.transform.position;
        m_grabbededObj.transform.parent = m_leftHand.transform;
    }
}
