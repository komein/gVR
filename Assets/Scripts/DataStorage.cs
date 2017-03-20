using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Game
{
    public List<LevelInfo> levels;

    public Game(List<LevelInfo> l)
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

        LevelInfo previousLevel = GetLevel(level - 1);

        if (null == previousLevel)
            return false;

        if (previousLevel.accumulatedScore >= previousLevel.maxScore)
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

    public LevelInfo GetLevel(int level)
    {
        if (!isValidLevel(level))
            return null;

        int levelIndex = level - 1;

        return levels[levelIndex];
    }

    public LevelInfo GetLevelByName(string name)
    {
        return levels.Find(p => p.title == name);
    }

    public bool SetScore(int level, long score)
    {
        LevelInfo l = GetLevel(level);

        if (null == l)
            return false;

        l.accumulatedScore = score;

        return true;
    }

    public long GetMaxScore(int level)
    {
        LevelInfo l = GetLevel(level);

        if (null == l)
            return -1;

        return l.maxScore;
    }

    public long GetScore(int level)
    {
        LevelInfo l = GetLevel(level);

        if (null == l)
            return -1;

        return l.accumulatedScore;
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
        {
            v.accumulatedScore = 0;
            v.bestScoreRecord = 0;
        }

        return true;
    }

    internal int GetStarRecord(int level)
    {
        LevelInfo l = GetLevel(level);

        if (null == l)
            return 0;

        return l.starRecord;
    }

}

[System.Serializable]
public class LevelInfo
{
    public string title
    {
        get
        {
            return "level" + number;
        }
    }

    public int number;

    public int starRecord // readonly
    {
        get
        {
            if (bestScoreRecord > threeStarRecord)
            {
                return 3;
            }
            else if (bestScoreRecord > twoStarRecord)
            {
                return 2;
            }
            else if (bestScoreRecord > oneStarRecord)
            {
                return 1;
            }
            return 0;
        }
    }

    public readonly long oneStarRecord;
    public readonly long twoStarRecord;
    public readonly long threeStarRecord;

    public long accumulatedScore = 0;
    public long bestScoreRecord;

    public long maxScore // readonly
    {
        get;
        private set;
    }

    public LevelInfo(int n, long max)
    {
        accumulatedScore = 0;
        
        number = n;
        maxScore = max;
    }

    public LevelInfo(int n, long max, float osr, float twsr, float thsr) : this (n, max)
    {
        oneStarRecord = (long)(maxScore * osr);
        twoStarRecord = (long)(maxScore * twsr);
        threeStarRecord = (long)(maxScore * thsr);
    }

}

public class SceneInfo
{
    public string title = "notitle";
    public long tempScore = 0;
}

public class DataStorage : MonoBehaviour
{
    public InAppManager inAppManagerPrefab;

    private static DataStorage instanceRef;
    private static InAppManager manager;

    public Game savedGame;

    public SceneInfo levelInfo = new SceneInfo();

    private int hp;

    const int maxHp = 3;

    const int hpDefaultValue = 2;

    const int LVL_1_SCORE = 50;
    const int LVL_2_SCORE = 60;
    const int LVL_3_SCORE = 70;

    public const int lastFreeLevelNumber = 1;
    public const bool purchaseMode = true;

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
            DontDestroyOnLoad(manager);
        }

        multiplier = 1;

        RestoreHp();
        LoadWithoutAction();
    }

    public SceneInfo GetCurrentLevel()
    {
        return levelInfo;
    }
    
    internal void SetMultiplier(int v)
    {
        if (v >= 1)
            multiplier = v;
        OptionalScoreAction();
    }

    public void OptionalScoreAction()
    {
        if (null != optionalScoreAction)
            optionalScoreAction();
    }

    public void OptionalHpAction()
    {
        if (null != optionalHpAction)
            optionalHpAction();
    }

    public void SetOptionalScoreAction(Action a)
    {
        if (null == optionalScoreAction)
            optionalScoreAction = a;
        else
            optionalScoreAction += a;
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
        
        OptionalScoreAction();

        return true;
    }

    public bool AddScore(long s)
    {
        if (null == savedGame)
            return false;

        levelInfo.tempScore += (int)(s * multiplier + 0.5f);
        
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

    public int GetStarRecord(int level)
    {
        if (null == savedGame)
            return 0;

        return savedGame.GetStarRecord(level);
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
        hp = Mathf.Min(maxHp, hp);
        OptionalHpAction();
    }

    public void LoseHp(int v)
    {
        hp -= v;
        hp = Mathf.Max(0, hp);
        OptionalHpAction();
    }

    public int GetHp()
    {
        return hp;
    }
    
    internal void RestoreHp()
    {
        hp = hpDefaultValue;
    }

    public void Save()
    {
        if (null == savedGame)
            return;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/save.bin");
        bf.Serialize(file, savedGame);
        file.Close();
    }

    public void UpdateBestScore()
    {
        if (null != levelInfo)
        {
            LevelInfo p = savedGame.GetLevelByName(levelInfo.title);
            if (p != null)
            {
                if (p.bestScoreRecord < levelInfo.tempScore)
                {
                    p.bestScoreRecord = levelInfo.tempScore;
                }
            }
        }
    }
    public void OnSceneChange()
    {
        if (null != levelInfo)
        {
            LevelInfo p = savedGame.GetLevelByName(levelInfo.title);
            if (p != null)
            {
                UpdateBestScore();

                p.accumulatedScore += levelInfo.tempScore;

                levelInfo.tempScore = 0;
            }
        }

        multiplier = 1;
        RestoreHp();

        optionalHpAction = null;
        optionalScoreAction = null;

        Save();
    }

    public void Load()
    {
        LoadWithoutAction();

        OptionalScoreAction();
    }

    public void LoadWithoutAction()
    {
        if (File.Exists(Application.persistentDataPath + "/save.bin"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/save.bin", FileMode.Open);
            savedGame = (Game)bf.Deserialize(file);
            file.Close();
        }
        else
        {
            MakeNewSaveFile();
        }
    }

    public void MakeNewSaveFile()
    {
        List<LevelInfo> levels = new List<LevelInfo>();

        levels.Add(new LevelInfo(1, LVL_1_SCORE, 0.2f, 0.5f, 1f));
        levels.Add(new LevelInfo(2, LVL_2_SCORE, 0.3f, 0.8f, 1.5f));
        levels.Add(new LevelInfo(3, LVL_3_SCORE, 0.5f, 1f, 2f));

        savedGame = new Game(levels);

        Save();
        LoadWithoutAction();
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

    void LateUpdate()
    {
        if (null != GvrViewer.Instance)
        {
            GvrViewer.Instance.UpdateState();
            if (GvrViewer.Instance.BackButtonPressed)
            {
                Application.Quit();
            }
        }
    }

    private void OnApplicationQuit()
    {
        Save();
    }
}
