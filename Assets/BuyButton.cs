using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuyButton : SceneButton {

    DataStorage store;
    public LevelButtonContainer container;

    protected override void Start()
    {
        base.Start();

        store = FindObjectOfType<DataStorage>();

        Toggle(false);
    }

    protected override void Function()
    {
        pressed = true;

        SceneManager.LoadScene("dismountRequestScreen", LoadSceneMode.Single);
    }

    internal void Toggle(bool v)
    {
        if (null != container)
        {
            container.gameObject.SetActive(v);
        }
    }
}
