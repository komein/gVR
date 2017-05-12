using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PauseType { pause, win, gameOver };
public class PlayerController : MonoBehaviour
{
    enum CatState { paused, cantMove, jump, dying, moving };

    CatState CurrentState
    {
        set
        {
            currentState = value;
        }

        get
        {
            return currentState;
        }
    }

    public bool IsJumping;

    CatState currentState;

    protected Rigidbody rb;

    public float maxSpeed = 1f;
    public float acceleration = 50f;
    public float strafeSpeed = 1f;
    public float strafeStep = 0.5f;
    public float dropSpeed = 50f;

    public bool hasCompensatingForce = false;

    GameCanvas gameCanvas;
    WinCanvas winCanvas;

    float curSpeed = 0f;

    Animator anim;

    List<GroundContainer> highGrounds = new List<GroundContainer>();
    List<GroundContainer> planeGrounds = new List<GroundContainer>();

    Coroutine immobilizeCoroutine = null;
    Coroutine flashCoroutine = null;

    SkinnedMeshRenderer mesh;
    Vector3 gravity;

    TextMesh questionMark;

    Vector3 savedMoveVector = Vector3.zero;

    CrushParticle cp;

    GraphicsConfigurator gc;

    Camera cam;

    private void Awake()
    {
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        anim = GetComponent<Animator>();

        rb = GetComponent<Rigidbody>();
        if (null == rb)
        {
            rb = GetComponentInChildren<Rigidbody>();
        }

        cam = Camera.main;

        rb.useGravity = true;

        cp = FindObjectOfType<CrushParticle>();

        questionMark = GetComponentInChildren<TextMesh>();

    }

    private void Start()
    {
        CurrentState = CatState.paused;

        gc = FindObjectOfType<GraphicsConfigurator>();

        gameCanvas = FindObjectOfType<GameCanvas>();
        if (null != gameCanvas)
        {
            gameCanvas.gameObject.SetActive(true);
        }

        winCanvas = FindObjectOfType<WinCanvas>();
        if (null != winCanvas)
        {
            winCanvas.gameObject.SetActive(false);
        }

        StartCoroutine(StartRunning());
    }

    private IEnumerator StartRunning()
    {
        yield return new WaitForSeconds(1);
        CurrentState = CatState.moving;
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

        switch (CurrentState)
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

                pos = GetMoveVector();
                if (pos == Vector3.zero)
                {
                    curSpeed = Mathf.Max(0, curSpeed - Time.fixedDeltaTime * acceleration * 8);
                    if (curSpeed > 0)
                    {
                        pos = savedMoveVector;
                    }
                }
                else
                {
                    savedMoveVector = pos;
                    if (curSpeed < maxSpeed)
                    {
                        curSpeed += Time.fixedDeltaTime * acceleration;
                    }
                }

                if (null != questionMark)
                {
                    questionMark.gameObject.SetActive(pos == Vector3.zero);
                }

                MoveAndRotate(pos);
                SetAnimation(pos);

                return;

        }

    }

    internal void ResumeLevel()
    {
        if (null != DataObjects.GameController)
        {
            DataObjects.GameController.TriggerOptionalScoreAction();

            if (null != gameCanvas && null != winCanvas)
            {
                winCanvas.gameObject.SetActive(false);
                gameCanvas.gameObject.SetActive(true);

                gameCanvas.BroadcastMessage("Reinitialize");
            }

            CurrentState = CatState.moving;
        }

        MultiplierCollectible col = FindObjectOfType<MultiplierCollectible>();
        if (null != col)
        {
            col.isPaused = false;
        }

        GameMusic gm = FindObjectOfType<GameMusic>();
        if (null != gm)
        {
            gm.Play(SceneManager.GetActiveScene().name);
        }
    }


    public void PauseLevel(PauseType reason, LevelInfo p)
    {
        if (CurrentState == CatState.paused)
            return;

        StopAllCoroutines();

        ToggleFlashing(false);

        CurrentState = CatState.paused;

        if (null != gameCanvas && null != winCanvas)
        {
            gameCanvas.gameObject.SetActive(false);
            winCanvas.gameObject.SetActive(true);
            winCanvas.ShowScore(reason, p);
        }
        else
        {
            StartCoroutine(Die());
        }

        MultiplierCollectible col = FindObjectOfType<MultiplierCollectible>();
        if (null != col)
        {
            col.isPaused = true;
        }

        DataObjects.GameController.OnPauseLevel();

        GameMusic gm = FindObjectOfType<GameMusic>();
        if (null != gm)
        {
            switch (reason)
            {
                case PauseType.gameOver:
                    gm.Play("gameOver");
                    break;
                case PauseType.win:
                    gm.Play("victory");
                    break;
                case PauseType.pause:
                    gm.Play("pause");
                    break;
            }
        }
    }

    private void SelectCatState()
    {
        switch (CurrentState)
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
            CurrentState = CatState.jump;
        }
        else
        {
            CurrentState = CatState.moving;
        }
    }

    private void SetGravity()
    {
        switch (CurrentState)
        {
            case CatState.paused:
            case CatState.dying:
            case CatState.cantMove:
            case CatState.jump:
                rb.useGravity = true;
                return;
            case CatState.moving:
                rb.useGravity = false;
                return;
        }
    }

    Vector2 CartesianToPolar(Vector3 point)
    {
        Vector2 polar;
        polar.y = Mathf.Atan2(point.x, point.z);
        float xzLen = new Vector2(point.x, point.z).magnitude;
        polar.x = Mathf.Atan2(-point.y, xzLen);
        polar *= Mathf.Rad2Deg;

        return polar;
    }

    Vector3 PolarToCartesian(Vector2 polar)
    {
        Vector3 origin = new Vector3(0, 0, 1);
        var rotation = Quaternion.Euler(polar.x, polar.y, 0);
        Vector3 point = rotation * origin;

        return point;
    }

    private Vector3 GetMoveVector()
    {
        if (null == gc)
        {
            return Vector3.zero;
        }

#if UNITY_HAS_GOOGLEVR
        GvrLaserPointer laser = gc.Laser;
        GvrReticlePointer reticle = gc.Reticle;

        if (null != laser)
        {
            if (!laser.gameObject.activeInHierarchy)
            {
                if (null != reticle)
                {
                    if (reticle.gameObject.activeInHierarchy)
                    {
                        return GetCameraMoveVector();
                    }
                    else
                    {
                        return Vector3.zero;
                    }
                }
                else
                {
                    return Vector3.zero;
                }
            }

            GameObject ret = laser.reticle;

            if (null == ret)
            {
                return Vector3.zero;
            }

            if (laser.IsPointerIntersecting)
            {
                //Debug.Log(laser.TargetGO.name);
                if (laser.TargetGO.GetComponent<CanvasRenderer>() != null)
                {
                    return Vector3.zero;
                }

                Vector3 gotoPos = ret.transform.position;
                Vector3 pos = Vector3.zero;

                float zPos = gotoPos.z - gameObject.transform.position.z;

                if (zPos < -1)
                {
                    return Vector3.zero;
                }

                float diff = gotoPos.x - gameObject.transform.position.x;

                pos.x = Mathf.Min(Mathf.Abs(diff), 0.4f) * Mathf.Sign(diff);
                pos.x *= strafeSpeed;
                pos.y = 0;
                pos.z = curSpeed;

                return pos;
            }

            return Vector3.zero;
        }
        
        return GetCameraMoveVector();
#endif
    }

    private Vector3 GetCameraMoveVector()
    {
        Vector3 ang = cam.transform.rotation.eulerAngles;

        Vector3 polarCoords = PolarToCartesian(new Vector2(ang.x, ang.y));

        if (polarCoords.y > 0)
        {
            return Vector3.zero;
        }

        Vector3 pos = new Vector3(polarCoords.x + (cam.transform.position.x - gameObject.transform.position.x) / 4f, 0, 0) * strafeSpeed;

        if (Mathf.Abs(pos.x) > strafeStep)
        {
            pos.x = strafeStep * Mathf.Sign(pos.x);
        }

        pos.z = curSpeed + 0.1f;

        return pos;
    }

    private void MoveAndRotate(Vector3 pos)
    {
        if (null != DataObjects.GameController)
        {
            if (DataObjects.GameController.isAlive)
            {
                if (CurrentState == CatState.moving)
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
                    }

                    rb.velocity = rot_ * pos * Time.fixedDeltaTime;

                    if (pos != Vector3.zero && curSpeed > 0)
                    {
                        transform.rotation = Quaternion.Lerp(transform.rotation, rot_ * Quaternion.LookRotation(pos), Time.fixedDeltaTime * 5);
                    }

                    return;
                }
                else if (CurrentState == CatState.jump)
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

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.collider.name);
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Obstacle>() != null)
        {
            other.gameObject.SetActive(false);
        }
        else if (other.GetComponent<DummyCollider>() != null)
        {
            other.GetComponent<DummyCollider>().Trigger();
        }
        else if (other.gameObject.GetComponent<HighGround>() != null)
        {
            GroundContainer cont = other.GetComponent<GroundContainer>();
            if (null == cont)
            {
                cont = other.GetComponentInParent<GroundContainer>();
            }
            if (null != cont)
            {
                highGrounds.Add(cont);
            }
        }
        else if (other.gameObject.GetComponent<PlaneGround>() != null)
        {
            if (null != immobilizeCoroutine)
            {
                StopCoroutine(immobilizeCoroutine);

                if (CurrentState == CatState.paused)
                {
                    // do nothing
                }
                else
                {
                    // start moving
                    CurrentState = CatState.moving;
                }

                immobilizeCoroutine = null;
            }

            GroundContainer cont = other.GetComponent<GroundContainer>();
            if (null == cont)
            {
                cont = other.GetComponentInParent<GroundContainer>();
            }
            if (null != cont)
            {
                if (!planeGrounds.Contains(cont))
                {
                    planeGrounds.Add(cont);
                }
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        GroundContainer cont = other.GetComponent<GroundContainer>();
        if (null == cont)
        {
            cont = other.GetComponentInParent<GroundContainer>();
        }

        if (other.gameObject.GetComponent<HighGround>() != null)
        {
            highGrounds.RemoveAll(p => p == cont);
            if (highGrounds.Count == 0)
            {
                if (currentState == CatState.moving)
                {
                    rb.velocity = new Vector3(0, 0, rb.velocity.z);
                    Jump();
                }
            }
        }
        else if (other.gameObject.GetComponent<PlaneGround>() != null)
        {
            planeGrounds.RemoveAll(p => p == cont);
        }
    }

    public void MakeCrush(Vector3 v)
    {
        if (null != cp)
        {
            ParticleSystem cm = cp.GetComponent<ParticleSystem>();
            if (null != cm)
            {
                cm.transform.position = transform.position + new Vector3(0, 0.3f, 0);
                cm.Play();
            }
        }

        if (DataObjects.GameController != null)
        {
            TakeDamage();

            if (!DataObjects.GameController.isAlive)
            {
                DataObjects.GameManager.PauseLevel(PauseType.gameOver);
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
        {
            curSpeed = 0;
        }
    }

    public void TakeDamage()
    {
        if (null != DataObjects.GameController)
        {
            DataObjects.GameController.LoseHp(1);
        }
    }

    public void ToggleFlashing(bool v)
    {
        if (v == true)
        {
            flashCoroutine = StartCoroutine(Flash());
        }
        else
        {
            if (null != flashCoroutine)
            {
                StopCoroutine(flashCoroutine);
                flashCoroutine = null;
                mesh.enabled = true;
            }
        }
    }

    private IEnumerator ImmobilizeCoroutine()
    {
        CurrentState = CatState.cantMove;
        yield return new WaitForSeconds(1.5f);
        CurrentState = CatState.moving;

        yield return null;
    }

    private IEnumerator JumpCoroutine()
    {
        CurrentState = CatState.jump;

        yield return new WaitForSeconds(5f);

        CurrentState = CatState.moving;

        yield return null;
    }

    private IEnumerator Die()
    {
        CurrentState = CatState.dying;

        yield return new WaitForSeconds(4f);

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
