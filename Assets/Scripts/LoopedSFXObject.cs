using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopedSFXObject : SFXObject
{
    public bool stopFlag = false;

    protected override void Start()
    {
        base.Start();

        StartCoroutine(SFXLoop());
    }


    IEnumerator SFXLoop()
    {
        if (null == aus)
        {
            yield return null;
        }
        if (null == aus.clip)
        {
            yield return null;
        }

        while (!stopFlag)
        {
            if (PlaySound())
            {
                yield return new WaitForSeconds(aus.clip.length);
            }
            yield return new WaitForSeconds(1f);
        }

        yield return null;
    }
}
