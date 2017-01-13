
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndestructibleObstacle : MonoBehaviour {

    CrushDelegate doCrush;
    
    private void OnCollisionEnter(Collision collision)
    {
        CharacterPrototype2 chc = collision.collider.GetComponent<CharacterPrototype2>();
        if (chc != null)
        {
            if (collision.contacts.Length > 0)
            {
                Vector3 v = chc.transform.position - collision.contacts[0].point;
                v.y = 0;
                float angle = Vector3.Angle(Vector3.forward, v);
                if (Vector3.Angle(Vector3.forward, v) > 165)
                {
                    if (null != doCrush)
                    {
                        doCrush(v);
                        AudioSource aus = GetComponent<AudioSource>();
                        if (null != aus)
                        {
                            aus.Play();
                        }
                    }
                }
            }
        }
    }

    public void SetCrushAction(CrushDelegate c)
    {
        doCrush = c;
    }
}
