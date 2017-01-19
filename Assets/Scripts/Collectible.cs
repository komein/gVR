using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour {

    RoadPart road;
    protected DataStorage data;

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
        AudioSource aus = GetComponent<AudioSource>();
        if (null != aus)
        {
            aus.Play();
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
