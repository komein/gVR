using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResolutionController : MonoBehaviour {

    public int width = 1920;
    public int height = 1080;

    public GvrReticlePointer reticlePrefab;
    public GvrViewer gvrViewerPrefab;

    //public Canvas cardboardCanvas;
    //public Canvas daydreamCanvas;

    public EventSystem cardboardEventSystem;
    public EventSystem daydreamEventSystem;

    public GvrController gvrController;

    public GvrControllerVisualManager gvrArm;


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
                        //MakeCardboardUI();
                        MakeGvrConfiguration();
                        break;
                    }

                case GameManager.VRMode.Daydream:
                    {
                        //MakeDaydreamUI();
                        MakeDaydreamConfiguration();
                        break;
                    }

                case GameManager.VRMode.noVR:
                    {
                        //MakeCardboardUI();
                        MakeMouseGazeConfiguration();
                        break;
                    }

                case GameManager.VRMode.Oculus:
                    {
                        // TODO
                        //MakeCardboardUI();
                        MakeMouseGazeConfiguration();
                        break;
                    }
            }
        }

    }
    /*
    private void MakeCardboardUI()
    {
        //Instantiate(cardboardCanvas);
        Instantiate(cardboardEventSystem);
    }

    private void MakeDaydreamUI()
    {
        //Instantiate(daydreamCanvas);
        Instantiate(daydreamEventSystem);
    }
    */

    private void MakeMouseGazeConfiguration()
    {
        Camera c = Camera.main;
        if (null != c)
        {
            c.gameObject.AddComponent<MoveCameraScript>();
            c.gameObject.AddComponent<PhysicsRaycaster>();
        }
    }

    private EventSystem ResetGvrEventSystem()
    {
        EventSystem es = FindObjectOfType<EventSystem>();
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
            MakeGvrConfiguration();
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

        /*
        */
        MakeGvrConfiguration();
        ResetGvrEventSystem();
    }

    void Start ()
    {
        //if (!DataObjects.gameManager.nonVRMode)
            //Screen.SetResolution(width, height, true);
    }
}
