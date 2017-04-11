using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Collectible : MonoBehaviour {

    protected AudioSource aus;
    RoadPart road;

    protected ParticleSystem collectionFX;
    protected ParticleSystem persistentFX;

    protected virtual void Start()
    {
        aus = GetComponent<AudioSource>();
        if (null == aus)
        {
            aus = gameObject.AddComponent<AudioSource>();
        }

        collectionFX = GetComponentsInChildren<ParticleSystem>().ToList().Find(p => p.name == "Particle");
        persistentFX = GetComponentsInChildren<ParticleSystem>().ToList().Find(p => p.name == "PersistentParticle");
    }

    public virtual void Collect()
    {
        SetVisible(false);

        if (null != collectionFX)
        {
            collectionFX.Play();
        }

        AudioSource aus = GetComponent<AudioSource>();
        if (null != aus)
        {
            aus.Play();
        }
    }

    public virtual void SetVisible(bool v)
    {
        if (null != persistentFX)
        {
            if (!v)
            {
                persistentFX.Stop();
            }
            else
            {
                persistentFX.Play();
            }
        }

        GetComponentInChildren<MeshRenderer>().enabled = v;
        GetComponent<Collider>().enabled = v;
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
