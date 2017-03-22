using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelExitButton : LookableButton
{

    RunningCatController ch;
    protected override void Start()
    {
        base.Start();

        ch = FindObjectOfType<RunningCatController>();

    }
    protected override void Function()
    {
        base.Function();

        ch.FinishLevel();
    }
}
