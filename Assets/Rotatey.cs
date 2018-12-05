using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotatey : MonoBehaviour {
    
	void Update () {
        transform.Rotate (new Vector3 (90, 90, 90)  * 2 * Time.deltaTime);
	}
}
