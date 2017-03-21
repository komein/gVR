using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelExitButton : LookableButton
{

    CharacterPrototype2 ch;
    protected override void Start()
    {
        base.Start();

        ch = FindObjectOfType<CharacterPrototype2>();

    }
    protected override void Function()
    {
        base.Function();

        ch.FinishLevel();
    }
}
