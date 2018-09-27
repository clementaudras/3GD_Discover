using UnityEngine;
using System.Collections;

public class RopeIgnoreLayerResetter : MonoBehaviour {

    // It's just for demonstration.
    // If you experienced in wrapping rope, disable this script and make your settings of rope ignore layer.

    
	void Start () {
        //  By default the layer assigned to object, that couldn't be processed in collisions with rope is 'Ignore Raycast' layer
        gameObject.layer = 2;

    }

    void Update () {
	
	}
}
