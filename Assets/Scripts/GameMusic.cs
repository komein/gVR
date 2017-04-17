
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class MusicUnit
{
    public string level;
    public AudioClip music;
    public float volume = 1f;
}

public class GameMusic : MonoBehaviour
{
    public static GameMusic instanceRef; // singleton pattern

    public List<MusicUnit> sceneMusics;

    private void Awake()
    {
        if (instanceRef == null)
        {
            instanceRef = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    internal void SetMusic(string v, AudioSource src)
    {
        if (null != src)
        {
            MusicUnit m = sceneMusics.Find(p => p.level == v);

            if (null != m)
            {
                src.clip = m.music;
                src.volume = m.volume;
            }
        }
    }

    internal void SetRandomMusic(string title, AudioSource src)
    {
        if (null != src)
        {
            List<MusicUnit> list = sceneMusics.FindAll(p => p.level.Contains(title));

            if (null != list)
            {
                MusicUnit toGet = list[Random.Range(0, list.Count)];
                if (null != toGet)
                {
                    src.clip = toGet.music;
                    src.volume = toGet.volume;
                }
            }

        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelLoaded;
    }

    internal void Play(string v)
    {
        MusicUnit music = sceneMusics.Find(p => p.level == v);

        if (null != music)
        {
            Camera c = Camera.main;

            if (null != c)
            {
                AudioSource aus = c.GetComponent<AudioSource>();

                if (null == aus)
                {
                    aus = c.gameObject.AddComponent<AudioSource>();
                }

                aus.clip = music.music;
                aus.Play();
            }
        }
        else
        {
            if (SceneManager.GetActiveScene().name != "loadingScreen")
            {
                Debug.Log("Music for title '" + v + "' is not found");
            }
        }
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelLoaded;
    }

    void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        string levelTitle = SceneManager.GetActiveScene().name;

        Play(levelTitle);
    }
}
