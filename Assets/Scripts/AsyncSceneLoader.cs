using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AsyncSceneLoader : MonoBehaviour
{
    public Image pBar;
    Text loadMessage;

    public string loading_id;

    private AsyncOperation async = null;
    private IEnumerator LoadALevel(string levelName)
    {
        Application.backgroundLoadingPriority = ThreadPriority.Low;
        async = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);
        async.allowSceneActivation = false;

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

        if (null != DataObjects.SceneInfo)
        {
            LoadLevel(DataObjects.SceneInfo.title);
        }
    }

    public void ActivateScene()
    {
        async.allowSceneActivation = true;
    }
    
    private void Update()
    {
        if (null != async)
        {
            float p = async.progress;

            UpdateUI(p / 0.9f);

            if (p >= 0.9f)
            {
                async.allowSceneActivation = true;
            }
        }

    }

    private void UpdateUI(float p)
    {
        if (null != pBar)
        {
            pBar.fillAmount = p;
        }

        if (null != loadMessage)
        {
            loadMessage.text = DataObjects.Localization.GetField(loading_id) + ": " + (p * 100f).ToString("F2") + "%";
        }
    }
}
