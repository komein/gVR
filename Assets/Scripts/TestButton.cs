using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestButton : MonoBehaviour {

    Image img;
	// Use this for initialization
	void Start () {
        img = GetComponent<Image>();
	}
	
    public void OnGazeEnter()
    {
        img.color = Color.red;
    }

    public void OnGazeExit()
    {
        img.color = Color.white;
    }
}
