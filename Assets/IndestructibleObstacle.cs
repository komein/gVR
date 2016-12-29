
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndestructibleObstacle : MonoBehaviour {

    CrushDelegate doCrush;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    private void OnCollisionEnter(Collision collision)
    {
        CharacterPrototype2 chc = collision.collider.GetComponent<CharacterPrototype2>();
        if (chc != null)
        {
            if (collision.contacts.Length > 0)
            {
                if (null != doCrush)
                {
                    doCrush(collision);
                }
            }
        }
    }

    public void SetCrushAction(CrushDelegate c)
    {
        doCrush = c;
    }
}
