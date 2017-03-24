using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResolutionController : MonoBehaviour {

    public int width = 1920;
    public int height = 1080;

    public GvrReticlePointer reticlePrefab;
    public GvrViewer gvrViewerPrefab;

    private void Awake()
    {
        if (null == DataObjects.gameManager)
        {
            // default
            MakeMouseGazeConfiguration();
        }
        else if (DataObjects.gameManager.nonVRMode)
        {
            MakeMouseGazeConfiguration();
        }
        else
        {
            MakeGvrConfiguration();
        }
    }

    private static void MakeMouseGazeConfiguration()
    {
        Camera c = Camera.main;
        if (null != c)
        {
            c.gameObject.AddComponent<MoveCameraScript>();
            c.gameObject.AddComponent<PhysicsRaycaster>();
        }
    }

    private static void MakeGvrConfiguration()
    {
        Camera c = Camera.main;

        c.gameObject.AddComponent<GvrHead>();
        c.gameObject.AddComponent<StereoController>();

        EventSystem es = FindObjectOfType<EventSystem>();
        if (null != es)
        {
            es.gameObject.AddComponent<GvrPointerInputModule>();
            es.gameObject.AddComponent<GvrPointerManager>();
        }
    }

    void Start ()
    {
        //if (!DataObjects.gameManager.nonVRMode)
        //Screen.SetResolution(width, height, true);
    }
}
