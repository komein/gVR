using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowRotator : MonoBehaviour
{
    Quaternion rotation;
    Sun sun;

    void Awake()
    {
        sun = FindObjectOfType<Sun>();
        if (null != sun)
        {
            rotation = sun.transform.rotation;
        }
        else
        {
            rotation = transform.rotation;
        }
    }

    void LateUpdate()
    {
        transform.rotation = rotation;
        //transform.localPosition = pos;
    }
}
