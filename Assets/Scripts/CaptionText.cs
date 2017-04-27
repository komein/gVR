using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptionText : MonoBehaviour
{

    TextMesh text;
    MeshRenderer mesh;

    public float upTime;
    public float floatStep;
    public Vector3 floatDirection = Vector3.up;

    Coroutine c = null;

    void Start () {
        text = GetComponent<TextMesh>();
        mesh = GetComponent<MeshRenderer>();
	}
	
    public void PlaceText(string t, Vector3 pos)
    {
        if (null != text)
        {
            text.text = t;
        }
        Toggle(true);
        transform.position = pos;

        if (null != c)
        {
            StopCoroutine(c);
        }

        c = StartCoroutine(FloatUp(upTime, floatStep, floatDirection));
    }

    public IEnumerator FloatUp(float time, float step, Vector3 direction)
    {
        float t = 0;
        while (t < time)
        {
            t += Time.deltaTime;

            float alpha = (time - t) / time;

            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);

            transform.position += direction.normalized * step;
            yield return new WaitForEndOfFrame();
        }

        Toggle(false);

        yield return null;
    }

    public void Toggle(bool v)
    {
        if (null != mesh)
        {
            mesh.enabled = v;
        }
    }

}
