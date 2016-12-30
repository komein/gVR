﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPrototype2 : MonoBehaviour
{
    protected Rigidbody rb;

    public Camera cam;

    public float speed = 1f;
    public float acceleration = 50f;
    public float strafeSpeed = 1f;

    public float strafeStep = 0.5f;
    public float deadZone = 0.4f;

    public float dropSpeed = 50f;

    public bool hasCompensatingForce = false;

    int count = 0;
    int enableCounter = 5;

    float curSpeed = 0f;

    CharacterController ch;
    Animator anim;

    List<Collider> grounds = new List<Collider>();
    List<Collider> planeGrounds = new List<Collider>();

    SkinnedMeshRenderer mesh;


    void Awake()
    {
        //Application.targetFrameRate = 60;
    }

    private void Start()
    {
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        anim = GetComponent<Animator>();

        rb = GetComponent<Rigidbody>();
        if (null == rb)
        {
            rb = GetComponentInChildren<Rigidbody>();
        }

        ch = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        if (null == cam)
        {
            return;
        }

        if (ch.enabled == false)
        {
            if (count < enableCounter)
            {
                count++;
                return;
            }
            else
            {
                ch.enabled = true;
                count = 0;
            }
        }

        if (curSpeed < speed)
        {
            curSpeed += Time.fixedDeltaTime * acceleration;
        }

        Vector3 pos = new Vector3(cam.transform.forward.x + (cam.transform.position.x - gameObject.transform.position.x) / 8f, 0, 0) * strafeSpeed;

        if (Mathf.Abs(pos.x) > strafeStep)
        {
            pos.x = strafeStep * Mathf.Sign(pos.x);
        }

        pos.z = curSpeed;

        if (null != anim)
        {
            if (planeGrounds.Count == 0 && grounds.Count == 0)
            {
                anim.SetBool("jumping", true);
                anim.speed = 1;
            }
            else
            {
                anim.SetBool("jumping", false);
                float d = Vector3.Distance(Vector3.zero, pos);
                anim.SetFloat("speed", d);
            }
        }

        ch.SimpleMove(pos * Time.deltaTime);

        Quaternion rot_ = planeGrounds.Count != 0 || grounds.Count == 0 ? Quaternion.identity : grounds[grounds.Count-1].transform.rotation;

        if (pos != Vector3.zero)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, rot_ * Quaternion.LookRotation(pos), Time.fixedDeltaTime * 20);
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Obstacle>() != null)
        {
            other.gameObject.SetActive(false);
        }/*
        else if (other.GetComponent<IndestructibleObstacle>() != null)
        {
            curSpeed -= dropSpeed;
        }*/
        else if (other.GetComponent<Collectible>() != null)
        {
            other.GetComponent<Collectible>().Collect();
        }

        else if (other.gameObject.GetComponent<Ground>() != null)
        {
            grounds.Add(other);
        }

        else if (other.gameObject.GetComponent<PlaneGround>() != null)
        {
            planeGrounds.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Ground>() != null)
        {
            grounds.Remove(other);
        }

        else if (other.gameObject.GetComponent<PlaneGround>() != null)
        {
            planeGrounds.Remove(other);
        }
    }

    public void MakeCrush(Vector3 v)
    {
        ch.enabled = false;
        v = v * 100;
        v.y = 10;
        rb.AddForce(v, ForceMode.Impulse);
        curSpeed = 0;
    }

    public void ToggleFlashing(bool v)
    {
        if (v == true)
        {
            StartCoroutine(Flash());
        }
        else
        {
            StopAllCoroutines();
            mesh.enabled = true;
        }
    }

    private IEnumerator Flash()
    {
        if (null != mesh)
        {
            while (true)
            {
                mesh.enabled = !mesh.enabled;
                yield return new WaitForSeconds(0.1f);
            }
        }
        yield return null;
    }
}
