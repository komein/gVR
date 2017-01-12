using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpContainer : MonoBehaviour {

    MeshRenderer mesh;

	// Use this for initialization
	void Start () {
        mesh = GetComponent<MeshRenderer>();	
	}
	
    public void Set(bool value)
    {
        if (null != mesh)
        {
            mesh.enabled = value;
        }
    }
}
