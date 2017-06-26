using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextFontUpdater : MonoBehaviour {

    Text text;

    int initialSize;

	// Use this for initialization
	void Start ()
    {
        text = GetComponent<Text>();
        initialSize = text.fontSize;
        UpdateFont();
	}
	

    public void UpdateFont()
    {
        DataObjects.Localization.SetFont(text, initialSize);
    }
}
