using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GraphicsConfigurator : MonoBehaviour
{
    public static GraphicsConfigurator instanceRef; // singleton pattern

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
                        MakeGoogleVRConfiguration(false);
                        break;
                    }

                case GameManager.VRMode.Daydream:
                    {
                        MakeGoogleVRConfiguration(true);
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
        DeleteController();
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
            if (null == c.gameObject.GetComponent<MoveCameraScript>())
            {
                c.gameObject.AddComponent<MoveCameraScript>();
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
                es = new GameObject().AddComponent<EventSystem>();
                es.name = "EventSystem";
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
            ReinitGoogleVRController();
        }
    }

    private void MakeGoogleVRCameraConfiguration()
    {
        if (DataObjects.gameManager.noStereoMode)
        {
            MakeMouseGazeConfiguration(false);
        }
        else
        {
            MakeGoogleVRStereoConfiguration();
        }
    }

    public void ReinitGoogleVRController()
    {
        if (null == gc)
        {
            gc = FindObjectOfType<GvrController>();
            if (null == gc)
            {
                gc = Instantiate(gvrController);
                DontDestroyOnLoad(gc);
            }
        }

        // no parent object to put controller into
        Player p = FindObjectOfType<Player>();
        if (null == p)
        {
            return;
        }

#if UNITY_HAS_GOOGLEVR
        DeleteController();
        // controller is found
        if (DataObjects.gameManager.controllerState == GvrConnectionState.Connected)
        {
            // hide gaze reticle
            if (null != ret)
            {
                ret.gameObject.SetActive(false);
            }

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
        // controller is not found
        else
#endif
        {
            // this code works because if we hid the reticle then we have it as a variable
            if (null == ret)
            {
                ret = FindObjectOfType<GvrReticlePointer>();
                if (null == ret)
                {
                    Debug.LogWarning("No reticle is attached to the main camera. Instantiating from prefab..");
                    ret = Instantiate(reticlePrefab);
                    ret.transform.SetParent(Camera.main.transform);
                    ret.transform.localPosition = Vector3.zero;
                    ret.transform.localRotation = Quaternion.identity;
                }
            }
            else
            {
                ret.gameObject.SetActive(true);
            }

            ret.SetAsMainPointer();
        }
    }

    private void DeleteController()
    {
        if (null != arm)
        {
            Destroy(arm.gameObject);
        }
    }

}
