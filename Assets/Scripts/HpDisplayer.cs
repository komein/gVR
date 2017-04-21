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

    HpContainer[] containers;

    public void SetHpDisplay(int value, bool animated)
    {
        SetHpNoAnimation(value);

        if (animated)
        {
            if (currentHp > value)
            {
                containers[value].BreakTheHeart();
            }
            else if (currentHp < value)
            {
                containers[value - 1].MakeBiggerForSec();
            }

        }

        currentHp = value;
    }

    public void SetHpNoAnimation(int value)
    {
        if (value < 0 || value > 3)
        {
            return;
        }

        for (int i = 0; i < containers.Length; i++)
        {
            containers[i].SetHeart(value > i);
        }
    }

    void Start()
    {
        containers = GetComponentsInChildren<HpContainer>();
        deadFlag = false;

        if (null != DataObjects.GameController)
        {
            DataObjects.GameController.SetOptionalHpAction(UpdateHp);
        }

        Reinitialize();
    }

    public void UpdateHp()
    {
        if (deadFlag)
        {
            return;
        }

        if (null != DataObjects.GameController)
        {
            int hp = DataObjects.GameController.GetHp();
            SetHpDisplay(hp, true);
        }
    }

    public void Reinitialize()
    {
        if (null != DataObjects.GameController)
        {
            int hp = DataObjects.GameController.GetHp();
            SetHpDisplay(hp, false);
        }
    }
}
