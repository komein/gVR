using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate void OnRoadTrigger(RoadPart r);

public class RoadGenerator : MonoBehaviour
{
    List<RoadPart> roadParts;
    RoadPart currentPart;
    RoadPart prevPart;
    CollectiblePool pool;
    List<RoadPart> nextCandidates;

    DataStorage data;

    public float MinChance = 0.3f;
    public float MaxChance = 0.7f;

	void Start ()
    {
        data = FindObjectOfType<DataStorage>();
        roadParts = GetComponentsInChildren<RoadPart>().ToList();
        nextCandidates = roadParts;
        pool = FindObjectOfType<CollectiblePool>();
		foreach (RoadPart r in roadParts)
        {
            r.SetDelegate(RoadTriggered);
        }
	}
	
    void RoadTriggered(RoadPart r)
    {
        if (null != r)
        {
            if (r == currentPart || r == prevPart)
            {
                return;
            }

            if (null != prevPart)
                nextCandidates.Add(prevPart);

            prevPart = currentPart;
            currentPart = r;;

            if (null != prevPart)
            {
                if (null != pool)
                {
                    pool.RemoveAllFromRoad(prevPart);
                }
            }

            nextCandidates.Remove(prevPart);
            nextCandidates.Remove(currentPart);

            RoadPart nextPart = nextCandidates[Random.Range(0, nextCandidates.Count)];

            if (nextPart != null)
            {
                nextPart.transform.position = currentPart.transform.position + new Vector3(0, 0, currentPart.partSize);

                if (null != pool)
                {
                    List<CollectiblePlace> places = nextPart.GetComponentsInChildren<CollectiblePlace>().ToList();
                    
                    if (null != data)
                    {
                        if (data.GetHp() < 3) // FIXME
                        {
                            CollectiblePlace hpPlace = places[Random.Range(0, places.Count)];
                            if (null != hpPlace)
                            {
                                pool.PlaceHp(hpPlace.transform.position, nextPart, currentPart);
                                places.Remove(hpPlace);
                            }
                        }
                    }

                    foreach(var p in places)
                    {
                        float chance = Random.Range(MinChance, MaxChance); // WOW
                        if (Random.value > chance) // MUCH RANDOM
                        {
                            pool.PlaceRandom(p.transform.position, nextPart);
                        }
                    }
                }
            }
        }
    }
}
