#define OCULUS_STUFF
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CanvasRaycaster : MonoBehaviour {

	// Use this for initialization
	void Awake () {
#if UNITY_HAS_GOOGLEVR
        if (null == GetComponent<GvrPointerGraphicRaycaster>())
        {
            gameObject.AddComponent<GvrPointerGraphicRaycaster>();
        }
        if (null == GetComponent<GvrPointerPhysicsRaycaster>())
        {
            gameObject.AddComponent<GvrPointerPhysicsRaycaster>();
        }
#endif

#if OCULUS_STUFF
        if (null == GetComponent<OVRRaycaster>())
        {
            gameObject.AddComponent<OVRRaycaster>();
        }
#else
        if (null == GetComponent<PhysicsRaycaster>())
        {
            gameObject.AddComponent<PhysicsRaycaster>());
        }
#endif
    }
}
