using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    private GvrLaserPointer laser;
    public GvrLaserPointer Laser
    {
        get
        {
            return laser;
        }
    }

    OVRInputModule inputModule;
    OVRCameraRig cameraRig;

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
                        // TODO
                        MakeOculusConfiguration();
                        break;
                    }

                case GameManager.VRMode.none:
                    {
                        MakeMouseGazeConfiguration(Camera.main.gameObject);
                        break;
                    }
            }
        }
    }

    private void MakeOculusConfiguration()
    {
        if (FindObjectOfType<OVRCameraRig>() != null)
        {
            return;
        }

        Vector3 cameraPos = Camera.main.transform.position;
        Transform cameraParent = Camera.main.transform.parent;
        Camera.main.gameObject.SetActive(false);

        GvrControllerVisualManager gvrManager = FindObjectOfType<GvrControllerVisualManager>();

        if (null != gvrManager)
        {
            gvrManager.gameObject.SetActive(false);
        }

        cameraRig = Instantiate((OVRCameraRig)Resources.Load("OVRCameraRig", typeof(OVRCameraRig)));

        if (null != cameraRig)
        {
            cameraRig.transform.position = cameraPos;
            cameraRig.transform.SetParent(cameraParent, true);

            EventSystem eventSystem = GameObject.FindObjectOfType<EventSystem>();
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

            cameraRig.EnsureGameObjectIntegrity();
            Canvas[] canvases = FindObjectsOfType<Canvas>();
            foreach(var v in canvases)
            {
                v.worldCamera = cameraRig.leftEyeCamera;
            }

#if UNITY_EDITOR
            MakeMouseGazeConfiguration(cameraRig.gameObject);
#endif
        }

        shouldHandleExitButton = true;

    }

    private void Update()
    {
        inputModule.rayTransform = OVRGazePointer.instance.rayTransform =
            (OVRInput.GetActiveController() == OVRInput.Controller.Touch) ? cameraRig.rightHandAnchor :
            (OVRInput.GetActiveController() == OVRInput.Controller.RTouch) ? cameraRig.rightHandAnchor :
            (OVRInput.GetActiveController() == OVRInput.Controller.LTouch) ? cameraRig.leftHandAnchor :
            cameraRig.centerEyeAnchor;
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
    
    private void MakeMouseGazeConfiguration(GameObject c) // no physics raycaster is needed in daydream controller case
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
    }

    private EventSystem GetEventSystem()
    {
        if (null == es)
        {
            es = FindObjectOfType<EventSystem>();
            if (null == es)
            {
                es = Instantiate(Resources.Load("EventSystem_noCurved") as GameObject).GetComponent<EventSystem>();
            }
        }
        return es;
    }
    
    private void MakeGoogleVRConfiguration(bool isDaydream)
    {
        if (FindObjectOfType<GvrViewer>() != null)
        {
            return;
        }

        GameObject v = Instantiate(Resources.Load("GvrViewerMain") as GameObject);
        if (null != v)
        {
            GvrViewer viewer = v.GetComponent<GvrViewer>();
            if (null != viewer)
            {
                viewer.VRModeEnabled = GameManager.StereoMode;
            }
        }

        Instantiate(Resources.Load("GvrEventSystem") as GameObject);

        if (null == FindObjectOfType<GvrController>())
        {
            GameObject go = Instantiate(Resources.Load("GvrControllerMain") as GameObject);
            DontDestroyOnLoad(go);
        }

        reticle = FindObjectOfType<GvrReticlePointer>();

        if (isDaydream)
        {
            laser = FindObjectOfType<GvrLaserPointer>();
        }

        string s = isDaydream ? "_daydream" : "_cardboard";

        Debug.Log(s);

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

}
