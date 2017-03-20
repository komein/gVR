using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarContainer : MonoBehaviour {

    public Text value;
    public ProgressStar star;

    public void HightlightStarAnimated()
    {
        if (null != star)
        {
            star.FillAnimated();
        }
    }

    public void HightlightStarNoAnimation()
    {
        if (null != star)
        {
            star.FillNoAnimation();
        }
    }

    public void UnhightlightStar()
    {
        if (null != star)
        {
            star.MakeEmpty();
        }
    }
}
