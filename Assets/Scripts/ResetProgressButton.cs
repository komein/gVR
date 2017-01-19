using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetProgressButton : LookableButton
{
    DataStorage data;

    protected override void Start()
    {
        base.Start();
        data = FindObjectOfType<DataStorage>();
    }

    protected override void Function()
    {
        data.SetScore(0);
        data.Save();
        data.Load();

        StartCoroutine(PressedMessage());

        base.Function();
    }

}
