using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterPrototype2 : MonoBehaviour
{
    enum CatState { cantMove, dying, moving };
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

    int count = 0;
    int enableCounter = 25;

    float curSpeed = 0f;

    CharacterController ch;
    Animator anim;

    List<Collider> grounds = new List<Collider>();
    List<Collider> planeGrounds = new List<Collider>();

    SkinnedMeshRenderer mesh;

    DataStorage data;

    Coroutine c;

    

    private void Start()
    {
        currentState = CatState.moving;

        data = FindObjectOfType<DataStorage>();
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

        switch(currentState)
        {
            case CatState.dying:
            case CatState.cantMove:
                SetIdleAnimation();
                return;
            case CatState.moving:
                if (curSpeed < maxSpeed)
                {
                    curSpeed += Time.deltaTime * acceleration;
                }
                Vector3 pos = GetMoveVector();
                SetAnimation(pos);
                MoveAndRotate(pos);
                break;
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
            if (data.isAlive && currentState == CatState.moving)
            {
                ch.SimpleMove(pos * Time.fixedDeltaTime);

                Quaternion rot_ = planeGrounds.Count != 0 || grounds.Count == 0 ? Quaternion.identity : grounds[grounds.Count - 1].transform.rotation;

                if (pos != Vector3.zero)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, rot_ * Quaternion.LookRotation(pos), Time.fixedDeltaTime * 20);
                }

                return;
            }
        }

        ch.SimpleMove(Vector3.zero);
    }

    private void SetAnimation(Vector3 pos)
    {
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
    }

    private void SetIdleAnimation()
    {
        if (null != anim)
        {
            anim.SetBool("jumping", false);
            anim.SetFloat("speed", 0f);
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Obstacle>() != null)
        {
            other.gameObject.SetActive(false);
        }
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
                StartCoroutine(Immobilize());
            }

            MakeKnockBack(v);
        }

    }

    private void MakeKnockBack(Vector3 v)
    {
        v = v.normalized * 2;
        v.y = 2;

        rb.AddForce(v, ForceMode.Impulse);

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

    private IEnumerator Immobilize()
    {
        currentState = CatState.cantMove;
        ch.enabled = false;
        yield return new WaitForSeconds(1f);
        ch.enabled = true;
        currentState = CatState.moving;

        yield return null;
    }

    private IEnumerator Die()
    {
        currentState = CatState.dying;
        ch.enabled = false;
        yield return new WaitForSeconds(4f);

        data.RestoreHp();
        data.Save();

        SceneManager.LoadScene("prototypeSelectScene", LoadSceneMode.Single);

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
