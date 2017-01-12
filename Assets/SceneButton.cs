using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButton : LookableButton {

    public string scenePath;

    protected override void Function()
    {
        base.Function();
        DataStorage scoreStorage = FindObjectOfType<DataStorage>();
        if (null != scoreStorage)
        {
            scoreStorage.Save();
        }

        SceneManager.LoadScene(scenePath, LoadSceneMode.Single);
    }
}
