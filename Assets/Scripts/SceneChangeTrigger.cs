using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChangeTrigger : MonoBehaviour
{
    private void OnDestroy()
    {
        if (null != DataObjects.gameController)
        {
            DataObjects.gameController.OnSceneChange();
        }
    }
}
