using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectiblePlace : MonoBehaviour {

    MeshRenderer mesh;

	// Use this for initialization
	void Start () {
        mesh = GetComponent<MeshRenderer>();
        if (null != mesh)
        {
            mesh.enabled = false;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
