using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrokenHeartContainer : MonoBehaviour {

    Image img;

    Coroutine toggleC;
    Coroutine flashC;

    void Start()
    {
        img = GetComponentInChildren<Image>();
        SetMeshes(false);
    }

    public void BreakTheHeart()
    {
        if (null != toggleC)
        {
            StopCoroutine(toggleC);
        }

        toggleC = StartCoroutine(ToggleFlash());
    }

    private IEnumerator ToggleFlash()
    {
        flashC = StartCoroutine(Flash());

        yield return new WaitForSeconds(0.5f);

        if (null != flashC)
        {
            StopCoroutine(flashC);
        }

        SetMeshes(false);

        yield return null;
    }

    private IEnumerator Flash()
    {
        bool v = false;
        while(true)
        {
            v = !v;
            SetMeshes(v);
            yield return new WaitForSeconds(0.05f);
        }
    }

    private void SetMeshes (bool v)
    {
        if (null != img)
        {
            img.enabled = v;
        }
    }
}
