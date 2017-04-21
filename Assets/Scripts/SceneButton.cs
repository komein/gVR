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

        if (isActiveButton)
        {
            if (null != DataObjects.GameController && null != DataObjects.SceneInfo)
            {
                DataObjects.GameController.OnSceneChange();
                DataObjects.SceneInfo.title = scenePath;
            }
            SceneManager.LoadScene("loadingScreen", LoadSceneMode.Single);
        }

    }
}
