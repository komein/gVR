using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Collectible : MonoBehaviour {

    protected AudioSource aus;
    RoadPart road;

    protected ParticleSystem collectionFX;
    protected ParticleSystem persistentFX;

    protected CaptionText caption;

    CapsuleCollider bigCollider;
    BoxCollider triggerCollider;

    protected virtual void Start()
    {
        caption = FindObjectOfType<CaptionText>();
        aus = GetComponent<AudioSource>();
        if (null == aus)
        {
            aus = gameObject.AddComponent<AudioSource>();
        }

        collectionFX = GetComponentsInChildren<ParticleSystem>().ToList().Find(p => p.name == "Particle");
        persistentFX = GetComponentsInChildren<ParticleSystem>().ToList().Find(p => p.name == "PersistentParticle");

        bigCollider = GetComponent<CapsuleCollider>();
        triggerCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            if (other as CapsuleCollider != null)
            {
                Collect();
            }
        }
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

    public virtual void SetVisible(bool value)
    {
        if (null != persistentFX)
        {
            if (!value)
            {
                persistentFX.Stop();
            }
            else
            {
                persistentFX.Play();
            }
        }

        GetComponentInChildren<MeshRenderer>().enabled = value;
        GetComponent<Collider>().enabled = value;

        ToggleModelCollider(value);
    }

    public void ToggleModelCollider(bool value)
    {
        MeshCollider[] c = GetComponentsInChildren<MeshCollider>();

        foreach (var v in c)
        {
            v.enabled = value;
        }
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
