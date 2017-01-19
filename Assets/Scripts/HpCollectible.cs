using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpCollectible : Collectible {

    public int value = 1;

    public override void Collect()
    {
        if (data.GetHp() < 3) // FIXME
        {
            base.Collect();
            data.AddHp(value);

            CaptionText text = FindObjectOfType<CaptionText>();
            if (null != text)
            {
                string v = "+" + value + "hp!";

                text.PlaceText(v, transform.position + Vector3.up * 0.5f);
            }
        }
    }
}
