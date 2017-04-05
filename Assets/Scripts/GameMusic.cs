
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class MusicUnit
{
    public string level;
    public AudioClip music;
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

    internal AudioClip GetMusic(string v)
    {
        MusicUnit m = sceneMusics.Find(p => p.level == v);

        if (null != m)
        {
            return m.music;
        }

        return null;
    }

    internal AudioClip GetRandomMusic(string title)
    {
        List <MusicUnit> m = sceneMusics.FindAll(p => p.level.Contains(title));

        if (null != m)
        {
            MusicUnit toGet = m[Random.Range(0, m.Count)];
            if (null != toGet)
            {
                return toGet.music;
            }
        }

        return null;
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
