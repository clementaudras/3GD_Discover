using UnityEngine;
using System.Collections;

public class Emitter : MonoBehaviour {

    public GameObject Projectile;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            var projectile = GameObject.Instantiate(Projectile);
            projectile.transform.position = transform.position;
            projectile.GetComponent<Rigidbody>().AddForce(transform.rotation * Vector3.forward * -500f, ForceMode.Force);
        }
    }
}
