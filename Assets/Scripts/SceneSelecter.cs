using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class SceneSelecter : MonoBehaviour, IGvrGazeResponder
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

    void LateUpdate()
    {
        GvrViewer.Instance.UpdateState();
        if (GvrViewer.Instance.BackButtonPressed)
        {
            Application.Quit();
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

    public void ToggleVRMode()
    {
        GvrViewer.Instance.VRModeEnabled = !GvrViewer.Instance.VRModeEnabled;
    }

    public void ToggleDistortionCorrection()
    {
        GvrViewer.Instance.DistortionCorrectionEnabled =
          !GvrViewer.Instance.DistortionCorrectionEnabled;
    }

#if !UNITY_HAS_GOOGLEVR || UNITY_EDITOR
    public void ToggleDirectRender()
    {
        GvrViewer.Controller.directRender = !GvrViewer.Controller.directRender;
    }
#endif

    protected virtual void Function()
    {
        DataStorage scoreStorage = FindObjectOfType<DataStorage>();
        if (null != scoreStorage)
        {
            scoreStorage.Save();
        }

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
