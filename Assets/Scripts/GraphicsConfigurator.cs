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
        Vector3 cameraPos = Camera.main.transform.position;
        Transform cameraParent = Camera.main.transform.parent;
        Camera.main.gameObject.SetActive(false);

        GvrControllerVisualManager gvrManager = FindObjectOfType<GvrControllerVisualManager>();

        if (null != gvrManager)
        {
            gvrManager.gameObject.SetActive(false);
        }

        GameObject v = Instantiate(Resources.Load("OVRCameraRig") as GameObject);

        if (null != v)
        {
            v.transform.position = cameraPos;
            v.transform.SetParent(cameraParent, true);

            Instantiate(Resources.Load("OVRInspectorLoader") as GameObject);

#if UNITY_EDITOR
            MakeMouseGazeConfiguration(v);
#endif
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
}
