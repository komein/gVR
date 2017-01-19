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
    public string pressedCaption;

    protected bool isGazedOn = false;
    protected bool pressed = false;

    protected float time = 0f;

    public float activateTime = 1.2f;

    public Color normalColor;
    public Color pressedColor;

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
        pressed = false;
        isGazedOn = gazedAt;
        img.fillAmount = 0;
        img.color = normalColor;
    }

    protected virtual void Update()
    {
        if (!pressed)
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
        else
        {
            img.color = pressedColor;
        }
    }

    protected virtual void Function()
    {
        pressed = true;
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


    protected IEnumerator PressedMessage()
    {
        string t = text.text;

        text.text = pressedCaption;

        yield return new WaitForSeconds(2f);

        text.text = t;

        yield return null;
    }
}
