using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StarContainer : MonoBehaviour
{

    public TextMeshProUGUI value;
    public ProgressStar star;


    private void Awake()
    {
        value = GetComponentInChildren<TextMeshProUGUI>();
        star = GetComponentInChildren<ProgressStar>();
    }
}
