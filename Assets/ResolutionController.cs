using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionController : MonoBehaviour {

    public int width = 1920;
    public int height = 1080;

	void Start () {
        Screen.SetResolution(width, height, true);
    }
}
