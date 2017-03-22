using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressStar : MonoBehaviour
{
    Image fillImage;
    Image borderImage;

    public float step = 0.1f;
    public float period = 4f;
    public float gaussConst = 2f;
    public float power = 1f;
    public float time = 1f;

    public bool filled = false;

	void Awake ()
    {
        fillImage = GetComponent<Image>();
        borderImage = GetComponentInChildren<Image>();
        MakeEmpty();
    }

    public void FillAnimated()
    {
        filled = true;
        if (null != fillImage)
        {
            fillImage.color = new Color(fillImage.color.r, fillImage.color.g, fillImage.color.b, 1);
        }
        StopAllCoroutines();
        StartCoroutine(PlayTransition());
    }

    public void FillNoAnimation()
    {
        filled = true;
        if (null != fillImage)
        {
            fillImage.color = new Color(fillImage.color.r, fillImage.color.g, fillImage.color.b, 1);
        }
    }

    public void MakeEmpty()
    {
        filled = false;
        if (null != fillImage)
        {
            fillImage.color = new Color(fillImage.color.r, fillImage.color.g, fillImage.color.b, 0);
        }
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
}
