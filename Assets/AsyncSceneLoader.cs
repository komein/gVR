using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AsyncSceneLoader : MonoBehaviour
{
    Camera cam;

    public Image pBar;
    Text loadMessage;

    DataStorage storage;


    private AsyncOperation async = null;
    private IEnumerator LoadALevel(string levelName)
    {
        async = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);
        //async.allowSceneActivation = false;
        yield return async;
    }

    public void LoadLevel(string s)
    {
        StopAllCoroutines();
        StartCoroutine(LoadALevel(s));
    }

    void Start()
    {
        loadMessage = GetComponentInChildren<Text>();

        if (cam == null)
        {
            cam = Camera.main;
        }

        if (cam != null)
        {
            transform.SetParent(cam.GetComponent<Transform>(), true);
        }

        storage = FindObjectOfType<DataStorage>();

        if (null != storage)
        {
            LoadLevel(storage.sceneToLoad);
        }
    }

    public void ActivateScene()
    {
        async.allowSceneActivation = true;
    }

    private void FixedUpdate()
    {
        if (null != async)
        {
            float p = async.progress;
            pBar.fillAmount = p;
            loadMessage.text = "Loading progress: " + (p * 100) + "%";
        }
    }
}
