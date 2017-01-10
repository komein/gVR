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

    private long score = 0;
    private int hp;

    long lvl2Unlock = 100;
    long lvl3Unlock = 250;

    const int maxHp = 3;

    Action optionalDisplayAction;

    void Awake ()
    {
        RestoreHp();
        DontDestroyOnLoad(transform.gameObject);
    }

    public long GetLvlUnlock()
    {
        if (score < lvl2Unlock)
            return lvl2Unlock;
        else if (score < lvl3Unlock)
            return lvl3Unlock;
        else
            return -1;
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

    private void OptionalAction()
    {
        if (null != optionalDisplayAction)
            optionalDisplayAction();
    }
    
    public void SetOptionalAction(Action a)
    {
        optionalDisplayAction = a;
    }

    public void AddScore(long s)
    {
        score += s;
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
        OptionalAction();
    }

    public void AddHp(int v)
    {
        hp += v;
        OptionalAction();
    }

    public void LoseHp(int v)
    {
        hp -= v;
        hp = Mathf.Max(0, hp);
        OptionalAction();
    }

    public int GetHp()
    {
        return hp;
    }
    
    internal void RestoreHp()
    {
        hp = maxHp;
        OptionalAction();
    }

    public void Save()
    {
        Game g = new Game(score);
        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new BinaryFormatter();
        System.IO.FileStream file = File.Create(Application.persistentDataPath + "/save.bin");
        bf.Serialize(file, g);
        file.Close();
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/save.bin"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/save.bin", FileMode.Open);
            Game g = (Game)bf.Deserialize(file);
            file.Close();

            score = g.score;
            OptionalAction();
        }
        else
            score = 0;
    }
}
