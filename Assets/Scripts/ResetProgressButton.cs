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
        if (null == DataObjects.dataManager)
            return;

        DataObjects.dataManager.MakeNewSaveFile();

        foreach (var c in FindObjectsOfType<Canvas>())
        {
            c.BroadcastMessage("Reinitialize");
        }
        
        StartCoroutine(PressedMessage());

        base.Function();
    }

}
