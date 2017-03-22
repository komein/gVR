using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplierCollectible : Collectible {

    public int multiplier = 1;
    public float period = 15;

    public override void Collect()
    {
        base.Collect();
        StopAllCoroutines();
        StartCoroutine(SetMultiplier(multiplier));
    }

    private IEnumerator SetMultiplier(int v)
    {
        if (null != DataObjects.gameController)
        {
            DataObjects.gameController.SetMultiplier(v);

            yield return new WaitForSeconds(period);

            DataObjects.gameController.SetMultiplier(1);
        }

        yield return null;
    }
}
