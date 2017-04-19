
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndestructibleObstacle : MonoBehaviour
{
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

        DataObjects.SetMusic("hit", aus);
    }

    private void OnCollisionEnter(Collision collision)
    {

        Debug.Log(collision.collider as BoxCollider);
        Debug.Log(collision.collider as CapsuleCollider);

        PlayerController chc = collision.collider.GetComponent<PlayerController>();
        if (chc != null)
        {
            if (collision.contacts.Length > 0)
            {
                if (chc.IsJumping)
                {
                    Crush(-Vector3.forward);
                }
                if (collision.collider as CapsuleCollider)
                {
                    Vector3 v = chc.transform.position - collision.contacts[0].point;
                    v.y = 0;
                    Crush(v);
                }
            }
        }
    }

    private void Crush(Vector3 v)
    {
        if (null != doCrush)
        {
            doCrush(v);
            if (null != aus)
            {
                if (aus.clip != null)
                {
                    aus.Play();
                }
            }
        }
    }

    public void SetCrushAction(CrushDelegate c)
    {
        doCrush = c;
    }
}
