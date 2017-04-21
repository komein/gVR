using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpCollectible : Collectible {

    public int value = 1;

    protected override void Start()
    {
        base.Start();
        DataObjects.SetMusic("healthPickup", aus);
    }

    public override void Collect()
    {
        if (null != DataObjects.GameController)
        {
            if (DataObjects.GameController.GetHp() < SceneInfo.HP_MAX)
            {
                base.Collect();
                DataObjects.GameController.AddHp(value);
                
                if (null != caption)
                {
                    caption.PlaceText("+" + value + "hp!", transform.position);
                }
            }
        }
    }
}
