using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenHeartContainer : MonoBehaviour {

    MeshRenderer [] meshes;

    void Start()
    {
        meshes = GetComponentsInChildren<MeshRenderer>();
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
        foreach (MeshRenderer m in meshes)
        {
            m.enabled = v;
        }
    }
}
