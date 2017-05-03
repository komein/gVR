using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GraphicsConfigurator : MonoBehaviour
{
    public static GraphicsConfigurator instanceRef; // singleton pattern

    public GoogleVRManager gvrManager;

    public GvrReticlePointer reticlePrefab;
    public GvrReticlePointer Reticle
    {
        get;
        private set;
    }

    public GvrController gvrController;
    GvrController gc;

    public GvrControllerVisualManager gvrArm;
    GvrControllerVisualManager arm;

    public EventSystem eventSystemPrefab;
    EventSystem es;

    public GvrLaserPointer Laser
    {
        get;
        private set;
    }

    GvrPointerInputModule gim;

    bool controllerIsEnabled = false;
    int counter = 0;

    private void OnApplicationQuit()
    {
        Destroy(gameObject);
    }

    internal void Initialize()
    {
        if (null != FindObjectOfType<GraphicsOverrider>())
        {
            return;
        }

        ResetEverything();

        if (null == DataObjects.GameManager)
        {
            MakeMouseGazeConfiguration(true);
        }
        else
        {
            switch (DataObjects.GameManager.mode)
            {
                case GameManager.VRMode.Cardboard:
                    {
                        MakeGoogleVRConfiguration(false);
                        break;
                    }

                case GameManager.VRMode.Daydream:
                    {
                        MakeGoogleVRConfiguration(true);
                        break;
                    }

                case GameManager.VRMode.Oculus:
                    {
                        // TODO
                        MakeMouseGazeConfiguration(true);
                        break;
                    }

                case GameManager.VRMode.noVR:
                    {
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

            DontDestroyOnLoad(gameObject);
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
    }

    private static void ResetCamera()
    {
        Camera c = Camera.main;
        if (null != c)
        {
            Destroy(c.gameObject.GetComponent<MoveCamera>());
            Destroy(c.gameObject.GetComponent<PhysicsRaycaster>());
            Destroy(c.gameObject.GetComponent<GvrHead>());
            Destroy(c.gameObject.GetComponent<StereoController>());
        }
    }

    private void MakeMouseGazeConfiguration(bool withRaycaster) // no physics raycaster is needed in daydream controller case
    {
        Camera c = Camera.main;
        if (null != c)
        {
            if (null == c.gameObject.GetComponent<MoveCamera>())
            {
                c.gameObject.AddComponent<MoveCamera>();
            }
            if (withRaycaster)
            {
                c.gameObject.AddComponent<PhysicsRaycaster>();
            }
        }
    }

    private EventSystem GetEventSystem()
    {
        if (null == es)
        {
            es = FindObjectOfType<EventSystem>();
            if (null == es)
            {
                es = Instantiate(eventSystemPrefab);
            }
        }
        return es;
    }

    private EventSystem MakeGoogleVREventSystem()
    {
        es = GetEventSystem();
        if (null != es)
        {
            if (null == (gim = es.GetComponent<GvrPointerInputModule>()))
            {
                gim = es.gameObject.AddComponent<GvrPointerInputModule>();
            }
            if (null == es.GetComponent<GvrPointerManager>())
            {
                es.gameObject.AddComponent<GvrPointerManager>();
            }
        }

        return es;
    }

    private void MakeGoogleVRStereoConfiguration()
    {
        Camera c = Camera.main;
        if (null != c)
        {
            c.gameObject.AddComponent<GvrHead>();
            c.gameObject.AddComponent<StereoController>();
        }
    }

#if UNITY_HAS_GOOGLEVR
    private void Update()
    {
        if (!controllerIsEnabled) // switching to daydream controller ONCE, no going back then.
        {
            if (DataObjects.GameManager.mode == GameManager.VRMode.Daydream)
            {
                if (GvrController.State == GvrConnectionState.Connected)
                {
                    if (counter++ >= 10)
                    {
                        SetGvrInput(true);
                    }
                }
                else
                {
                    counter = 0;
                }
            }
        }
    }
#endif

    private void MakeGoogleVRConfiguration(bool withController)
    {
        MakeGoogleVRCameraConfiguration();
        MakeGoogleVREventSystem();

        if (null == gc)
        {
            gc = FindObjectOfType<GvrController>();
            if (null == gc)
            {
                gc = Instantiate(gvrController);
                DontDestroyOnLoad(gc);
            }
        }

        SetGvrInput(false);
    }

    public void SetGvrInput(bool withController)
    {
        controllerIsEnabled = withController;

        if (withController)
        {
#if UNITY_HAS_GOOGLEVR
            ShowGvrLaserPointer();
#else
            ShowGazePointer();
#endif
        }
        else
        {
            ShowGvrReticlePointer();
        }
    }

    private void ShowGvrReticlePointer()
    {
        if (null == Reticle)
        {
            Reticle = GetGvrReticlePointer();
        }

        if (null != Reticle && null != gim)
        {
            Reticle.gameObject.SetActive(true);
            gim.DeactivateModule();
            Reticle.SetAsMainPointer();
            gim.ShouldActivateModule();
        }
    }

    private void MakeGoogleVRCameraConfiguration()
    {
        if (!GameManager.StereoMode)
        {
            MakeMouseGazeConfiguration(false);
        }
        else
        {
            MakeGoogleVRStereoConfiguration();
        }
    }

    private GvrReticlePointer GetGvrReticlePointer()
    {
        if (null != Reticle)
        {
            return Reticle;
        }

        GvrReticlePointer toReturn = FindObjectOfType<GvrReticlePointer>();
        if (null == toReturn)
        {
            //Debug.LogWarning("No reticle is attached to the main camera. Instantiating from prefab..");
            toReturn = Instantiate(reticlePrefab);
            toReturn.transform.SetParent(Camera.main.transform);
            toReturn.transform.localPosition = Vector3.zero;
            toReturn.transform.localRotation = Quaternion.identity;
        }

        return toReturn;
    }

#if UNITY_HAS_GOOGLEVR

    private GvrLaserPointer GetGvrLaserPointer()
    {
        if (null != Laser)
        {
            return Laser;
        }

        GvrLaserPointer toReturn = null;
        if (null == arm)
        {
            arm = FindObjectOfType<GvrControllerVisualManager>();
            if (null == arm)
            {
                Player p = FindObjectOfType<Player>();
                if (null == p)
                {
                    return toReturn;
                }

                arm = Instantiate(gvrArm);
                arm.transform.SetParent(p.transform);
                arm.transform.localPosition = Vector3.zero;
            }
        }

        if (null != arm)
        {
            toReturn = arm.GetComponentInChildren<GvrLaserPointer>();
        }

        return toReturn;
    }

    public void ShowGvrLaserPointer()
    {
        if (null == Laser)
        {
            Laser = GetGvrLaserPointer();
        }

        if (null != Laser && null != gim)
        {
            if (null != Reticle)
            {
                Reticle.gameObject.SetActive(false);
            }

            gim.DeactivateModule();
            Laser.SetAsMainPointer();
            gim.ShouldActivateModule();
        }
    }

#endif

}
