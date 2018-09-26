using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class ObiRopeSetLength : MonoBehaviour {
    public ObiRopeCursor cursor;
    public float m_ropeLength;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        cursor.ChangeLength(m_ropeLength);
    }
}
