using System;

public class SceneInfo
{
    public const int HP_DEFAULT = 2;
    public const int HP_MAX = 3;

    public string title;
    public long tempScore;
    public int hp;
    public float multiplier;

    public SceneInfo()
    {
        title = "notitle";
        tempScore = 0;
        hp = HP_DEFAULT;
        multiplier = 1;
    }

    internal void Reset()
    {
        multiplier = 1;
        hp = HP_DEFAULT;
    }
}