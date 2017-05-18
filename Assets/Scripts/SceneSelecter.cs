using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class SceneSelecter : MonoBehaviour
{
    protected Vector3 startingPosition;

    protected bool isGazedOn = false;

    public string scenePath = "";

    protected float time = 0f;
    protected const float TELEPORT_TIME = 1.2f;

    protected Color GAZED_COLOR = Color.green;
    protected Color UNGAZED_COLOR = Color.red;

    void Start()
    {
        startingPosition = transform.localPosition;
        SetGazedAt(false);
        isGazedOn = false;
    }

    protected virtual void Update()
    {
        if (isGazedOn)
        {
            time += Time.deltaTime;
            GetComponent<Renderer>().material.color = GAZED_COLOR - new Color(0, time / TELEPORT_TIME, 0);
        }
        else
        {
            time = 0f;
        }

        if (time >= TELEPORT_TIME)
        {
            Function();
        }
    }

    public virtual void SetGazedAt(bool gazedAt)
    {
        isGazedOn = gazedAt;
        GetComponent<Renderer>().material.color = isGazedOn ? GAZED_COLOR : UNGAZED_COLOR;
    }

    public void Reset()
    {
        transform.localPosition = startingPosition;
    }

    protected virtual void Function()
    {
        SceneManager.LoadScene(scenePath, LoadSceneMode.Single);
    }

    public void OnGazeEnter()
    {
        SetGazedAt(true);
    }

    public void OnGazeExit()
    {
        SetGazedAt(false);
    }

    public void OnGazeTrigger()
    {
        Function();
    }
}
