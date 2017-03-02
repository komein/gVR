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
    DataStorage data;

    protected override void Start()
    {
        base.Start();
        data = FindObjectOfType<DataStorage>();
    }

    protected override void Function()
    {
        if (null == data)
            return;
        
        data.MakeNewSaveFile();

        foreach (var c in FindObjectsOfType<Canvas>())
        {
            c.BroadcastMessage("Reinitialize");
        }
        
        StartCoroutine(PressedMessage());

        base.Function();
    }

}
