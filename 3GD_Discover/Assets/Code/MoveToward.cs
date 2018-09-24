using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Opsive.UltimateCharacterController.Character;


public class MoveToward : MonoBehaviour {
    public Transform _targetTr;
    public float _speed = 20.0f;
    // Use this for initialization*
    public UltimateCharacterLocomotion ucl;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.A)){
            ucl.enabled = false;
            transform.position = Vector3.MoveTowards(transform.position, _targetTr.transform.position, Time.deltaTime * _speed);

        }
    }

    public IEnumerator WaitGrappling()
    {
        yield return new WaitForSeconds(10f);
        ucl.enabled = true;
    }
}
