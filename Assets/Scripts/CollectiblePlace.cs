using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectiblePlace : MonoBehaviour
{

    MeshRenderer mesh;

	void Start ()
    {
        mesh = GetComponent<MeshRenderer>();

        if (null != mesh)
        {
            mesh.enabled = false;
        }

	}
}
