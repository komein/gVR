using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GraphicsConfigurator : MonoBehaviour
{

    // deprecated
    public int width = 1920;
    // deprecated
    public int height = 1080;

    public GvrReticlePointer reticlePrefab;
    public GvrViewer gvrViewerPrefab;
    public GvrController gvrController;
    public GvrControllerVisualManager gvrArm;

    public EventSystem eventSystemPrefab;

    private void Awake()
    {
        if (null == DataObjects.gameManager)
        {
            // default
            MakeMouseGazeConfiguration();
        }
        else
        {
            switch (DataObjects.gameManager.mode)
            {
                case GameManager.VRMode.Cardboard:
                    {
                        MakeGvrConfiguration();
                        break;
                    }

                case GameManager.VRMode.Daydream:
                    {
                        MakeDaydreamConfiguration();
                        break;
                    }

                case GameManager.VRMode.noVR:
                    {
                        MakeMouseGazeConfiguration();
                        break;
                    }

                case GameManager.VRMode.Oculus:
                    {
                        // TODO
                        MakeMouseGazeConfiguration();
                        break;
                    }
            }
        }

    }

    private void MakeMouseGazeConfiguration()
    {
        Camera c = Camera.main;
        if (null != c)
        {
            c.gameObject.AddComponent<MoveCameraScript>();
            c.gameObject.AddComponent<PhysicsRaycaster>();
        }
        ResetGvrEventSystem();
    }

    private EventSystem GetEventSystem()
    {
        EventSystem es = FindObjectOfType<EventSystem>();
        if (null == es)
        {
            es = Instantiate(eventSystemPrefab);
        }
        return es;
    }

    private EventSystem ResetGvrEventSystem()
    {
        EventSystem es = GetEventSystem();
        if (null != es)
        {
            es.gameObject.AddComponent<GvrPointerInputModule>();
            es.gameObject.AddComponent<GvrPointerManager>();
        }

        return es;
    }

    private void MakeGvrConfiguration()
    {
        EventSystem es = ResetGvrEventSystem();

        Camera c = Camera.main;

        c.gameObject.AddComponent<GvrHead>();
        c.gameObject.AddComponent<StereoController>();
    }

    private void MakeDaydreamConfiguration()
    {
        Player p = FindObjectOfType<Player>();

        if (null == p)
        {
            MakeMouseGazeConfiguration();
            return;
        }

        GvrReticlePointer ret = FindObjectOfType<GvrReticlePointer>();
        if (null != ret)
        {
            Destroy(ret.gameObject);
        }

        GvrController gc = Instantiate(gvrController);

        GvrControllerVisualManager arm = Instantiate(gvrArm);

        arm.transform.SetParent(p.transform);
        arm.transform.localPosition = Vector3.zero;

        if (DataObjects.gameManager.noStereoMode)
        {
            MakeMouseGazeConfiguration();
        }
        else
        {
            MakeGvrConfiguration();
        }
    }

    /*
     * DEPRECATED:
     * Do not use with Daydream, everything fucks up
     * ( tested on GoogleVR v1.0 + Unity 5.6 beta )
    void Start()
    {
        Screen.SetResolution(width, height, true);
    }
     */
}
