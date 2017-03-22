using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot("SavedGame")]
public class SavedGame
{
    [XmlArray("Levels")]
    [XmlArrayItem("LevelInfo")]
    public List<LevelInfo> levels;

    public SavedGame()
    {
        levels = new List<LevelInfo>();
    }

    public SavedGame(List<LevelInfo> l)
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