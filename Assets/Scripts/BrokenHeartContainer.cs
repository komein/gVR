using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrokenHeartContainer : MonoBehaviour {

    Image img;

    void Start()
    {
        img = GetComponentInChildren<Image>();
        SetMeshes(false);
    }

    public void BreakTheHeart()
    {
        StopAllCoroutines();
        StartCoroutine(ToggleFlash());
    }

    private IEnumerator ToggleFlash()
    {
        StartCoroutine(Flash());
        yield return new WaitForSeconds(0.5f);
        StopAllCoroutines();
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
