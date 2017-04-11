using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LookableButton : MonoBehaviour, IGvrGazeResponder
{
    public Image img;
    protected TextMeshProUGUI text;

    public string caption;
    public string pressedCaption;

    protected bool isGazedOn = false;
    protected bool pressed = false;

    protected float time = 0f;

    public float activateTime = 1.2f;

    public Color normalColor;
    public Color pressedColor;

    AudioSource aus;

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

    private bool gMode = true;

    protected virtual void Start ()
    {
        aus = GetComponent<AudioSource>();
        if (null == aus)
            aus = gameObject.AddComponent<AudioSource>();

        text = GetComponentInChildren<TextMeshProUGUI>();

        if (null != text)
        {
            text.text = caption;
        }

        SetGazedAt(false);
    }

    public void UpdateGazeMode()
    {
#if UNITY_HAS_GOOGLEVR
        gazeMode = DataObjects.gameManager.controllerState != GvrConnectionState.Connected;
#else
        gazeMode = true;
#endif
    }

    public virtual void SetGazedAt(bool gazedAt)
    {
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
                aus.clip = DataObjects.music.GetMusic("gaze");
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
                aus.clip = DataObjects.music.GetMusic("hover");
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
        aus.Stop();
        aus.clip = DataObjects.music.GetMusic("click");
        aus.Play();

        if (isActiveButton)
            pressed = true;
        else
            pressed = false;
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
}
