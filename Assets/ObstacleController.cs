using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void CrushDelegate(Collision c);
public class ObstacleController : MonoBehaviour
{
    IndestructibleObstacle[] obstacles;
    CharacterPrototype2 character;
	// Use this for initialization
	void Start () {
        obstacles = FindObjectsOfType<IndestructibleObstacle>();
        Debug.Log("amount of obstacles: " + obstacles.Length);
        foreach(IndestructibleObstacle i in obstacles)
        {
            i.SetCrushAction(CrushIntoObstacle);
        }
        character = FindObjectOfType<CharacterPrototype2>();
        Debug.Log("character" + character != null ? "" : " not" + " found!");
	}

    private void CrushIntoObstacle(Collision c)
    {
        StopAllCoroutines();
        StartCoroutine(CrushCoroutine(c));
    }

    private IEnumerator CrushCoroutine(Collision c)
    {
        ToggleObstaclesGhost(false);
        character.MakeCrush(c);
        character.ToggleFlashing(true);
        yield return new WaitForSeconds(4.0f);
        ToggleObstaclesGhost(true);
        character.ToggleFlashing(false);
        yield return null;
    }

    private void ToggleObstaclesGhost(bool v)
    {
        foreach(IndestructibleObstacle i in obstacles)
        {
            BoxCollider c = i.GetComponent<BoxCollider>();
            if (null != c)
            {
                c.enabled = v;
            }
        }
    }
}
