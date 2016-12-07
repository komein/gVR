using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPrototype1 : MonoBehaviour {

    public CharacterPrototype1 ch;
    public float distance = 3f;
    public float height = 3f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        Vector3 newPos = new Vector3(gameObject.transform.position.x, ch.transform.position.y + height, ch.transform.position.z - distance);
        gameObject.transform.position = newPos;
	}
}
