using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelExitButton : LookableButton
{
    protected override void Function()
    {
        base.Function();

        DataObjects.gameManager.PauseLevel(PauseType.pause);
    }
}
