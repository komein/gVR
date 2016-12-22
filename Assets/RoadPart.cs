using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadPart : MonoBehaviour {

    PlaneGround [] grounds;

    OnRoadTrigger onRoad;

    public float partSize = 16;

    void Awake () {
        grounds = GetComponentsInChildren<PlaneGround>();
        Debug.Log(gameObject.name + "grounds init");
    }

    public void SetDelegate(OnRoadTrigger r)
    {
        onRoad = r;
        if (null != grounds)
            foreach (PlaneGround p in grounds)
            {
                p.SetTriggerCallback(TriggerAction);
            }
        else
            Debug.LogError(gameObject.name + "grounds are null");
    }

    void TriggerAction()
    {
        Debug.Log("touched " + gameObject.name);
        if (null != onRoad)
            onRoad(this);
    }
}
