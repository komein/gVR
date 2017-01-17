using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCollectible : Collectible {

    public int value = 1;

    public override void Collect()
    {
        base.Collect();
        if (null != data)
        {
            data.AddScore(value);
        }
    }
}
