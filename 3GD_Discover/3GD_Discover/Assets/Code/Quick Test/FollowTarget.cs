using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour {
    public Transform m_target;

	void FixedUpdate () {
		transform.position = m_target.position;
    }
}
