using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCollectible : Collectible {

    public int value = 1;

    protected override void Start()
    {
        base.Start();
        if (null != DataObjects.music)
        {
            aus.clip = DataObjects.music.GetMusic("scorePickup");
        }
    }

    public override void Collect()
    {
        base.Collect();

        if (null != DataObjects.gameController)
        {
            ScoreDisplayer d = FindObjectOfType<ScoreDisplayer>();
            if (null == d)
            {
                return;
            }
            
            DataObjects.gameController.AddScore(value);

        }
    }
}
