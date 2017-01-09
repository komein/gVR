using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour {

    MeshRenderer ren;
    RoadPart road;
    ScoreStorage scoreStorage;

    public int value = 1;

    private void Start()
    {
        scoreStorage = FindObjectOfType<ScoreStorage>();
    }

    public void Collect()
    {
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;
        if (null != scoreStorage)
        {
            scoreStorage.UpdateScore(value);
        }
    }

    public void Replace()
    {
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<BoxCollider>().enabled = true;
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
