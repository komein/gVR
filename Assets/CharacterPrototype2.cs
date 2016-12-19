using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPrototype2 : MonoBehaviour
{
    protected Rigidbody rb;

    public Camera cam;

    public float speed = 1f;
    public float strafeSpeed = 1f;

    public float strafeStep = 0.5f;
    public float deadZone = 0.4f;

    public bool hasCompensatingForce = false;

    float curSpeed = 0f;

    Color SEE_COLOR = Color.green;
    Color UNSEE_COLOR = Color.red;

    Vector3 targetVec = Vector3.zero;
    Vector3 compensatingForceVector = Vector3.zero;

    Vector3 collisionVector = Vector3.zero;

    Quaternion rot;

    CharacterController ch;
    Animator anim;
    bool onGround = true;

    List<Collider> grounds = new List<Collider>();
    List<Collider> planeGrounds = new List<Collider>();


    void Awake()
    {
        //Application.targetFrameRate = 60;
    }

    private void Start()
    {
        anim = GetComponent<Animator>();

        rb = GetComponent<Rigidbody>();
        if (null == rb)
            rb = GetComponentInChildren<Rigidbody>();

        ResetTargetVector();

        ch = GetComponent<CharacterController>();
    }

    private void ResetTargetVector()
    {
        targetVec = Vector3.zero;
    }

    private void Update()
    {
        if (null == cam)
            return;

        if (curSpeed < speed)
            curSpeed += Time.deltaTime * speed;

        Vector3 pos = new Vector3(cam.transform.forward.x + (cam.transform.position.x - gameObject.transform.position.x) / 8f, 0, 0) * strafeSpeed;

        if (Mathf.Abs(pos.x) > strafeStep)
            pos.x = strafeStep * Mathf.Sign(pos.x);

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
            transform.rotation = Quaternion.Lerp(transform.rotation, rot_ * Quaternion.LookRotation(pos), Time.fixedDeltaTime * 20);
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Obstacle>() != null)
        {
            curSpeed = 0f;
            other.gameObject.SetActive(false);
        }

        else if (other.GetComponent<Collectible>() != null)
        {
            other.gameObject.SetActive(false);
        }

        else if (other.gameObject.GetComponent<Ground>() != null)
        {
            grounds.Add(other);
            rot = other.transform.rotation;
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
