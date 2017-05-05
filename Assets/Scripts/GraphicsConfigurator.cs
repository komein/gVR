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
            if (null == reticle)
            {
                reticle = FindObjectOfType<GvrReticlePointer>();
            }
            return reticle;
        }
    }

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

    internal void Initialize()
    {
        if (null != FindObjectOfType<GraphicsOverrider>())
        {
            return;
        }

        if (null == DataObjects.GameManager)
        {
            MakeMouseGazeConfiguration();
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
                        break;
                    }

                case GameManager.VRMode.noVR:
                    {
                        MakeMouseGazeConfiguration();
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
    
    private void MakeMouseGazeConfiguration() // no physics raycaster is needed in daydream controller case
    {
        Camera c = Camera.main;
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
    
    private void MakeGoogleVRConfiguration()
    {
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

        GameObject manager = Instantiate(Resources.Load("DemoInputManager") as GameObject);

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
}
