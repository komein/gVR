using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void CrushDelegate(Vector3 v);
public class ObstacleController : MonoBehaviour
{
    IndestructibleObstacle[] obstacles;
    RunningCatController character;

    public bool ghostMode = false;

	void Start () {

        obstacles = FindObjectsOfType<IndestructibleObstacle>();

        if (null != obstacles)
        {
            foreach (IndestructibleObstacle i in obstacles)
            {
                if (null != i)
                {
                    i.SetCrushAction(CrushIntoObstacle);
                }
            }
        }

        ToggleObstaclesGhost(ghostMode);

        character = FindObjectOfType<RunningCatController>();
    }

    private void CrushIntoObstacle(Vector3 v)
    {
        StopAllCoroutines();
        StartCoroutine(CrushCoroutine(v));
    }

    private IEnumerator CrushCoroutine(Vector3 v)
    {
        if (null != character)
        {
            character.MakeCrush(v);

            ToggleObstaclesGhost(true);
            character.ToggleFlashing(true);

            yield return new WaitForSeconds(4.0f);

            ToggleObstaclesGhost(false);
            character.ToggleFlashing(false);
        }

        yield return null;
    }

    private void ToggleObstaclesGhost(bool isGhost)
    {
        foreach(IndestructibleObstacle i in obstacles)
        {
            if (null != i)
            {
                Collider c = i.GetComponent<Collider>();
                if (null != c)
                {
                    c.enabled = !isGhost;
                }
                
            }
        }
    }
}
