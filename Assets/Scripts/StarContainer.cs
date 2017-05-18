using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarContainer : MonoBehaviour
{

    public Text value;
    public ProgressStar star;


    private void Awake()
    {
        value = GetComponentInChildren<Text>();
        star = GetComponentInChildren<ProgressStar>();
    }
}
