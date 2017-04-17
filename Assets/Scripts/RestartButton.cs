using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButton : SceneButton
{
    protected override void Start()
    {
        base.Start();
        scenePath = SceneManager.GetActiveScene().name;
    }
}
