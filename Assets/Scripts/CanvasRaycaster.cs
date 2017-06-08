using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CanvasRaycaster : MonoBehaviour
{
	void Awake ()
    {
        switch(DataObjects.GameManager.mode)
        {
            case GameManager.VRMode.GoogleVR_Cardboard:
            case GameManager.VRMode.GoogleVR_Daydream:

                if (null == GetComponent<GvrPointerGraphicRaycaster>())
                {
                    gameObject.AddComponent<GvrPointerGraphicRaycaster>();
                }
                if (null == GetComponent<GvrPointerPhysicsRaycaster>())
                {
                    gameObject.AddComponent<GvrPointerPhysicsRaycaster>();
                }
                break;
            case GameManager.VRMode.Oculus:
                if (null == GetComponent<OVRRaycaster>())
                {
                    gameObject.AddComponent<OVRRaycaster>();
                }
                break;
            case GameManager.VRMode.none:
            default:
                break;
        }

    }
}
