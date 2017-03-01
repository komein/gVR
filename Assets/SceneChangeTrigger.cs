using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChangeTrigger : MonoBehaviour
{

    DataStorage storage;

	void Start () {
        storage = FindObjectOfType<DataStorage>();
	}

    private void OnDestroy()
    {
        storage.OnSceneChange();
    }
}
