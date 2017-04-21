using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChangeTrigger : MonoBehaviour
{
    private void OnDestroy()
    {
        if (null != DataObjects.GameController)
        {
            DataObjects.GameController.OnSceneChange();
        }
    }

    public void WriteSomeShit()
    {
        Debug.Log("some shit!");
    }
}
