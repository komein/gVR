﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LookableButton : UIWithText
{
    public Image img;
    protected Text text;

    public string caption;
    public string pressedCaption;

    protected bool isGazedOn = false;
    protected bool pressed = false;

    protected float time = 0f;

    public float activateTime = 1.2f;

    public Color normalColor;
    public Color pressedColor;

    protected AudioSource aus;

    protected bool isActiveButton = true;

    public bool gazeMode
    {
        get
        {
            return gMode;
        }
        set
        {
            if (null != img)
            {
                img.fillAmount = 0;
                time = 0f;
            }
            gMode = value;
        }
    }

    protected bool gMode = true;

    protected virtual void Start ()
    {
        text = GetComponentInChildren<Text>();

        UpdateText();

        SetGazedAt(false);
    }

    public void UpdateGazeMode()
    {
#if UNITY_HAS_GOOGLEVR
        gazeMode = GvrController.State != GvrConnectionState.Connected;
#else
        gazeMode = true;
#endif

#if OCULUS_STUFF
        if (gazeMode) // still may be oculus case
        {
            gazeMode = FindObjectOfType<OVRInputModule>() == null;
        }
#endif
    }

    public virtual void SetGazedAt(bool gazedAt)
    {
        if (null == aus)
        {
            MakeAudioSystem();
        }

        aus.Stop();

        UpdateGazeMode();
        if (gMode)
        {
            pressed = false;
            if (!isActiveButton)
                return;

            isGazedOn = gazedAt;

            if (isGazedOn)
            {
                DataObjects.SetMusic("gaze", aus);
                aus.Play();
            }

            if (null != img)
            {
                img.fillAmount = 0;
                img.color = normalColor;
            }
        }
        else
        {
            if (gazedAt)
            {
                DataObjects.SetMusic("hover", aus);
                aus.Play();
                img.fillAmount = 1;
                img.color = new Color(normalColor.r, normalColor.g, normalColor.b, 1f);
            }
            else
            {
                img.fillAmount = 0;
            }
        }
    }

    private void MakeAudioSystem()
    {
        aus = GetComponent<AudioSource>();
        if (null == aus)
            aus = gameObject.AddComponent<AudioSource>();
    }

    protected virtual void Update()
    {
        if (gMode)
        {
            if (!pressed)
            {
                if (isGazedOn)
                {
                    time += Time.deltaTime;

                    if (null != img)
                    {
                        img.fillAmount = Mathf.Min(1f, (time / (float)activateTime));
                    }
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
                if (null != img)
                {
                    img.color = pressedColor;
                }
            }
        }
    }

    protected virtual void Function()
    {
        if (null == aus)
        {
            MakeAudioSystem();
        }

        aus.Stop();
        DataObjects.SetMusic("click", aus);
        aus.Play();

        if (isActiveButton)
            pressed = true;
        else
            pressed = false;

        SetGazedAt(false);
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
        if (null != text)
        {
            string t = text.text;

            text.text = pressedCaption;

            yield return new WaitForSeconds(2f);

            text.text = t;
        }

        yield return null;
    }

    public override void UpdateText()
    {
        if (null != text)
        {
            text.text = DataObjects.Localization.GetField(caption);
        }
    }
}
