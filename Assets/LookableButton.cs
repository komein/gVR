using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LookableButton : MonoBehaviour, IGvrGazeResponder
{
    public Image img;
    Text text;

    public string caption;

    protected bool isGazedOn = false;
    protected float time = 0f;

    public float activateTime = 1.2f;

    protected virtual void Start () {
        text = GetComponentInChildren<Text>();
        
        if (null != text)
        {
            text.text = caption;
        }

        SetGazedAt(false);

    }

    public virtual void SetGazedAt(bool gazedAt)
    {
        isGazedOn = gazedAt;
        img.fillAmount = 0;
    }

    protected virtual void Update()
    {
        if (isGazedOn)
        {
            time += Time.deltaTime;

            img.fillAmount = Mathf.Min(1f, (time / (float)activateTime));
        }
        else
        {
            time = 0f;
        }

        if (time >= activateTime)
        {
            Function();
        }
    }

    protected virtual void Function()
    {

    }

    public void OnGazeEnter()
    {
        SetGazedAt(true);
    }

    public void OnGazeExit()
    {
        SetGazedAt(false);
    }

    public void OnGazeTrigger()
    {
        Function();
    }
}
