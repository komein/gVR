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

    //public float maxSpeed = 2f;
    //public float minSpeed = 0.5f;

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
    int planeCount = 0;

    void Awake()
    {
        Application.targetFrameRate = 60;
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

    void DrawLine(LineRenderer lr, Vector3 start, Vector3 end, Color color, float width)
    {
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.SetColors(color, color);
        lr.SetWidth(width, width);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }

    private void Update()
    {
        if (null == cam)
            return;

        if (curSpeed < speed)
            curSpeed += Time.deltaTime * speed;

        if (null != anim)
        {
            if (!onGround && planeCount == 0)
            {
                anim.SetBool("jumping", true);
                anim.speed = 1;
            }
            else
            {   
                anim.SetBool("jumping", false);

                anim.SetFloat("speed", curSpeed);
                anim.speed = curSpeed / speed * 0.5f + 0.5f;
            }
        }

        Vector3 pos = new Vector3(cam.transform.forward.x - gameObject.transform.position.x / 16f, 0, 0) * strafeSpeed;

        if (Mathf.Abs(pos.x) > strafeStep)
            pos.x = strafeStep * Mathf.Sign(pos.x);

        pos.z = curSpeed;

        ch.SimpleMove(pos * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, 0.1f);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(pos), 0.1f);
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Obstacle>() != null)
        {
            curSpeed = 0f;
            other.gameObject.SetActive(false);
        }

        if (other.GetComponent<Collectible>() != null)
        {
            other.gameObject.SetActive(false);
        }

        if (other.gameObject.GetComponent<Ground>() != null)
        {
            this.GetComponent<ConstantForce>().enabled = true;
            planeCount++;
            rot = other.transform.rotation;
        }

        if (other.gameObject.GetComponent<PlaneGround>() != null)
        {
            this.GetComponent<ConstantForce>().enabled = true;
            onGround = true;
            rot = Quaternion.identity;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Ground>() != null)
        {
            this.GetComponent<ConstantForce>().enabled = false;
            planeCount--;
            rot = Quaternion.identity;
        }

        if (other.gameObject.GetComponent<PlaneGround>() != null)
        {
            this.GetComponent<ConstantForce>().enabled = false;
            onGround = false;
            rot = other.transform.rotation;
        }
    }
}
