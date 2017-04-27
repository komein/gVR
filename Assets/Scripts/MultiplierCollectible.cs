using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplierCollectible : Collectible {

    public int multiplier = 1;
    public float period = 15;

    public bool isPaused = false;

    Action startAction;
    Action stopAction;

    Coroutine c = null;

    protected override void Start()
    {
        base.Start();
        DataObjects.SetMusic("boostPickup", aus);
    }

    public override void Collect()
    {
        base.Collect();

        if (null != c)
        {
            StopCoroutine(c);
        }
        c = StartCoroutine(SetMultiplier(multiplier));

        if (null != caption)
        {
            caption.PlaceText("x2!", transform.position);
        }
    }

    public void SetActions(Action stt, Action stp)
    {
        startAction = stt;
        stopAction = stp;
    }

    private IEnumerator SetMultiplier(int v)
    {
        if (null != DataObjects.GameController)
        {
            DataObjects.GameController.SetMultiplier(v);

            if (null != startAction)
            {
                startAction();
            }

            for (float t = 0; t < 15; t += Time.deltaTime)
            {
                while (isPaused)
                {
                    yield return null;
                }

                yield return new WaitForEndOfFrame();
            }

            DataObjects.GameController.SetMultiplier(1);

            if (null != stopAction)
            {
                stopAction();
            }
        }

        yield return null;
    }
}
