using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarContainer : MonoBehaviour {

    public Text value;
    public ProgressStar star;

    public void HightlightStar()
    {
        if (null != star)
        {
            star.Fill();
        }
    }
}
