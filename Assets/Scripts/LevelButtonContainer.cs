using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelButtonContainer : MonoBehaviour {

    public string title;
    public int number;

    public LevelButton buttonPrefab;
    public LevelButton Button
    {
        get;
        private set;
    }
    
    public void Initialize()
    {
        if (null == GetComponentInChildren<LevelButton>())
        {
            if (null != buttonPrefab)
            {
                Button = Instantiate(buttonPrefab);
                Button.transform.SetParent(this.transform);
                RectTransform rectTransform = Button.GetComponent<RectTransform>();
                UIUtilities.FitToParent(rectTransform);
            }
        }

    }
}
