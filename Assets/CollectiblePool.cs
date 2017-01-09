using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectiblePool : MonoBehaviour {

    private Collectible[] cakePrefabs;

    private List<Collectible> pool = new List<Collectible>();
    private List<Collectible> placedElements = new List<Collectible>();

    public int poolDepth = 10;

	void Awake () {
        cakePrefabs = GetComponentsInChildren<Collectible>();
		foreach(Collectible c in cakePrefabs)
        {
            for (int i = 0; i < poolDepth; i++)
            {
                Collectible instance = GameObject.Instantiate<Collectible>(c);
                pool.Add(instance);
                instance.transform.SetParent(this.transform);
            }
        }
	}
	
    private Collectible GetRandomCollectible()
    {
        if (null != pool)
        {
            if (pool.Count > 0)
            {
                int randomSubPoolN = Random.Range(0, pool.Count);
                return pool[randomSubPoolN];
            }
        }

        return null;
    }

    public void RemoveAllFromRoad(RoadPart r)
    {
        List<Collectible> list = placedElements.FindAll(p => p.GetRoadPart() == r);
        placedElements.RemoveAll(p => p.GetRoadPart() == r);
        pool.AddRange(list);
    }

    public Collectible PlaceRandom(Vector3 v, RoadPart r)
    {
        Collectible c = GetRandomCollectible();
        if (null != c)
        {
            c.transform.position = v;
            c.SetRoadPart(r);
            c.Replace();

            pool.Remove(c);
            placedElements.Add(c);
        }

        return c;
    }
}
