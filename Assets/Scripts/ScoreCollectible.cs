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
            ScoreDisplayer2 d = FindObjectOfType<ScoreDisplayer2>();
            if (null == d)
            {
                return;
            }

            data.AddScore(value);

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
