using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpContainer : MonoBehaviour {

    MeshRenderer mesh;

	void Start () {
        mesh = GetComponent<MeshRenderer>();	
	}
	
    public void Set(bool value)
    {
        if (null != mesh)
        {
            mesh.enabled = value;
        }
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
}
