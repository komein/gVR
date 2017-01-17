using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplierCollectible : Collectible {

    public int multiplier = 1;
    public float period = 15;

    public override void Collect()
    {
        base.Collect();
        StartCoroutine(SetMultiplier(multiplier));
    }

    private IEnumerator SetMultiplier(int v)
    {
        if (null != data)
        {
            data.SetMultiplier(v);

            yield return new WaitForSeconds(period);

            data.SetMultiplier(1);
        }

        yield return null;
    }
}
