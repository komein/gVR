using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuyButton : SceneButton, IUICanReinitialize
{
    RectTransform container;

    private void Awake()
    {
        container = GetComponentsInChildren<RectTransform>().ToList().Find(p => p.name == "HidingPanel");
        Toggle(false);
    }

    protected override void Start()
    {
        base.Start();
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

    public void Reinitialize()
    {
        Toggle(false);
    }
}
