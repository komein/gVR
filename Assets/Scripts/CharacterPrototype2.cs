using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterPrototype2 : MonoBehaviour
{
    enum CatState { paused, cantMove, jump, dying, moving };
    CatState currentState;

    protected Rigidbody rb;

    public Camera cam;

    public float maxSpeed = 1f;
    public float acceleration = 50f;
    public float strafeSpeed = 1f;

    public float strafeStep = 0.5f;
    public float deadZone = 0.4f;

    public float dropSpeed = 50f;

    public bool hasCompensatingForce = false;
 
    float curSpeed = 0f;
    
    Animator anim;

    List<GroundContainer> highGrounds = new List<GroundContainer>();
    List<GroundContainer> planeGrounds = new List<GroundContainer>();

    Coroutine immobilizeCoroutine = null;

    SkinnedMeshRenderer mesh;

    DataStorage data;

    Coroutine c;

    Vector3 gravity;

    private void Start()
    {
        currentState = CatState.paused;

        data = FindObjectOfType<DataStorage>();
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        anim = GetComponent<Animator>();

        rb = GetComponent<Rigidbody>();
        if (null == rb)
        {
            rb = GetComponentInChildren<Rigidbody>();
        }

        if (null == cam)
            cam = Camera.main;

        StartCoroutine(StartRunning());

        rb.useGravity = true;
        //gravity = Physics.gravity;
    }

    private IEnumerator StartRunning()
    {
        yield return new WaitForSeconds(1);
        currentState = CatState.moving;
        yield return null;
    }

    private void FixedUpdate()
    {
        if (null == cam)
        {
            return;
        }
        Vector3 pos = Vector3.zero;

        SetGravity();
        SelectCatState();

        switch (currentState)
        {
            case CatState.paused:
            case CatState.dying:
            case CatState.cantMove:

                curSpeed = 0;
                SetIdleAnimation();

                return;

            case CatState.jump:

                pos = GetMoveVector();
                MoveAndRotate(pos);
                SetJumpAnimation();

                return;

            case CatState.moving:

                if (curSpeed < maxSpeed)
                {
                    curSpeed += Time.fixedDeltaTime * acceleration;
                }
                pos = GetMoveVector();
                MoveAndRotate(pos);

                SetAnimation(pos);

                return;

        }

    }

    private void SelectCatState()
    {
        switch (currentState)
        {
            case CatState.paused:
            case CatState.dying:
            case CatState.cantMove:
                return;
            case CatState.jump:
            case CatState.moving:
                break;
        }

        if (highGrounds.Count == 0 && planeGrounds.Count == 0)
        {
            currentState = CatState.jump;
        }
        else
        {
            currentState = CatState.moving;
        }
    }

    private void SetGravity()
    {
        switch (currentState)
        {
            case CatState.paused:
            case CatState.dying:
            case CatState.cantMove:
            case CatState.jump:
                rb.useGravity = true;
                //Physics.gravity = gravity;
                return;
            case CatState.moving:
                rb.useGravity = false;
                //Physics.gravity = Vector3.zero;
                return;
        }
    }

    private Vector3 GetMoveVector()
    {
        Vector3 pos = new Vector3(cam.transform.forward.x + (cam.transform.position.x - gameObject.transform.position.x) / 8f, 0, 0) * strafeSpeed;

        if (Mathf.Abs(pos.x) > strafeStep)
        {
            pos.x = strafeStep * Mathf.Sign(pos.x);
        }

        pos.z = curSpeed;

        return pos;
    }

    private void MoveAndRotate(Vector3 pos)
    {
        if (null != data)
        {

            if (data.isAlive)
            {


                if (currentState == CatState.moving)
                {
                    Quaternion rot_ = Quaternion.identity;

                    if (highGrounds.Count > 0)
                    {
                        rot_ = highGrounds.Last().transform.rotation;
                        rot_ = Quaternion.Euler(-rot_.eulerAngles.x, 0, 0);

                    }
                    else
                    {
                        if (planeGrounds.Count > 0)
                        {
                            rot_ = planeGrounds.Last().transform.rotation;
                            if (!planeGrounds.Last().rotationCrutch)
                            {
                                rot_ = Quaternion.Euler(rot_.eulerAngles.x, 0, 0);
                            }
                            else
                            {
                                rot_ = Quaternion.Euler(rot_.eulerAngles.x - 90f, 0, 0);
                            }
                        }

                        //rb.velocity = pos * Time.fixedDeltaTime;
                    }

                    rb.velocity = rot_ * pos * Time.fixedDeltaTime;

                    if (pos != Vector3.zero && curSpeed > 0)
                    {
                        transform.rotation = Quaternion.Lerp(transform.rotation, rot_ * Quaternion.LookRotation(pos), Time.fixedDeltaTime * 5);
                    }

                    return;
                }
                else if (currentState == CatState.jump)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.fixedDeltaTime);
                }
            }

            
        }
    }

    private Quaternion GetNormal()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit))
        {
            Debug.DrawLine(hit.point, hit.normal + hit.point, Color.red, 5f);
            return Quaternion.FromToRotation(Vector3.up, hit.normal);
        }

        return Quaternion.identity;
    }

    public void Jump()
    {
        immobilizeCoroutine = StartCoroutine(JumpCoroutine());
        MakeKnockBack(Vector3.forward, 5, 0, false);
    }

    private void SetAnimation(Vector3 pos)
    {
        if (null != anim)
        {
            if (planeGrounds.Count == 0 && highGrounds.Count == 0)
            {
                SetJumpAnimation();
            }
            else
            {
                anim.SetBool("jumping", false);
                float d = Vector3.Distance(Vector3.zero, pos);
                anim.SetFloat("speed", d);

                anim.speed = 1f;
            }
        }
    }

    private void SetIdleAnimation()
    {
        if (null != anim)
        {
            anim.SetBool("jumping", false);
            anim.SetFloat("speed", 0f);

            anim.speed = 1f;
        }
    }

    private void SetJumpAnimation()
    {
        if (null != anim)
        {
            anim.SetBool("jumping", true);
            anim.speed = 0.33f;
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        Debug.Log("enter " + other.name);
        if (other.GetComponent<Obstacle>() != null)
        {
            other.gameObject.SetActive(false);
        }
        else if (other.GetComponent<Collectible>() != null)
        {
            other.GetComponent<Collectible>().Collect();
        }
        else if (other.gameObject.GetComponent<HighGround>() != null)
        {
            GroundContainer cont = other.GetComponent<GroundContainer>();
            if (null == cont)
                cont = other.GetComponentInParent<GroundContainer>();
            if (null != cont)
                highGrounds.Add(cont);
        }
        else if (other.gameObject.GetComponent<PlaneGround>() != null)
        {
            if (null != immobilizeCoroutine)
            {
                StopCoroutine(immobilizeCoroutine);
                
                currentState = CatState.moving;

                immobilizeCoroutine = null;
            }

            GroundContainer cont = other.GetComponent<GroundContainer>();
            if (null == cont)
                cont = other.GetComponentInParent<GroundContainer>();
            if (null != cont)
                if (!planeGrounds.Contains(cont))
                    planeGrounds.Add(cont);
        }
    }
    

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("exit " + other.name);
        GroundContainer cont = other.GetComponent<GroundContainer>();
        if (null == cont)
            cont = other.GetComponentInParent<GroundContainer>();

        if (other.gameObject.GetComponent<HighGround>() != null)
        {
            highGrounds.RemoveAll(p => p == cont);
            if (highGrounds.Count == 0)
            {
                rb.velocity = new Vector3(0, 0, rb.velocity.z);
                Jump();
            }
        }

        else if (other.gameObject.GetComponent<PlaneGround>() != null)
        {
            planeGrounds.RemoveAll(p => p == cont);
        }
    }

    public void MakeCrush(Vector3 v)
    {
        CrushParticle cp = FindObjectOfType<CrushParticle>();
        if (null != cp)
        {
            ParticleSystem cm = cp.GetComponent<ParticleSystem>();
            if (null != cm)
            {
                cm.transform.position = transform.position + new Vector3(0,0.3f,0);
                cm.Play();
            }
        }

        if (data != null)
        {
            TakeDamage();

            if (!data.isAlive)
            {
                StartCoroutine(Die());
            }
            else
            {
                immobilizeCoroutine = StartCoroutine(ImmobilizeCoroutine());
            }

            MakeKnockBack(v, 2, 2);
        }

    }

    private void MakeKnockBack(Vector3 v, float forceUp, float forceHor, bool resetSpeed = true)
    {
        v = v.normalized * forceHor;
        v.y = forceUp;

        rb.AddForce(v, ForceMode.Impulse);

        if (resetSpeed)
            curSpeed = 0;
    }

    public void TakeDamage()
    {
        if (null != data)
        {
            data.LoseHp(1);
        }
    }

    public void ToggleFlashing(bool v)
    {
        if (v == true)
        {
            c = StartCoroutine(Flash());
        }
        else
        {
            if (null != c)
            {
                StopCoroutine(c);
                c = null;
                mesh.enabled = true;
            }
        }
    }

    private IEnumerator ImmobilizeCoroutine()
    {
        currentState = CatState.cantMove;
        yield return new WaitForSeconds(1.5f);
        currentState = CatState.moving;

        yield return null;
    }

    private IEnumerator JumpCoroutine()
    {
        currentState = CatState.jump;
        yield return new WaitForSeconds(5f);
        currentState = CatState.moving;

        yield return null;
    }

    private IEnumerator Die()
    {
        currentState = CatState.dying;
        yield return new WaitForSeconds(4f);

        data.RestoreHp();
        data.Save();

        SceneManager.LoadScene("mainMenu", LoadSceneMode.Single);

        yield return null;
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
