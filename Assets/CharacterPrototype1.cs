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

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (null == rb)
            rb = GetComponentInChildren<Rigidbody>();

        gim = SceneSelecter.FindObjectOfType<GazeInputModule>();
    }

    void DrawLine(LineRenderer lr, Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.SetColors(color, color);
        lr.SetWidth(0.1f, 0.1f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }

    void FixedUpdate () {
        if (movMode == MovingMode.inertial)
        {
            if (curSpeed < speed)
                curSpeed += Time.fixedDeltaTime * speed;
            rb.velocity = new Vector3(0, 0, curSpeed);
        }

        GameObject target = gim.GetCurrentGameObject();

        if (null != target)
        {

            Vector3 gotoPos = gim.GetIntersectionPosition();
            Vector3 movePos = new Vector3(gotoPos.x, gameObject.transform.position.y, gotoPos.z);

            RaycastHit hit;
            Physics.Raycast(gameObject.transform.position, gotoPos - gameObject.transform.position, out hit);

            //Debug.DrawLine(gameObject.transform.position, movePos, Color.yellow, Time.fixedDeltaTime);

            LineRenderer lr = GetComponent<LineRenderer>();

            DrawLine(GetComponent<LineRenderer>(), gameObject.transform.position, hit.point, Color.yellow, Time.fixedDeltaTime);

            if (hit.collider == null)
            {
                GetComponent<Renderer>().material.color = UNSEE_COLOR;
                return;
            }

            if (!Object.Equals(hit.collider.gameObject, target))
            {
                GetComponent<Renderer>().material.color = UNSEE_COLOR;
                return;
            }
            else
            {
                GetComponent<Renderer>().material.color = SEE_COLOR;

                float step = Vector3.Distance(gameObject.transform.position, movePos);
                
                if (movMode == MovingMode.inertial)
                {
                    //rb.velocity = rb.velocity + new Vector3(0, 0, movePos.z);
                    movePos = new Vector3(movePos.x, movePos.y, gameObject.transform.position.z);
                    rb.AddForce((movePos - gameObject.transform.position) * speed, ForceMode.Force);
                }
                else
                {
                    if (step > maxStepPerTick)
                    {
                        movePos = Vector3.Lerp(gameObject.transform.position, movePos, maxStepPerTick / step);
                        step = maxStepPerTick;
                    }
                    rb.MovePosition(Vector3.Lerp(gameObject.transform.position, movePos, speed * Time.fixedDeltaTime));
                }
            }
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Obstacle>() != null)
        {
            if (movMode == MovingMode.inertial)
            {
                curSpeed = 0f;
                Destroy(other.gameObject);
            }
        }
    }

}
