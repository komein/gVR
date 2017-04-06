using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowRotator : MonoBehaviour
{
    Quaternion rotation;
    Sun sun;

    Vector3 pos;

    void Awake()
    {
        sun = FindObjectOfType<Sun>();
        if (null != sun)
        {
            rotation = sun.transform.rotation;
            pos = rotation * Vector3.up;
        }
        else
        {
            rotation = transform.rotation;
            pos = Vector3.up;
        }
    }

    void LateUpdate()
    {
        transform.rotation = rotation;
        transform.localPosition = pos;
    }
}
