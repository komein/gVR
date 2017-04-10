using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIUtilities
{

    public static void FitToParent(RectTransform rectTransform)
    {
        if (null != rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
        }
    }

    public static void FitToParent(GameObject go)
    {
        RectTransform rectTransform = go.GetComponent<RectTransform>();
        FitToParent(rectTransform);
    }
}
