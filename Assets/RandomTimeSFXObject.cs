using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTimeSFXObject : SFXObject
{
    public float chancePerSec;

    public bool stopFlag = false;

	protected override void Start()
    {
        base.Start();

        StartCoroutine(SFXChance(chancePerSec));
    }
	

    IEnumerator SFXChance(float chance)
    {
        if (null == aus)
        {
            yield return null;
        }
        if (null == aus.clip)
        {
            yield return null;
        }

        while(!stopFlag)
        {
            if (Random.value < chance)
            {
                if (PlaySound())
                {
                    yield return new WaitForSeconds(aus.clip.length);
                }
            }
            yield return new WaitForSeconds(1f);
        }

        yield return null;
    }
}
