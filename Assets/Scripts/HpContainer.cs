using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HpContainer : MonoBehaviour
{
    Image heart;
    Image broken;

	void Awake ()
    {
        heart = GetComponentsInChildren<Image>().ToList().Find(p => p.name == "Heart");
        broken = GetComponentsInChildren<Image>().ToList().Find(p => p.name == "Broken");

        SetBroken(false);
    }

    public void SetHeart(bool value)
    {
        if (null != heart)
        {
            heart.enabled = value;
        }

        if (value)
        {
            SetBroken(false);
        }
    }

    public void SetBroken(bool value)
    {
        if (null != broken)
        {
            broken.enabled = value;
        }
    }

    public void BreakTheHeart()
    {
        StopAllCoroutines();
        StartCoroutine(ToggleFlash());
    }

    public void MakeBiggerForSec()
    {
        StopAllCoroutines();
        StartCoroutine(MakeBigger());
    }

    private IEnumerator MakeBigger()
    {
        Vector3 scale = transform.localScale;
        float t = 0;
        while (t < 0.1f)
        {
            t += Time.deltaTime;
            transform.localScale *= 1.1f;
            yield return new WaitForEndOfFrame();
        }

        t = 0;
        while (t < 0.1f)
        {
            t += Time.deltaTime;
            transform.localScale /= 1.1f;
            yield return new WaitForEndOfFrame();
        }

        transform.localScale = scale;

        yield return null;
    }

    private IEnumerator ToggleFlash()
    {
        StartCoroutine(Flash());
        yield return new WaitForSeconds(0.5f);
        StopAllCoroutines();
        SetBroken(false);
        yield return null;
    }

    private IEnumerator Flash()
    {
        bool v = false;
        while (true)
        {
            v = !v;
            SetBroken(v);
            yield return new WaitForSeconds(0.05f);
        }
    }
}
