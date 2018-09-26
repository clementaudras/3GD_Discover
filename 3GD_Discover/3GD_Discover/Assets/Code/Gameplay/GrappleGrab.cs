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
    
    private float m_objTravelSpeed = 10.0f;
    private float m_distanceToPlayerMax = 1.0f;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (_grabbed)
        {
            AttractObject();
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
                //Debug.Log("Light Object");
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
                _grabbed = true;
                //Debug.Log("Heavy Object");
            }
        }
        else
        {
            return;
        }
    }
}
