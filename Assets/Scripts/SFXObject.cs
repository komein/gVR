using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXObject : MonoBehaviour {

    public string title;
    public float volume = 1f;

    protected AudioSource aus;

    protected virtual void Start()
    {
        aus = GetComponent<AudioSource>();

        if (null == aus)
        {
            aus = gameObject.AddComponent<AudioSource>();
        }
        aus.volume = volume;
    }

    private void ChooseClip()
    {
        GameMusic gm = DataObjects.MusicStorage;
        if (null != gm)
        {
            gm.SetRandomMusic(title, aus);
        }
    }

    protected bool PlaySound()
    {
        ChooseClip();
        if (null != aus)
        {
            if (null != aus.clip)
            {
                aus.Play();
                return true;
            }
        }
        return false;
    }
}
