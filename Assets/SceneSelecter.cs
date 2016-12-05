using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class SceneSelecter : MonoBehaviour, IGvrGazeResponder
{
    private Vector3 startingPosition;

    bool isGazedOn = false;

    public string scenePath = "";

    float time = 0f;
    const float TELEPORT_TIME = 1.2f;

    Color GAZED_COLOR = Color.green;
    Color UNGAZED_COLOR = Color.red;

    void Start()
    {
        startingPosition = transform.localPosition;
        SetGazedAt(false);
        isGazedOn = false;
    }

    private void Update()
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

    public void SetGazedAt(bool gazedAt)
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
#endif  //  !UNITY_HAS_GOOGLEVR || UNITY_EDITOR

    protected virtual void Function()
    {
        Debug.Log("loading " + scenePath);
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
