using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class GameController
{
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

    Action optionalScoreAction;
    Action optionalHpAction;

    internal void SetMultiplier(int v)
    {
        if (v >= 1)
            DataObjects.sceneInfo.multiplier = v;
        TriggerOptionalScoreAction();
    }

    public void TriggerOptionalScoreAction()
    {
        if (null != optionalScoreAction)
            optionalScoreAction();
    }

    public void TriggerOptionalHpAction()
    {
        if (null != optionalHpAction)
            optionalHpAction();
    }

    public void AddOptionalScoreAction(Action a)
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
        if (null == DataObjects.savedGame)
            return false;

        TriggerOptionalScoreAction();

        return DataObjects.savedGame.SetScore(level, s);
    }

    public bool AddScore(long s)
    {
        if (null == DataObjects.savedGame)
        {
            return false;
        }

        DataObjects.sceneInfo.tempScore += (long)(s * DataObjects.sceneInfo.multiplier + 0.5f);

        TriggerOptionalScoreAction();

        return true;
    }

    public long GetScore(int level)
    {
        if (null == DataObjects.savedGame)
            return -1;

        return DataObjects.savedGame.GetScore(level);
    }

    public long GetMaxScore(int level)
    {
        if (null == DataObjects.savedGame)
            return -1;

        return DataObjects.savedGame.GetMaxScore(level);
    }

    public int GetStarRecord(int level)
    {
        if (null == DataObjects.savedGame)
            return 0;

        return DataObjects.savedGame.GetStarRecord(level);
    }

    public bool ResetScore()
    {
        if (null == DataObjects.savedGame)
            return false;

        return DataObjects.savedGame.ResetScore();
    }

    public void SetHp(int h)
    {
        DataObjects.sceneInfo.hp = h;
        TriggerOptionalHpAction();
    }

    public void AddHp(int v)
    {
        DataObjects.sceneInfo.hp += v;
        DataObjects.sceneInfo.hp = Mathf.Min(SceneInfo.HP_MAX, DataObjects.sceneInfo.hp);
        TriggerOptionalHpAction();
    }

    public void LoseHp(int v)
    {
        DataObjects.sceneInfo.hp -= v;
        DataObjects.sceneInfo.hp = Mathf.Max(0, DataObjects.sceneInfo.hp);
        TriggerOptionalHpAction();
    }

    public int GetHp()
    {
        return DataObjects.sceneInfo.hp;
    }

    internal void RestoreHp()
    {
        DataObjects.sceneInfo.hp = SceneInfo.HP_DEFAULT;
    }

    public void UpdateBestScore()
    {
        if (null != DataObjects.sceneInfo)
        {
            LevelInfo p = DataObjects.savedGame.GetLevelByName(SceneManager.GetActiveScene().name);
            if (p != null)
            {
                if (p.bestScoreRecord < DataObjects.sceneInfo.tempScore)
                {
                    p.bestScoreRecord = DataObjects.sceneInfo.tempScore;
                }
            }
        }
    }

    public void OnSceneChange()
    {
        if (null != DataObjects.sceneInfo && null != DataObjects.savedGame && null != DataObjects.dataManager)
        {
            SaveTempScore();

            DataObjects.sceneInfo.ResetLevel();

            optionalHpAction = null;
            optionalScoreAction = null;
        }
    }

    public void SaveTempScore()
    {
        if (null != DataObjects.sceneInfo && null != DataObjects.savedGame && null != DataObjects.dataManager)
        {
            LevelInfo p = DataObjects.savedGame.GetLevelByName(SceneManager.GetActiveScene().name);
            if (p != null)
            {
                p.accumulatedScore += DataObjects.sceneInfo.tempScore;
                DataObjects.sceneInfo.tempScore = 0;
            }
            DataObjects.dataManager.Save();
        }
    }
}
