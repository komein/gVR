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
    RoadPart nextPart;
    CollectiblePool pool;
    List<RoadPart> nextCandidates;

    DataStorage data;

    public float MinChance = 0.3f;
    public float MaxChance = 0.7f;
    public float multChance = 0.3f;

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
        if (!IsValidRoadPart(r))
        {
            return;
        }

        RemoveCollectiblesFromRoadPart(currentPart);

        UpdateFutureCandidates(r);
        UpdateRoadParts(r);

        PlaceNextRoadPart();
    }

    private bool IsValidRoadPart(RoadPart r) // basic meaning - allowed only that was stored in 'nextPart'; some additional conditions just to be safe
    {
        if (null == r)
        {
            return false;
        }

        if (r == currentPart || r == prevPart)
        {
            return false;
        }

        if (null != nextPart)
        {
            if (r != nextPart)
            {
                return false;
            }
        }

        return true;
    }

    private void PlaceNextRoadPart()
    {
        if (nextCandidates.Count > 0)
        {
            nextPart = nextCandidates[Random.Range(0, nextCandidates.Count)];

            PlaceNextPart();
            FillCollectibles();
        }
    }

    private void RemoveCollectiblesFromRoadPart(RoadPart r)
    {
        if (null != r)
        {
            if (null != pool)
            {
                pool.RemoveAllFromRoad(r);
            }
        }
    }

    private void UpdateFutureCandidates(RoadPart r) // making sure we don't use current or previous road as next one
    {
        if (null != r)
        {
            nextCandidates.Remove(r);

            if (null != currentPart)
            {
                nextCandidates.Remove(currentPart);
            }

            if (null != prevPart)
            {
                nextCandidates.Add(prevPart);
            }
        }
    }

    private void UpdateRoadParts(RoadPart r)
    {
        prevPart = currentPart;
        currentPart = r;
    }

    private void PlaceNextPart()
    {
        if (currentPart != null && nextPart != null)
        {
            nextPart.transform.position = currentPart.transform.position + new Vector3(0, 0, currentPart.partSize);
        }
    }

    private void FillCollectibles()
    {
        if (null != pool && null != nextPart && null != data)
        {
            List<CollectiblePlace> places = nextPart.GetComponentsInChildren<CollectiblePlace>().ToList();

            if (null != places)
            {
                if (places.Count > 0)
                    if (data.GetHp() < 3) // FIXME someday
                    {
                        CollectiblePlace hpPlace = places[Random.Range(0, places.Count)];
                        if (null != hpPlace)
                        {
                            pool.PlaceHp(hpPlace.transform.position, nextPart, currentPart);
                            places.Remove(hpPlace);
                        }
                    }

                if (places.Count > 0)
                    if (Random.value < multChance)
                    {
                        CollectiblePlace multPlace = places[Random.Range(0, places.Count)];
                        if (null != multPlace)
                        {
                            pool.PlaceMultiplier(multPlace.transform.position + new Vector3(0, 0.05f, 0), nextPart, currentPart);
                            places.Remove(multPlace);
                        }
                    }

                foreach (var p in places)
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
