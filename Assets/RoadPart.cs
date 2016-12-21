using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadPart : MonoBehaviour {

    PlaneGround [] grounds;

    OnRoadTrigger onRoad;

    public float partSize = 16;

    void Start () {
        grounds = GetComponentsInChildren<PlaneGround>();

	}

    public void SetDelegate(OnRoadTrigger r)
    {
        onRoad = r;

        foreach(PlaneGround p in grounds)
        {
            p.SetTriggerCallback(TriggerAction);
        }
    }

    void TriggerAction()
    {
        if (null != onRoad)
            onRoad(this);
    }
}
