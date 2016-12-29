using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnRoadTrigger(RoadPart r);

public class RoadGenerator : MonoBehaviour
{
    public List<RoadPart> roadParts;
    RoadPart currentPart;
    RoadPart prevPart;

	void Start ()
    {
		foreach (RoadPart r in roadParts)
        {
            r.SetDelegate(RoadTriggered);
        }
	}
	
    void RoadTriggered(RoadPart r)
    {
        if (null != r)
        {
            //Debug.Log("triggered " + r.gameObject.name);
            if (r == currentPart)
            {
                return;
            }

            prevPart = currentPart;
            currentPart = r;;

            List<RoadPart> nextCandidates = roadParts.FindAll(p => p != prevPart && p != currentPart);

            RoadPart nextPart = nextCandidates[Random.Range(0, nextCandidates.Count)];
            if (nextPart != null)
            {
                nextPart.transform.position = currentPart.transform.position + new Vector3(0, 0, currentPart.partSize);
                Collectible[] collectibles = nextPart.GetComponentsInChildren<Collectible>();
                foreach(Collectible c in collectibles)
                {
                    c.Replace();
                }
            }
        }
    }
}
