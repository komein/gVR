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

    private GvrReticlePointer reticle;
    public GvrReticlePointer Reticle
    {
        get
        {
            if (null == reticle)
            {
                reticle = FindObjectOfType<GvrReticlePointer>();
            }
            return reticle;
        }
    }

    public GvrController gvrController;
    GvrController gc;

    public GvrControllerVisualManager gvrArm;
    GvrControllerVisualManager arm;

    public EventSystem eventSystemPrefab;
    EventSystem es;

    private GvrLaserPointer laser;
    public GvrLaserPointer Laser
    {
        get
        {
            if (null == laser)
            {
                laser = FindObjectOfType<GvrLaserPointer>();
            }
            return laser;
        }
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
                        MakeGoogleVRConfiguration();
                        break;
                    }

                case GameManager.VRMode.Daydream:
                    {
                        MakeGoogleVRConfiguration();
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
    
    
    /*
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
*/

    private void MakeGoogleVRConfiguration()
    {
        Instantiate(Resources.Load("GvrViewerMain") as GameObject);
        Instantiate(Resources.Load("GvrEventSystem") as GameObject);

        if (null == FindObjectOfType<GvrController>())
        {
            GameObject v = Instantiate(Resources.Load("GvrControllerMain") as GameObject);
            DontDestroyOnLoad(v);
        }

        GameObject manager = Instantiate(Resources.Load("DemoInputManager") as GameObject);

        if (null != manager)
        {
            Vector3 pos = manager.transform.position;

            Player p = FindObjectOfType<Player>();

            if (null != p)
            {
                //manager.transform.position = new Vector3(p.transform.position.x, manager.transform.position.y, p.transform.position.z) + pos;
                manager.transform.SetParent(p.transform, true);
            }
        }


    }

}
