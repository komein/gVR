using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GraphicsConfigurator : MonoBehaviour
{
    public static GraphicsConfigurator instanceRef; // singleton pattern

    // deprecated
    public int width = 1920;
    // deprecated
    public int height = 1080;

    public GvrReticlePointer reticlePrefab;
    public GvrViewer gvrViewerPrefab;
    public GvrController gvrController;
    public GvrControllerVisualManager gvrArm;

    public EventSystem eventSystemPrefab;
    GvrReticlePointer ret;

    internal void Initialize()
    {
        ResetEverything();

        if (null == DataObjects.gameManager)
        {
            MakeMouseGazeConfiguration(true);
        }
        else
        {
            switch (DataObjects.gameManager.mode)
            {
                case GameManager.VRMode.Cardboard:
                    {
                        MakeGvrStereoConfiguration();
                        break;
                    }

                case GameManager.VRMode.Daydream:
                    {
                        MakeDaydreamConfiguration();
                        break;
                    }

                case GameManager.VRMode.noVR:
                    {
                        MakeMouseGazeConfiguration(true);
                        break;
                    }

                case GameManager.VRMode.Oculus:
                    {
                        // TODO
                        MakeMouseGazeConfiguration(true);
                        break;
                    }
            }
        }
    }

    private void Awake()
    {
        if (null == instanceRef)
        {
            instanceRef = this;
            DontDestroyOnLoad(this.gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void ResetEverything()
    {
        ResetCamera();
        ResetEventSystem();
        ResetController();
    }

    private static void ResetCamera()
    {
        Camera c = Camera.main;
        if (null != c)
        {
            Destroy(c.gameObject.GetComponent<MoveCameraScript>());
            Destroy(c.gameObject.GetComponent<PhysicsRaycaster>());
            Destroy(c.gameObject.GetComponent<GvrHead>());
            Destroy(c.gameObject.GetComponent<StereoController>());
        }
    }

    private void ResetEventSystem()
    {
        EventSystem es = GetOrCreateEventSystem();
        if (null != es)
        {
            Destroy(es.gameObject.GetComponent<GvrPointerInputModule>());
            Destroy(es.gameObject.GetComponent<GvrPointerManager>());
        }
    }

    private void MakeMouseGazeConfiguration(bool withRaycaster)
    {
        Camera c = Camera.main;
        if (null != c)
        {
            MoveCameraScript mc = c.gameObject.GetComponent<MoveCameraScript>();
            Destroy(mc);

            c.gameObject.AddComponent<MoveCameraScript>();
            if (withRaycaster)
                c.gameObject.AddComponent<PhysicsRaycaster>();
        }

        MakeGvrEventSystem();
    }

    private EventSystem GetOrCreateEventSystem()
    {
        EventSystem es = FindObjectOfType<EventSystem>();
        if (null == es)
        {
            es = new GameObject().AddComponent<EventSystem>();
            es.name = "EventSystem";
        }
        return es;
    }

    private EventSystem MakeGvrEventSystem()
    {
        ResetEventSystem();
        EventSystem es = GetOrCreateEventSystem();
        if (null != es)
        {
            es.gameObject.AddComponent<GvrPointerInputModule>();
            es.gameObject.AddComponent<GvrPointerManager>();
        }

        return es;
    }

    private void MakeGvrStereoConfiguration()
    {
        EventSystem es = MakeGvrEventSystem();

        Camera c = Camera.main;

        c.gameObject.AddComponent<GvrHead>();
        c.gameObject.AddComponent<StereoController>();
    }

    private void MakeDaydreamConfiguration()
    {
        // if we don't have a Player object, the scene is just isn't configured correctly to be used with daydream
        Player p = FindObjectOfType<Player>();
        if (null == p)
        {
            MakeGvrStereoConfiguration();
            return;
        }
        //

        if (DataObjects.gameManager.noStereoMode)
        {
            MakeMouseGazeConfiguration(false);
        }
        else
        {
            MakeGvrStereoConfiguration();
        }


        GvrController gc = FindObjectOfType<GvrController>();
        if (null == gc)
        {
            gc = Instantiate(gvrController);
            DontDestroyOnLoad(gc);
        }

        ReinitControllerState();
    }

    public void ReinitControllerState()
    {
        Player p = FindObjectOfType<Player>();
        if (null == p)
        {
            return;
        }

        ResetController();

        // controller is found
        if (DataObjects.gameManager.controllerState == GvrConnectionState.Connected)
        {
            if (null != ret)
            {
                ret.gameObject.SetActive(false);
            }

            GvrControllerVisualManager arm = Instantiate(gvrArm);

            arm.transform.SetParent(p.transform);
            arm.transform.localPosition = Vector3.zero;

            GvrLaserPointer pointer = FindObjectOfType<GvrLaserPointer>();

            if (null != pointer)
            {
                pointer.SetAsMainPointer();
            }

            GvrPointerInputModule inputModule = FindObjectOfType<GvrPointerInputModule>();
            if (null != inputModule)
            {
                inputModule.Process();
            }

        }
        // controller is not found
        else
        {
            if (null == ret)
            {
                ret = Instantiate(reticlePrefab);
                ret.transform.SetParent(Camera.main.transform);
                ret.transform.localPosition = Vector3.zero;
                ret.transform.localRotation = Quaternion.identity;
            }
            else
            {
                ret.gameObject.SetActive(true);
            }

            ret.SetAsMainPointer();

        }
    }

    private static void ResetController()
    {/*
        GvrReticlePointer ret = FindObjectOfType<GvrReticlePointer>();
        if (null != ret)
        {
            Destroy(ret.gameObject);
        }*/
        GvrControllerVisualManager arm = FindObjectOfType<GvrControllerVisualManager>();
        if (null != arm)
        {
            Destroy(arm.gameObject);
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
