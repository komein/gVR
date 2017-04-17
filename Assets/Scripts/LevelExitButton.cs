using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelExitButton : LookableButton
{

    PlayerController ch;
    protected override void Start()
    {
        base.Start();

        ch = FindObjectOfType<PlayerController>();

    }
    protected override void Function()
    {
        base.Function();

        ch.FinishLevel(PlayerController.PauseType.pause);
    }
}
