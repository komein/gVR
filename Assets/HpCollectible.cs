using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpCollectible : Collectible {

    public override void Collect()
    {
        if (data.GetHp() < 3) // FIXME
        {
            data.AddHp(value);
            base.Collect();
        }
    }
}
