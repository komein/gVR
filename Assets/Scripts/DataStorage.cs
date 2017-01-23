using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Game
{
    public long score;

    public Game(long s)
    {
        score = s;
    }
}

public class DataStorage : MonoBehaviour {

    public string sceneToLoad = "";

    private long score = 0;
    private int hp;

    long lvl2Unlock = 100;
    long lvl3Unlock = 250;

    const int maxHp = 3;

    public float multiplier {
        get;
        private set;
    }

    Action optionalScoreAction;
    Action optionalHpAction;

    public bool isAlive
    {
        get
        {
            if (GetHp() <= 0)
            {
                return false;
            }
            return true;
        }
    }

    void Awake ()
    {
        if (null != FindObjectOfType<DataStorage>())
        {
            Destroy(gameObject);
            return;
        }

        multiplier = 1;
        RestoreHp();
        DontDestroyOnLoad(transform.gameObject);
    }

    public long GetNextLvlUnlock()
    {
        if (score < lvl2Unlock)
            return lvl2Unlock;
        else if (score < lvl3Unlock)
            return lvl3Unlock;
        else
            return -1;
    }

    public long GetCurrentLevelUnlock()
    {
        if (score < lvl2Unlock)
            return 0;
        else if (score < lvl3Unlock)
            return lvl2Unlock;
        else
            return lvl3Unlock;
    }

    public int GetCurrentLevel()
    {
        if (score < lvl2Unlock)
            return 1;
        if (score < lvl3Unlock)
            return 2;
        return 3;
    }

    public void SetScore(long s)
    {
        score = s;
        Save();
        OptionalAction();
    }

    internal void SetMultiplier(int v)
    {
        if (v >= 1)
            multiplier = v;
        OptionalAction();
    }


    private void OptionalAction()
    {
        if (null != optionalScoreAction)
            optionalScoreAction();
    }

    private void OptionalHpAction()
    {
        if (null != optionalHpAction)
            optionalHpAction();
    }

    public void SetOptionalAction(Action a)
    {
        optionalScoreAction = a;
    }

    internal bool IsMaxLvl()
    {
        return GetCurrentLevel() == 3;
    }

    public void SetOptionalHpAction(Action a)
    {
        optionalHpAction = a;
    }

    public void AddScore(long s)
    {
        score += (int)(s * multiplier + 0.5f);
        Save();
        OptionalAction();
    }

    public long GetScore()
    {
        return score;
    }

    public void SetHp(int h)
    {
        hp = h;
        OptionalHpAction();
    }

    public void AddHp(int v)
    {
        hp += v;
        OptionalHpAction();
    }

    public void LoseHp(int v)
    {
        hp -= v;
        hp = Mathf.Max(0, hp);
        OptionalHpAction();
        OptionalAction();
    }

    public int GetHp()
    {
        return hp;
    }
    
    internal void RestoreHp()
    {
        hp = maxHp;
    }

    public void Save()
    {
        Game g = new Game(score);
        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new BinaryFormatter();
        System.IO.FileStream file = File.Create(Application.persistentDataPath + "/save14.bin");
        bf.Serialize(file, g);
        file.Close();
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/save14.bin"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/save14.bin", FileMode.Open);
            Game g = (Game)bf.Deserialize(file);
            file.Close();

            score = g.score;
        }
        else
            score = 0;

        OptionalAction();
    }
}
