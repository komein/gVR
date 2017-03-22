
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndestructibleObstacle : MonoBehaviour {

    CrushDelegate doCrush;

    public float crushAngle = 165;

    private void OnCollisionEnter(Collision collision)
    {
        RunningCatController chc = collision.collider.GetComponent<RunningCatController>();
        if (chc != null)
        {
            if (collision.contacts.Length > 0)
            {
                Vector3 v = chc.transform.position - collision.contacts[0].point;
                v.y = 0;
                float angle = Vector3.Angle(Vector3.forward, v);
                if (angle > crushAngle)
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
