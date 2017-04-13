using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomMover : AbstractMover
{
    List<Collider> grounds = new List<Collider>();
    List<Collider> planeGrounds = new List<Collider>();

    UnityEngine.CharacterController ch;

    protected override void Start()
    {
        base.Start();
        ch = GetComponent<UnityEngine.CharacterController>();
    }

    protected override void Update ()
    {
        currentSpeed += Time.deltaTime * acceleration;

        Vector3 v = cam.transform.forward;
        v.y = 0;

        ch.SimpleMove(v * currentSpeed * Time.deltaTime);
    }


    protected override void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Obstacle>() != null)
        {
            currentSpeed -= dropSpeed;
            currentSpeed = Mathf.Max(0, currentSpeed);
            other.gameObject.SetActive(false);
        }
        else if (other.GetComponent<IndestructibleObstacle>() != null)
        {
            currentSpeed = dropSpeed;
        }

        else if (other.GetComponent<Collectible>() != null)
        {
            other.gameObject.SetActive(false);
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
}
