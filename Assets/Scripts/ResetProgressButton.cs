using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IUICanReinitialize : IEventSystemHandler
{
    void Reinitialize();
}

public class ResetProgressButton : LookableButton
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void Function()
    {
        if (!ResetProgress())
            return;

        StartCoroutine(PressedMessage());

        base.Function();
    }

    public bool ResetProgress()
    {
        if (null == DataObjects.DataManager)
            return false;

        DataObjects.DataManager.MakeNewSaveFile();

        foreach (var c in FindObjectsOfType<Canvas>())
        {
            c.BroadcastMessage("Reinitialize");
        }

        return true;
    }
}
