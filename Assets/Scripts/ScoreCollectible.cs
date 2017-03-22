using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCollectible : Collectible {

    public int value = 1;

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

            CaptionText text = FindObjectOfType<CaptionText>();
            if (null != text)
            {
                string v = "+" + value;

                if (null != DataObjects.sceneInfo)
                {
                    if (DataObjects.sceneInfo.multiplier > 1)
                        v += " (x" + DataObjects.sceneInfo.multiplier + "!)";
                }
                 
                text.PlaceText(v, transform.position + Vector3.up * 0.5f);
            }

        }
    }
}
