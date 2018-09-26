using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stalactite : MonoBehaviour {
    public Animator anim_stalactiteFall;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hook"))
        {
            //yes dirty... I know. Dont judge plz
            anim_stalactiteFall.enabled = true;

            Debug.Log("Stalactite fall");
        }
    }
}
