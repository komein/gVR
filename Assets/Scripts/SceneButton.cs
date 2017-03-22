using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButton : LookableButton
{
    public string scenePath;
    protected override void Start()
    {
        base.Start();

    }
    protected override void Function()
    {
        base.Function();

        if (null != DataObjects.gameController && null != DataObjects.sceneInfo)
        {
            DataObjects.gameController.OnSceneChange();
            DataObjects.sceneInfo.title = scenePath;
        }

        SceneManager.LoadScene("loadingScreen", LoadSceneMode.Single);
    }
}
