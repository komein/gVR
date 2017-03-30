using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GraphicsOverrider : MonoBehaviour {
    // just a dummy object preventing creating the graphics controller which setups vr stuff

    private void Start()
    {
        EventSystem es = FindObjectOfType<EventSystem>();

        if (null != es)
        {
            es.gameObject.AddComponent<StandaloneInputModule>();
        }
    }
}
