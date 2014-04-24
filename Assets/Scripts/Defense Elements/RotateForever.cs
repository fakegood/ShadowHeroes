using UnityEngine;
using System.Collections;

public class RotateForever : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		this.transform.Rotate(0,0,15f);
	}
}
