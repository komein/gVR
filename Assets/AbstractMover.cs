using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbstractMover : MonoBehaviour {

    public enum ControlMode { move, rotate };

    public ControlMode mode;

    protected Camera cam;
    protected Rigidbody rb;

    public float maxSpeed = 1f;
    public float minSpeed = 0f;
    public float acceleration = 3f;

    public float strafeSpeed = 3f;

    public float dropSpeed = 50f;

    protected float currentSpeed = 0f;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (null == cam)
            cam = GetComponentInChildren<Camera>();

        rb = GetComponent<Rigidbody>();
        if (null == rb)
            rb = GetComponentInChildren<Rigidbody>();
    }

    protected virtual void Update () {
        if (currentSpeed < maxSpeed)
        {
            if (cam.transform.forward.y > 0)
            {
                currentSpeed += acceleration * Time.deltaTime;
            }
            else
            {
                currentSpeed -= (-cam.transform.forward.y) * acceleration * Time.deltaTime;
            }
            currentSpeed = Mathf.Max(currentSpeed, 0);
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Obstacle>() != null)
        {
            currentSpeed -= dropSpeed;
            currentSpeed = Mathf.Max(0, currentSpeed);

            Destroy(other.gameObject);
        }
    }
}
