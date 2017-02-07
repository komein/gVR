using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPrototype1 : MonoBehaviour {

    protected Rigidbody rb;

    public GvrReticle camReticle;
    public float speed = 1f;
    public float maxStepPerTick = 4f;

    public float curSpeed = 0f;

    public enum MovingMode { kinematic, inertial }

    public MovingMode movMode;

    GazeInputModule gim;

    Color SEE_COLOR = Color.green;
    Color UNSEE_COLOR = Color.red;

    Vector3 targetVec = Vector3.zero;

    Camera cam;

    private void Start()
    {
        cam = Camera.main;

        rb = GetComponent<Rigidbody>();
        if (null == rb)
            rb = GetComponentInChildren<Rigidbody>();

        gim = SceneSelecter.FindObjectOfType<GazeInputModule>();

        ResetTargetVector();
    }

    private void ResetTargetVector()
    {
        if (movMode == MovingMode.inertial)
        {
            targetVec = Vector3.zero;
        }
        else
        {
            targetVec = gameObject.transform.position;
        }
    }
    /*
    void DrawLine(LineRenderer lr, Vector3 start, Vector3 end, Color color, float width)
    {
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.SetColors(color, color);
        lr.SetWidth(width, width);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }
    */
    void Update () {
        /*
        if (movMode == MovingMode.inertial)
        {
            if (curSpeed < speed)
                curSpeed += Time.deltaTime * speed;
            rb.velocity = new Vector3(0, 0, curSpeed);
        }

        GameObject target = gim.GetCurrentGameObject();

        //Debug.Log(target);

        if (null != target)
        {/*
            Vector3 gotoPos = gim.GetIntersectionPosition(cam);
            Vector3 movePos = new Vector3(gotoPos.x, gameObject.transform.position.y, gotoPos.z);

            RaycastHit hit;
            Physics.Raycast(gameObject.transform.position, gotoPos - gameObject.transform.position, out hit, Mathf.Infinity, LayerMask.NameToLayer("TransparentFX"));
            */
            //Debug.DrawLine(gameObject.transform.position, movePos, Color.yellow, Time.fixedDeltaTime);

            //LineRenderer lr = GetComponent<LineRenderer>();


            //DrawLine(GetComponent<LineRenderer>(), gameObject.transform.position, hit.point, Color.yellow, 0.1f);
            /*
            if (hit.collider == null)
            {
                ResetTargetVector();

                GetComponent<Renderer>().material.color = UNSEE_COLOR;
                return;
            }*/
            /*
            if (!Object.Equals(hit.collider.gameObject, target))
            {
                ResetTargetVector();

                GetComponent<Renderer>().material.color = UNSEE_COLOR;
                return;
            }
            else
            {

                GetComponent<Renderer>().material.color = SEE_COLOR;

                float step = Vector3.Distance(gameObject.transform.position, movePos);
                
                if (movMode == MovingMode.inertial)
                {
                    movePos = new Vector3(movePos.x, movePos.y, gameObject.transform.position.z);
                    targetVec = (movePos - gameObject.transform.position) * speed;

                    rb.AddForce((movePos - gameObject.transform.position) * speed, ForceMode.Force);
                }
                else
                {
                    if (step > maxStepPerTick)
                    {
                        movePos = Vector3.Lerp(gameObject.transform.position, movePos, maxStepPerTick / step);
                        step = maxStepPerTick;
                    }

                    targetVec = Vector3.Lerp(gameObject.transform.position, movePos, speed * Time.deltaTime);
                    rb.MovePosition(Vector3.Lerp(gameObject.transform.position, movePos, speed * Time.deltaTime));
                }
            }
        }*/
    }

    private void FixedUpdate()
    {
        if (movMode == MovingMode.inertial)
        {
            rb.AddForce(targetVec, ForceMode.Force);
        }
        else
        {
            rb.MovePosition(targetVec);
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Obstacle>() != null)
        {
            if (movMode == MovingMode.inertial)
            {

                other.gameObject.SetActive(false);
            }
        }

        if (other.GetComponent<Collectible>() != null)
        {
            other.gameObject.SetActive(false);
        }
    }

}
