using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void CrushDelegate(Vector3 v);
public class ObstacleController : MonoBehaviour
{
    IndestructibleObstacle[] obstacles;
    CharacterPrototype2 character;

	void Start () {
        obstacles = FindObjectsOfType<IndestructibleObstacle>();
        foreach(IndestructibleObstacle i in obstacles)
        {
            i.SetCrushAction(CrushIntoObstacle);
        }
        ToggleObstaclesGhost(false);
        character = FindObjectOfType<CharacterPrototype2>();

        //ToggleObstaclesGhost(true);
    }

    private void CrushIntoObstacle(Vector3 v)
    {
        StopAllCoroutines();
        StartCoroutine(CrushCoroutine(v));
    }

    private IEnumerator CrushCoroutine(Vector3 v)
    {
        character.MakeCrush(v);

        ToggleObstaclesGhost(true);
        character.ToggleFlashing(true);

        yield return new WaitForSeconds(4.0f);

        ToggleObstaclesGhost(false);
        character.ToggleFlashing(false);

        yield return null;
    }

    private void ToggleObstaclesGhost(bool isGhost)
    {
        foreach(IndestructibleObstacle i in obstacles)
        {
            BoxCollider c = i.GetComponent<BoxCollider>();
            if (null != c)
            {
                c.enabled = !isGhost;
            }
        }
    }
}
