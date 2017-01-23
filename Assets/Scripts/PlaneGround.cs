﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneGround : MonoBehaviour {

    Action onTrigger;

    public void SetTriggerCallback(Action toSet)
    {
        onTrigger = toSet;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CharacterPrototype2>() != null)
        {
            if (null != onTrigger)
            {
                onTrigger();
            }
        }
    }
}