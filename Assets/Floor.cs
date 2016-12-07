using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class Floor : SceneSelecter
{
    protected override void Update()
    {
        if (isGazedOn)
        {
            time += Time.deltaTime;
            GetComponent<Renderer>().material.color = GAZED_COLOR - new Color(0, time / TELEPORT_TIME, 0);
        }
    }

    public override void SetGazedAt(bool gazedAt)
    {
        isGazedOn = gazedAt;
    }
}
