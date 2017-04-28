using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyCollider : MonoBehaviour
{

    Action action;

    public void SetAction(Action a)
    {
        action = a;
    }

    public void Trigger()
    {
        if (null != action)
        {
            action();
        }
    }

}
