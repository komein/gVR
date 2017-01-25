using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Game
{
    public List<LevelProgress> levels;

    public Game(List<LevelProgress> l)
    {
        levels = l;
    }

    public bool isLevelUnlocked(int level)
    {
        int actualLevel = level - 1;
        if (actualLevel < 0)
            return false;

        if (null == levels)
            return false;

        if (levels.Count < level)
            return false;

        if (actualLevel == 0) // first level
            return true;

        if (actualLevel > 0) // check if previous is completed
            if (levels[actualLevel - 1].score >= levels[actualLevel - 1].maxScore)
                return true;

        return false;
    }
}

[System.Serializable]
public class LevelProgress
{
    public long score;
    public long maxScore;

    public LevelProgress(long s, long max)
    {
        score = s;
        maxScore = max;
    }

}

public class LevelInfo
{
    public string title;
    public int number;
}

public class DataStorage : MonoBehaviour
{
    private static DataStorage instanceRef;

    public Game savedGame;

    public LevelInfo levelInfo = new LevelInfo();

    private int hp;

    const int maxHp = 3;

    public float multiplier {
        get;
        private set;
    }

    Action optionalScoreAction;
    Action optionalHpAction;
    private ScoreDisplayer displayer;

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
        // singleton pattern
        if (instanceRef == null)
        {
            instanceRef = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }

        multiplier = 1;
        RestoreHp();
        Load();
    }

    public int GetCurrentLevel()
    {
        if (null == levelInfo)
            return -1;

        return levelInfo.number;
    }

    public void SetScore(int level, long s)
    {
        if (!saveIsOk())
            return;

        savedGame.levels[level].score = s;

        Save();
        OptionalScoreAction();
    }

    internal void SetMultiplier(int v)
    {
        if (v >= 1)
            multiplier = v;
        OptionalScoreAction();
    }


    private void OptionalScoreAction()
    {
        if (null == displayer)
            return;

        if (null != optionalScoreAction)
            optionalScoreAction();
    }

    private void OptionalHpAction()
    {
        if (null != optionalHpAction)
            optionalHpAction();
    }

    public void SetOptionalAction(ScoreDisplayer d, Action a)
    {
        this.displayer = d;
        optionalScoreAction = a;
        OptionalScoreAction();
    }

    internal bool IsMaxLvl(int lvl)
    {
        if (!saveIsOk())
            return false;
        if (savedGame.levels.Count == (lvl - 1))
            return true;

        return false;
    }

    public void SetOptionalHpAction(Action a)
    {
        optionalHpAction = a;
    }

    public void AddScore(int lvl, long s)
    {
        SetScore(lvl, GetScore(lvl) + (int)(s * multiplier + 0.5f));
    }

    public long GetScore(int level)
    {
        if (!saveIsOk())
            return -1;

        return savedGame.levels[level].score;
    }

    public long GetMaxScore(int level)
    {
        if (!saveIsOk())
            return -1;

        if (level <= 0)
            return -1;

        return savedGame.levels[level-1].maxScore;
    }

    public void ResetScore()
    {
        if (!saveIsOk())
            return;

        foreach (var v in savedGame.levels)
            v.score = 0;
    }

    public bool saveIsOk()
    {
        if (null == savedGame)
            return false;

        return true;
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
        OptionalScoreAction();
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
        if (null == savedGame)
            return;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/s2.bin");
        bf.Serialize(file, savedGame);
        file.Close();
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/s2.bin"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/s2.bin", FileMode.Open);
            savedGame = (Game)bf.Deserialize(file);
            file.Close();
        }
        else
        {
            List<LevelProgress> levels = new List<LevelProgress>();

            levels.Add(new LevelProgress(0, 200));
            levels.Add(new LevelProgress(0, 300));
            levels.Add(new LevelProgress(0, 400));

            savedGame = new Game(levels);
            Save();
        }

        OptionalScoreAction();
    }
}
