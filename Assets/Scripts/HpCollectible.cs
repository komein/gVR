using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpCollectible : Collectible {

    public int value = 1;

    public override void Collect()
    {
        if (null != DataObjects.gameController)
        {
            if (DataObjects.gameController.GetHp() < SceneInfo.HP_MAX)
            {
                base.Collect();
                DataObjects.gameController.AddHp(value);

                CaptionText text = FindObjectOfType<CaptionText>();
                if (null != text)
                {
                    string v = "+" + value + "hp!";

                    text.PlaceText(v, transform.position + Vector3.up * 0.5f);
                }
            }
        }
    }
}
