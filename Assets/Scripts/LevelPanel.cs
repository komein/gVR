﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class LevelPanelInfo
{
    public string title;
}

public class LevelPanel : MonoBehaviour
{
    public List<LevelPanelInfo> levels;

    List<LevelButton> buttons;

	// Use this for initialization
	void Awake ()
    {
        if (null != levels)
        {
            buttons = GetComponentsInChildren<LevelButton>().ToList();
            for (int i = 0; i < levels.Count; i++)
            {
                int n = i + 1;
                if (buttons.Count >= n)
                {
                    LevelButton c = buttons[i];
                    c.caption = levels[i].title;
                    c.pressedCaption = c.caption;
                    c.scenePath = "level" + n.ToString();
                    c.Initialize();
                    c.transform.SetAsLastSibling();
                }
            }
        }
	}

    void Start()
    {
        if (null != buttons)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].transform.SetAsLastSibling();
            }
        }
    }
}
