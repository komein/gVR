﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectiblePool : MonoBehaviour {

    private ScoreCollectible[] cakePrefabs;
    private HpCollectible hpPrefab;
    private MultiplierCollectible multPrefab;

    private List<ScoreCollectible> pool = new List<ScoreCollectible>();
    private List<HpCollectible> hpPool = new List<HpCollectible>();

    private List<ScoreCollectible> placedElements = new List<ScoreCollectible>();

    public int poolDepth = 10;

	void Awake () {
        hpPrefab = GetComponentInChildren<HpCollectible>();
        multPrefab = GetComponentInChildren<MultiplierCollectible>();
        cakePrefabs = GetComponentsInChildren<ScoreCollectible>();
        foreach (ScoreCollectible c in cakePrefabs)
        {
            for (int i = 0; i < poolDepth; i++)
            {
                ScoreCollectible instance = GameObject.Instantiate<ScoreCollectible>(c);
                pool.Add(instance);
                instance.transform.SetParent(this.transform);
            }
        }
	}
	
    private ScoreCollectible GetRandomCollectible()
    {
        if (null != pool)
        {
            if (pool.Count > 0)
            {
                int randomSubPoolN = UnityEngine.Random.Range(0, pool.Count);
                return pool[randomSubPoolN];
            }
        }

        return null;
    }

    public void RemoveAllFromRoad(RoadPart r)
    {
        List<ScoreCollectible> list = placedElements.FindAll(p => p.GetRoadPart() == r);
        placedElements.RemoveAll(p => p.GetRoadPart() == r);
        pool.AddRange(list);
    }

    public ScoreCollectible PlaceRandom(Vector3 v, RoadPart r)
    {
        ScoreCollectible c = GetRandomCollectible();
        if (null != c)
        {
            c.transform.position = v;
            c.transform.Rotate(Vector3.up, UnityEngine.Random.value * 180);
            c.SetRoadPart(r);
            c.SetVisible(true);

            pool.Remove(c);
            placedElements.Add(c);
        }

        return c;
    }
    
    public Collectible PlaceHp(Vector3 v, RoadPart r, RoadPart c)
    {
        if (null != hpPrefab)
        {
            if (hpPrefab.GetRoadPart() == c)
            {
                return null;
            }

            hpPrefab.transform.position = v + Vector3.up * 0.1f;
            hpPrefab.SetRoadPart(r);
            hpPrefab.SetVisible(true);
        }
        return hpPrefab;
    }

    internal Collectible PlaceMultiplier(Vector3 position, RoadPart nextPart, RoadPart currentPart)
    {
        if (null != multPrefab)
        {
            if (multPrefab.GetRoadPart() == currentPart)
            {
                return null;
            }

            multPrefab.transform.position = position + Vector3.up * 0.1f;
            multPrefab.SetRoadPart(nextPart);
            multPrefab.SetVisible(true);
        }
        return multPrefab;
    }
}
