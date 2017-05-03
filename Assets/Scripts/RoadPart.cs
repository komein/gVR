using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadPart : MonoBehaviour
{
    PlaneGround [] grounds;

    OnRoadTrigger onRoad;

    public float partSize = 16;

    void Awake ()
    {
        grounds = GetComponentsInChildren<PlaneGround>();
    }

    public void SetDelegate(OnRoadTrigger r)
    {
        onRoad = r;
        if (null != grounds)
        {
            foreach (PlaneGround p in grounds)
            {
                p.SetTriggerCallback(TriggerAction);
            }
        }
    }

    void TriggerAction()
    {
        if (null != onRoad)
        {
            onRoad(this);
        }
    }
}
