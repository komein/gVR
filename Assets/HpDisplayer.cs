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
    }

    void Start()
    {
        deadFlag = false;

        storage = FindObjectOfType<DataStorage>();
        if (null != storage)
        {
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
