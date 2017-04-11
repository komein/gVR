using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplierCollectible : Collectible {

    public int multiplier = 1;
    public float period = 15;

    Action startAction;
    Action stopAction;

    protected override void Start()
    {
        base.Start();
        aus.clip = DataObjects.Music("boostPickup");
    }

    public override void Collect()
    {
        base.Collect();
        StopAllCoroutines();
        StartCoroutine(SetMultiplier(multiplier));
    }

    public void SetActions(Action stt, Action stp)
    {
        startAction = stt;
        stopAction = stp;
    }

    private IEnumerator SetMultiplier(int v)
    {
        if (null != DataObjects.gameController)
        {
            DataObjects.gameController.SetMultiplier(v);

            if (null != startAction)
            {
                startAction();
            }

            yield return new WaitForSeconds(period);

            DataObjects.gameController.SetMultiplier(1);

            if (null != stopAction)
            {
                stopAction();
            }
        }

        yield return null;
    }
}
