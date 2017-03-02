using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuyPageButton : SceneButton
{
    public ScreenOrientation orientationToChange;

    protected override void Function()
    {
        Screen.orientation = orientationToChange;

        pressed = true;

        SceneManager.LoadScene(scenePath, LoadSceneMode.Single);
    }
}
