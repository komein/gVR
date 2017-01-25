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
            ScoreDisplayer d = FindObjectOfType<ScoreDisplayer>();
            if (null == d)
            {
                return;
            }
            data.AddScore(d.levelNumber, value);

            CaptionText text = FindObjectOfType<CaptionText>();
            if (null != text)
            {
                string v = "+" + value;
                if (data.multiplier > 1)
                    v += " (x" + data.multiplier + "!)";
                 
                text.PlaceText(v, transform.position + Vector3.up * 0.5f);
            }

        }
    }
}
