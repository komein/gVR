using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class HpDisplayer : MonoBehaviour
{
    int lvl;
    bool deadFlag;
    int currentHp;

    HpContainer2[] containers;

    public void SetHpDisplay(int value)
    {
        if (value < 0 || value > 3)
        {
            return;
        }

        for (int i = 0; i < containers.Length; i++)
        {
            containers[i].SetHeart(value > i);
        }

        if (currentHp > value)
        {
            containers[value].BreakTheHeart();
        }
        else if (currentHp < value)
        {
            containers[value-1].MakeBiggerForSec();
        }

        currentHp = value;

    }

    void Start()
    {
        containers = GetComponentsInChildren<HpContainer2>();
        deadFlag = false;

        if (null != DataObjects.gameController)
        {
            DataObjects.gameController.SetOptionalHpAction(UpdateHp);
            UpdateHp();
        }
    }

    public void UpdateHp()
    {
        if (deadFlag)
        {
            return;
        }

        if (null != DataObjects.gameController)
        {
            int hp = DataObjects.gameController.GetHp();
            SetHpDisplay(hp);
        }
    }
}
