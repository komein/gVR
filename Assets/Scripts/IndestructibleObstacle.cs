
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndestructibleObstacle : MonoBehaviour {

    CrushDelegate doCrush;

    AudioSource aus;
    GameMusic gm;

    public float crushAngle = 165;

    private void Start()
    {
        aus = GetComponent<AudioSource>();
        if (null == aus)
        {
            aus = gameObject.AddComponent<AudioSource>();
        }

        if (null != DataObjects.music)
        {
            aus.clip = DataObjects.music.GetMusic("hit");
        }
    }

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
                            if (aus.clip != null)
                            {
                                aus.Play();
                            }
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
