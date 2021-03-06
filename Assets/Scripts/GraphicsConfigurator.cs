﻿using UnityEngine;
using UnityEngine.EventSystems;

public class GraphicsConfigurator : MonoBehaviour
{
    public static GraphicsConfigurator instanceRef; // singleton pattern

    
    private GvrReticlePointer reticle;
    public GvrReticlePointer Reticle
    {
        get
        {
            return reticle;
        }
    }
    EventSystem es;

#if UNITY_HAS_GOOGLEVR
    private GvrLaserPointer laser; 
    public GvrLaserPointer Laser
    {
        get
        {
            return laser;
        }
    }

    GvrController gvrController;
    
#endif
#if OCULUS_STUFF
    OVRInputModule inputModule;
    OVRCameraRig cameraRig;
#endif
    bool shouldHandleExitButton = false;

    internal void Initialize()
    {
        if (null != FindObjectOfType<GraphicsOverrider>())
        {
            return;
        }

        if (null == DataObjects.GameManager)
        {
            MakeMouseGazeConfiguration(Camera.main.gameObject);
        }
        else
        {
            switch (DataObjects.GameManager.mode)
            {
                case GameManager.VRMode.GoogleVR_Daydream:
                    {
                        MakeGoogleVRConfiguration(true);
                        break;
                    }

                case GameManager.VRMode.GoogleVR_Cardboard:
                    {
                        MakeGoogleVRConfiguration(false);
                        break;
                    }

                case GameManager.VRMode.Oculus:
                    {
#if OCULUS_STUFF
                        MakeOculusConfiguration();
#endif
                        break;
                    }

                case GameManager.VRMode.none:
                    {
                        MakeMouseGazeConfiguration(Camera.main.gameObject);
                        break;
                    }
            }
        }

#if UNITY_EDITOR
        QualitySettings.antiAliasing = 0;
#endif
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

    private void Update()
    {
#if OCULUS_STUFF
        if (null != inputModule && null != cameraRig)
        {
            inputModule.rayTransform = OVRGazePointer.instance.rayTransform =
                (OVRInput.GetActiveController() == OVRInput.Controller.Touch) ? cameraRig.rightHandAnchor :
                (OVRInput.GetActiveController() == OVRInput.Controller.RTouch) ? cameraRig.rightHandAnchor :
                (OVRInput.GetActiveController() == OVRInput.Controller.LTouch) ? cameraRig.leftHandAnchor :
                cameraRig.centerEyeAnchor;
        }
#endif
    }
    
    void LateUpdate()
    {
        if (shouldHandleExitButton)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
    }
    
    private void MakeMouseGazeConfiguration(GameObject c)
    {
        if (null != c)
        {
            if (null == c.gameObject.GetComponent<MoveCamera>())
            {
                c.gameObject.AddComponent<MoveCamera>();
            }
            if (null == c.gameObject.GetComponent<PhysicsRaycaster>())
            {
                c.gameObject.AddComponent<PhysicsRaycaster>();
            }
        }

        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            Debug.Log("Creating EventSystem");
            EventSystem eventSystemPrefab = (EventSystem)Resources.Load("EventSystem_noVR", typeof(EventSystem));
            eventSystem = Instantiate(eventSystemPrefab);
        }
    }

#if OCULUS_STUFF
    void EntitlementCheck(Oculus.Platform.Message msg)
    {
        if (!msg.IsError)
        {
            // we are ok to play
        }
        else
        {
            // sudden gracious (lol no) exit without any explanations
            Application.Quit();
        }
    }
    
    private void MakeOculusConfiguration()
    {
        if (FindObjectOfType<OVRCameraRig>() != null)
        {
            return;
        }

        if (Camera.main != null)
        {
            Vector3 cameraPos = Camera.main.transform.position;
            Transform cameraParent = Camera.main.transform.parent;
            Camera.main.gameObject.SetActive(false);
#if UNITY_HAS_GOOGLEVR
            GvrControllerVisualManager gvrManager = FindObjectOfType<GvrControllerVisualManager>();

            if (null != gvrManager)
            {
                gvrManager.gameObject.SetActive(false);
            }
#endif

            cameraRig = Instantiate((OVRCameraRig)Resources.Load("OVRCameraRig", typeof(OVRCameraRig)));
            if (null != cameraRig)
            {
                cameraRig.transform.position = cameraPos;
                cameraRig.transform.SetParent(cameraParent, true);
                cameraRig.EnsureGameObjectIntegrity();

                EventSystem eventSystem = FindObjectOfType<EventSystem>();
                if (eventSystem == null)
                {
                    Debug.Log("Creating EventSystem");
                    EventSystem eventSystemPrefab = (EventSystem)Resources.Load("Prefabs/EventSystem", typeof(EventSystem));
                    eventSystem = Instantiate(eventSystemPrefab);
                }
                else
                {
                    if (eventSystem.GetComponent<OVRInputModule>() == null)
                    {
                        eventSystem.gameObject.AddComponent<OVRInputModule>();
                    }
                }

                inputModule = eventSystem.GetComponent<OVRInputModule>();

                Canvas[] canvases = FindObjectsOfType<Canvas>();
                foreach (var v in canvases)
                {
                    v.worldCamera = cameraRig.leftEyeCamera;
                }

#if UNITY_EDITOR
                MakeMouseGazeConfiguration(cameraRig.gameObject);
#endif
            
        }

        if (null != FindObjectOfType<OVRPlatformMenu>())
        {
            return;
        }}

        Oculus.Platform.Core.AsyncInitialize("1523835387640661");
        Oculus.Platform.Entitlements.IsUserEntitledToApplication().OnComplete(EntitlementCheck);

        OVRPlugin.cpuLevel = 1;
        OVRPlugin.gpuLevel = 3;

        // this is to handle the exiting just like Oculus guys want
        shouldHandleExitButton = false;

        OVRPlatformMenu platformMenu = new GameObject().AddComponent<OVRPlatformMenu>();
        DontDestroyOnLoad(platformMenu.gameObject);

        platformMenu.enabled = true;
        platformMenu.gameObject.name = "OculusReservedButtonsHandler";

    }
#endif
    
    private void MakeGoogleVRConfiguration(bool isDaydream)
    {
        Instantiate(Resources.Load("GvrEventSystem") as GameObject);
        reticle = FindObjectOfType<GvrReticlePointer>();
#if UNITY_HAS_GOOGLEVR

        if (null == gvrController)
        {
            GameObject go = Instantiate(Resources.Load("GvrControllerMain") as GameObject);
            gvrController = go.GetComponent<GvrController>();
            if (null != gvrController)
            {
                DontDestroyOnLoad(gvrController.gameObject);
            }
        }

        if (isDaydream)
        {
            laser = FindObjectOfType<GvrLaserPointer>();
        }

        string s = isDaydream ? "_daydream" : "_cardboard";

        GameObject manager = Instantiate(Resources.Load("DemoInputManager" + s) as GameObject);
        if (null != manager)
        {
            Vector3 pos = manager.transform.position;

            Player p = FindObjectOfType<Player>();

            if (null != p)
            {
                manager.transform.SetParent(p.transform, true);
            }
        }

        shouldHandleExitButton = true;

#if UNITY_EDITOR
        MakeMouseGazeConfiguration(Camera.main.gameObject);
#endif
#else
        MakeMouseGazeConfiguration(Camera.main.gameObject);
#endif

    }

}
