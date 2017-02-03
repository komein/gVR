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
        if (!isValidLevel(level))
            return false;

        int levelIndex = level - 1;

        if (levelIndex == 0) // first level
            return true;

        LevelProgress previousLevel = GetLevel(level - 1);

        if (null == previousLevel)
            return false;

        if (previousLevel.score >= previousLevel.maxScore)
                return true;

        return false;
    }

    private bool isValidLevel(int level)
    {
        if (null == levels)
            return false;

        if (levels.Count < level)
            return false;

        int levelIndex = level - 1;
        if (levelIndex < 0)
            return false;

        return true;
    }

    public LevelProgress GetLevel(int level)
    {
        if (!isValidLevel(level))
            return null;

        int levelIndex = level - 1;

        return levels[levelIndex];
    }

    public bool SetScore(int level, long score)
    {
        LevelProgress l = GetLevel(level);

        if (null == l)
            return false;

        l.score = score;

        return true;
    }

    public long GetMaxScore(int level)
    {
        LevelProgress l = GetLevel(level);

        if (null == l)
            return -1;

        return l.maxScore;
    }

    public long GetScore(int level)
    {
        LevelProgress l = GetLevel(level);

        if (null == l)
            return -1;

        return l.score;
    }

    public bool AddScore(int level, int score)
    {
        long s = GetScore(level);
        return SetScore(level, s + score);
    }

    internal bool ResetScore()
    {
        if (null == levels)
            return false;

        foreach (var v in levels)
            v.score = 0;

        return true;
    }
}

[System.Serializable]
public class LevelProgress
{
    public long score;
    public long maxScore // readonly
    {
        get;
        private set;
    }

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
    public InAppManager inAppManagerPrefab;

    private static DataStorage instanceRef;
    private InAppManager manager;

    public Game savedGame;

    public LevelInfo levelInfo = new LevelInfo();

    private int hp;

    const int maxHp = 3;

    public const int lastFreeLevelNumber = 1;
    public const bool purchaseMode = true;

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

    void Awake()
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

        if (null == manager)
        {
            manager = Instantiate(inAppManagerPrefab);
        }

        multiplier = 1;

        RestoreHp();
        Load();
    }

    private void Start()
    {
        Screen.SetResolution(1280, 720, true);
    }

    public int GetCurrentLevel()
    {
        if (null == levelInfo)
            return -1;

        return levelInfo.number;
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

    public void SetOptionalHpAction(Action a)
    {
        optionalHpAction = a;
    }

    public bool SetScore(int level, long s)
    {
        if (null == savedGame)
            return false;

        savedGame.SetScore(level, s);

        Save();
        OptionalScoreAction();

        return true;
    }

    public bool AddScore(int level, long s)
    {
        if (null == savedGame)
            return false;

        savedGame.AddScore(level, (int)(s * multiplier + 0.5f));

        Save();
        OptionalScoreAction();

        return true;
    }

    public long GetScore(int level)
    {
        if (null == savedGame)
            return -1;

        return savedGame.GetScore(level);
    }

    public long GetMaxScore(int level)
    {
        if (null == savedGame)
            return -1;

        return savedGame.GetMaxScore(level);
    }

    public bool ResetScore()
    {
        if (null == savedGame)
            return false;

        return savedGame.ResetScore();
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
        FileStream file = File.Create(Application.persistentDataPath + "/s3.bin");
        bf.Serialize(file, savedGame);
        file.Close();
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/s3.bin"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/s3.bin", FileMode.Open);
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

    internal bool LevelsArePurchased()
    {

        return manager.IsProductBought(InAppManager.pLevels);
    }

    internal bool PurchaseLevel()
    {

        return manager.BuyProductID(InAppManager.pLevels);
    }

    internal void SetActions(Action setSuccessMessage, Action setFailMessage)
    {

        manager.SetActions(setSuccessMessage, setFailMessage);
    }
}
