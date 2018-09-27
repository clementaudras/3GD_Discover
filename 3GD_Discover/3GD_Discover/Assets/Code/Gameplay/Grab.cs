using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Grab : MonoBehaviour {
    public float m_GrabDistanceMax;

    public Camera fpsCam;
    public float m_InteractDist = 1.0f;

    public GameObject m_pickedUpObject;
    public bool m_IsGrabbed;
    public bool m_CanGrab;
    public Transform m_leftHand;

    public float Stiffness = 0.5f;

    public Image m_grabCrossHair;
    public Sprite m_normalObjSprite;
    public Sprite m_targetObjSprite;

    public Collider m_PlayerCollider;

    protected Rigidbody m_Rigidbody = null;
    protected Rigidbody Rigidbody
    {
        get
        {
            if (m_Rigidbody == null)
                m_Rigidbody = GetComponent<Rigidbody>();
            return m_Rigidbody;
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Detector();

        if(!m_IsGrabbed && m_pickedUpObject != null)
        {
            StartGrab();
        }

        if(m_IsGrabbed)
            m_pickedUpObject = m_leftHand.GetChild(0).gameObject;

        if (m_leftHand.childCount == 0)
        {
            m_IsGrabbed = false;
        }

        if(m_pickedUpObject == null)
        {
            m_IsGrabbed = false;
        }

        StartThrow();
    }

    void Detector()
    {

        // Create a vector at the center of our camera's viewport
        Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.2f));

        // Declare a raycast hit to store information about what our raycast has hit
        RaycastHit hit;

        Debug.DrawRay(rayOrigin, fpsCam.transform.forward* 5.0f, Color.green);

        if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, m_InteractDist))
        {
            if (!hit.transform.GetComponent<ObjectWeight>())
            {
                m_CanGrab = false;
                m_grabCrossHair.sprite = m_normalObjSprite;
                //m_pickedUpObject.transform.transform.parent = null;
                return;
            }

            m_grabCrossHair.sprite = m_targetObjSprite;
            
            //If raycast hit an object with the Weight Object script
            //If the object is light
            if (hit.transform.GetComponent<ObjectWeight>().m_light || hit.transform.GetComponent<ObjectWeight>().m_medium)
            {
                m_pickedUpObject = hit.transform.gameObject;
                m_CanGrab = true;
                //Debug.Log("Light Object");
            }
            else
            {
                m_pickedUpObject = null;
                return;
            }

            /*
            //If the object is medium
            if (hit.transform.GetComponent<ObjectWeight>().m_medium)
            {
                m_pickedUpObject = hit.transform.gameObject;
                m_CanGrab = true;
                //Debug.Log("Medium Object");
            }
            else
            {
                return;
            }
            */
        }
        else
        {
            m_pickedUpObject = null;
            //m_pickedUpObject.transform.transform.parent = null;
            m_CanGrab = false;
            m_grabCrossHair.sprite = m_normalObjSprite;
            return;
        }
    }

    void StartGrab()
        {

        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(m_WaitGrabbedBool());
            m_pickedUpObject.transform.position = m_leftHand.transform.position;
            m_pickedUpObject.GetComponent<Rigidbody>().useGravity = false;
            m_pickedUpObject.GetComponent<Rigidbody>().isKinematic = true;
            m_pickedUpObject.GetComponent<Collider>().enabled = false;
            m_pickedUpObject.layer = 29;
            Physics.IgnoreCollision(m_PlayerCollider, m_pickedUpObject.GetComponent<Collider>(), true);
            m_pickedUpObject.transform.transform.parent = m_leftHand.transform;
        }



        /*
        // make player ignore collisions with this object
        if (m_Controller.Transform.GetComponent<Collider>().enabled && Collider.enabled)
            Physics.IgnoreCollision(m_Controller.Transform.GetComponent<Collider>(), Collider, true);

            // alter this object's physics to allow proper carrying motion
            if (Rigidbody != null)
            {
                Rigidbody.useGravity = false;
                Rigidbody.drag = (Stiffness * 60.0f);
                Rigidbody.angularDrag = (Stiffness * 60.0f);
            }
            */
    }




    void StartThrow()
    {
        if(m_IsGrabbed && Input.GetKeyDown(KeyCode.F))
        {
            // Create a vector at the center of our camera's viewport
            Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.2f));
            StartCoroutine(m_WaitReset());
            m_IsGrabbed = false;
            m_pickedUpObject.GetComponent<Rigidbody>().useGravity = true;
            m_pickedUpObject.GetComponent<Rigidbody>().isKinematic = false;
            m_pickedUpObject.GetComponent<Collider>().enabled = true;
            m_pickedUpObject.transform.transform.parent = null;
            m_pickedUpObject.layer = 0;
            m_pickedUpObject.GetComponent<Rigidbody>().AddForce(rayOrigin * 5.0f, ForceMode.Impulse);
        }
    }

    IEnumerator m_WaitGrabbedBool()
    {
        yield return new WaitForSeconds(0.5f);
        m_IsGrabbed = true;
    }

    IEnumerator m_WaitReset()
    {
        yield return new WaitForSeconds(0.5f);
        m_pickedUpObject = null;

    }
}
