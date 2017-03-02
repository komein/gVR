using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class HpDisplayer : MonoBehaviour
{
    DataStorage storage;

    int lvl;

    bool deadFlag;

    int currentHp;

    HpContainer2[] containers;

    bool[,] hpMatrix = {
        { false, false, false },
        { true, false, false },
        { true, true, false },
        { true, true, true } };

    public void SetHp(int value)
    {
        if (containers.Length < 2)
            Debug.LogError("Hp init fuckup");

        if (value < 0 || value > 3)
            return;

        containers[0].SetHeart(hpMatrix[value, 0]);
        containers[1].SetHeart(hpMatrix[value, 1]);
        containers[2].SetHeart(hpMatrix[value, 2]);

        if (currentHp > value)
        {
            switch (currentHp)
            {
                case 1:
                    containers[0].BreakTheHeart();
                    break;
                case 2:
                    containers[1].BreakTheHeart();
                    break;
                case 3:
                    containers[2].BreakTheHeart();
                    break;
                default:
                    return;
            }
        }
        else if (currentHp < value)
        {
            switch (currentHp)
            {
                case 1:
                    containers[1].MakeBiggerForSec();
                    break;
                case 2:
                    containers[2].MakeBiggerForSec();
                    break;
                default:
                    return;
            }
        }

        currentHp = value;

    }

    void Start()
    {
        containers = GetComponentsInChildren<HpContainer2>();
        deadFlag = false;

        storage = FindObjectOfType<DataStorage>();
        if (null != storage)
        {
            currentHp = storage.GetHp();

            SetHp(currentHp);

            storage.SetOptionalHpAction(UpdateHp);
        }
    }

    public void UpdateHp()
    {
        if (deadFlag)
            return;

        if (null != storage)
        {
            int hp = storage.GetHp();
            SetHp(hp);
        }
    }
}
