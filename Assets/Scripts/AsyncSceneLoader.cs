using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AsyncSceneLoader : MonoBehaviour
{
    public Image pBar;
    Text loadMessage;

    private AsyncOperation async = null;
    private IEnumerator LoadALevel(string levelName)
    {
        async = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);
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

        if (Camera.main != null)
        {
            //transform.SetParent(Camera.main.gameObject.transform, true);
        }

        if (null != DataObjects.sceneInfo)
        {
            LoadLevel(DataObjects.sceneInfo.title);
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

            if (null != pBar)
            {
                pBar.fillAmount = p;
            }

            if (null != loadMessage)
            {
                loadMessage.text = "Loading progress: " + (p * 100) + "%";
            }
        }
    }
}
