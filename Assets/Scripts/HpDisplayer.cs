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

    public HpContainer hp1;
    public HpContainer hp2;
    public HpContainer hp3;

    public BrokenHeartContainer bhp1;
    public BrokenHeartContainer bhp2;
    public BrokenHeartContainer bhp3;

    int currentHp;

    bool[,] hpMatrix = {
        { false, false, false },
        { true, false, false },
        { true, true, false },
        { true, true, true } };

    public void SetHp(int value)
    {
        if (value < 0 || value > 3)
            return;

        hp1.Set(hpMatrix[value, 0]);
        hp2.Set(hpMatrix[value, 1]);
        hp3.Set(hpMatrix[value, 2]);

        if (currentHp > value)
        {
            switch (currentHp)
            {
                case 1:
                    bhp1.BreakTheHeart();
                    break;
                case 2:
                    bhp2.BreakTheHeart();
                    break;
                case 3:
                    bhp3.BreakTheHeart();
                    break;
                default:
                    return;
            }
        }
        else
        {
            switch (currentHp)
            {
                case 1:
                    hp2.MakeBiggerForSec();
                    break;
                case 2:
                    hp3.MakeBiggerForSec();
                    break;
                default:
                    return;
            }
        }

        currentHp = value;

    }

    void Start()
    {
        deadFlag = false;

        storage = FindObjectOfType<DataStorage>();
        if (null != storage)
        {
            currentHp = storage.GetHp();

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
