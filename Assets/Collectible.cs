using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour {

    MeshRenderer ren;
    RoadPart road;
    protected DataStorage data;

    public int value = 1;

    private void Start()
    {
        data = FindObjectOfType<DataStorage>();
    }

    public virtual void Collect()
    {
        SetVisible(false);
        ParticleSystem ps = GetComponent<ParticleSystem>();
        if (null != ps)
        {
            ps.Play();
        }
    }

    public void SetVisible(bool v)
    {
        GetComponent<MeshRenderer>().enabled = v;
        GetComponent<BoxCollider>().enabled = v;
    }


    public RoadPart GetRoadPart()
    {
        return road;
    }

    public void SetRoadPart(RoadPart r)
    {
        road = r;
    }
}
