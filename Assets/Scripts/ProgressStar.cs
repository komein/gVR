using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressStar : MonoBehaviour
{
    public Image fillImage;
    public Image borderImageActive;
    public Image borderImageInactive;

    public float step = 0.1f;
    public float period = 4f;
    public float gaussConst = 2f;
    public float power = 1f;
    public float time = 1f;

    public enum StarState { invalid, inactive, empty, filled };
    public StarState state;

    public bool IsFilled
    {
        get
        {
            return state == StarState.filled;
        }
    }

	void Awake ()
    {
        state = StarState.invalid;
        ResetStarImages();
    }

    private void ResetStarImages()
    {
        ToggleImage(fillImage, false);
        ToggleImage(borderImageActive, false);
        ToggleImage(borderImageInactive, false);
    }

    public void SetState(StarState s, bool animated = false)
    {
        switch(s)
        {
            case StarState.invalid:
                Debug.LogError("That isn't the state you should set.");
                return;
            case StarState.inactive:
                ResetStarImages();
                if (ToggleImage(borderImageInactive, true))
                    state = s;
                return;
            case StarState.empty:
                ResetStarImages();
                if (ToggleImage(borderImageActive, true))
                    state = s;
                return;
            case StarState.filled:
                ResetStarImages();
                if (ToggleImage(fillImage, true))
                {
                    if (animated)
                    {
                        StopAllCoroutines();
                        StartCoroutine(PlayTransition());
                    }
                    state = s;
                }
                return;
        }
    }

    private bool ToggleImage(Image i, bool v)
    {
        if (null != i)
        {
            i.enabled = v;
            return true;
        }
        return false;
    }

    private IEnumerator PlayTransition()
    {
        if (null == fillImage)
        {
            yield return null;
        }

        fillImage.transform.localScale = Vector3.one;

        float x = -period / 2;
        float y = 0;

        float stepAmount = period / step;
        float timeDelta = time / stepAmount;

        while (x < period / 2)
        {
            x += period / stepAmount;
            y = 1f / (gaussConst * Mathf.PI) * Mathf.Exp(-(Mathf.Pow(x, gaussConst) / 2)) * power + 1;
            fillImage.transform.localScale = new Vector3(y, y, y);
            yield return new WaitForSeconds(timeDelta);
        }

        fillImage.transform.localScale = Vector3.one;

        yield return null;
    }

    internal void MakeNormalSize()
    {
        if (null == fillImage)
        {
            return;
        }
        fillImage.transform.localScale = Vector3.one;
    }
}
