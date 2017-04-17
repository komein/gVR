using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueButton : LookableButton
{
    PlayerController player;

    protected override void Start()
    {
        base.Start();
        player = FindObjectOfType<PlayerController>();
    }

    protected override void Function()
    {
        base.Function();
        if (null != player)
        {
            player.ResumeLevel();
        }
    }
}
