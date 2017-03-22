using System.Collections.Generic;
using System.Xml.Serialization;

public class LevelInfo
{
    [XmlArray("StarRecords")]
    [XmlArrayItem("StarRecord")]
    public readonly List<long> starRecords;

    [XmlAttribute("accScore")]
    public long accumulatedScore = 0;
    [XmlAttribute("bestScore")]
    public long bestScoreRecord = 0;

    [XmlAttribute("maxScore")]
    public long maxScore
    {
        get;
        private set;
    }

    [XmlAttribute("number")]
    public int number
    {
        get;
        private set;
    }

    public string title
    {
        get
        {
            return "level" + number;
        }
    }

    public int starRecord
    {
        get
        {
            for (int i = starRecords.Count; i > 0; i--)
            {
                if (bestScoreRecord >= starRecords[i - 1])
                {
                    return i;
                }
            }
            return 0;
        }
    }

    public int GetStarRecord(long r)
    {
        for (int i = starRecords.Count; i > 0; i--)
        {
            if (r > starRecords[i - 1])
            {
                return i;
            }
        }
        return 0;
    }

    public LevelInfo()
    {
        number = 0;
        maxScore = 0;

        accumulatedScore = 0;
        bestScoreRecord = 0;
        starRecords = new List<long>();
    }

    public LevelInfo(int n, long max) : this()
    {
        number = n;
        maxScore = max;
    }

    public LevelInfo(int n, long max, float osr, float twsr, float thsr) : this(n, max)
    {
        starRecords.Add((long)(maxScore * osr));
        starRecords.Add((long)(maxScore * twsr));
        starRecords.Add((long)(maxScore * thsr));
    }

}