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
    GvrReticlePointer ret;

    public GvrController gvrController;
    GvrController gc;

    public GvrControllerVisualManager gvrArm;
    GvrControllerVisualManager arm;

    public EventSystem eventSystemPrefab;
    EventSystem es;

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

            //gvrManager = new GoogleVRManager();
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
        ResetEventSystem();
        ToggleGoogleVRGazePointer(false);
#if UNITY_HAS_GOOGLEVR
        DeleteGoogleVRController();
#endif
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

    private void ResetEventSystem()
    {
        es = GetEventSystem();
        if (null != es)
        {
            Destroy(es.gameObject.GetComponent<GvrPointerInputModule>());
            Destroy(es.gameObject.GetComponent<GvrPointerManager>());
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
            if (null == es.GetComponent<GvrPointerInputModule>())
            {
                es.gameObject.AddComponent<GvrPointerInputModule>();
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

    private void MakeGoogleVRConfiguration(bool withController)
    {
        MakeGoogleVRCameraConfiguration();
        MakeGoogleVREventSystem();

        if (withController)
        {
#if UNITY_HAS_GOOGLEVR
            ReinitGoogleVRController();
#else
            ToggleGoogleVRGazePointer(true);
#endif
        }
		else
		{
			ToggleGoogleVRGazePointer(true);
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

    private void ToggleGoogleVRGazePointer(bool isVisible)
    {
        ret = GetGoogleVRGazePointer();

        if (null != ret)
        {
            ret.gameObject.SetActive(isVisible);
            if (isVisible)
            {
                ret.SetAsMainPointer();
            }
        }
    }

    private GvrReticlePointer GetGoogleVRGazePointer()
    {
        if (null != ret)
        {
            return ret;
        }

        GvrReticlePointer toReturn = FindObjectOfType<GvrReticlePointer>();
        if (null == toReturn)
        {
            Debug.LogWarning("No reticle is attached to the main camera. Instantiating from prefab..");
            toReturn = Instantiate(reticlePrefab);
            toReturn.transform.SetParent(Camera.main.transform);
            toReturn.transform.localPosition = Vector3.zero;
            toReturn.transform.localRotation = Quaternion.identity;
        }

        return toReturn;
    }

#if UNITY_HAS_GOOGLEVR
    public void ReinitGoogleVRController()
    {
        // no parent object to put controller into, scene is not configured properly
        Player p = FindObjectOfType<Player>();
        if (null == p)
        {
            return;
        }

        if (null == gc)
        {
            gc = FindObjectOfType<GvrController>();
            if (null == gc)
            {
                gc = Instantiate(gvrController);
                DontDestroyOnLoad(gc);
            }
        }

        DeleteGoogleVRController();
        // controller is found
        if (DataObjects.GameManager.controllerState == GvrConnectionState.Connected)
        {
            ToggleGoogleVRGazePointer(false);
            InitializeGoogleVRController(p);
        }
        // controller is not found
        else
        {
            ToggleGoogleVRGazePointer(true);
        }
    }

    private void InitializeGoogleVRController(Player p)
    {
        // make actual controller
        arm = Instantiate(gvrArm);
        arm.transform.SetParent(p.transform);
        arm.transform.localPosition = Vector3.zero;

        GvrLaserPointer pointer = FindObjectOfType<GvrLaserPointer>();
        if (null != pointer)
        {
            pointer.SetAsMainPointer();
        }
    }

    private void DeleteGoogleVRController()
    {
        if (null != arm)
        {
            Destroy(arm.gameObject);
        }
    }
#endif

}
