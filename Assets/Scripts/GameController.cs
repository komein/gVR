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
            DataObjects.SceneInfo.multiplier = v;
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
        if (null == DataObjects.SavedGame)
            return false;

        TriggerOptionalScoreAction();

        return DataObjects.SavedGame.SetScore(level, s);
    }

    public bool AddScore(long s)
    {
        if (null == DataObjects.SavedGame)
        {
            return false;
        }

        DataObjects.SceneInfo.tempScore += (long)(s * DataObjects.SceneInfo.multiplier + 0.5f);

        TriggerOptionalScoreAction();

        return true;
    }

    public long GetScore(int level)
    {
        if (null == DataObjects.SavedGame)
            return -1;

        return DataObjects.SavedGame.GetScore(level);
    }

    public long GetMaxScore(int level)
    {
        if (null == DataObjects.SavedGame)
            return -1;

        return DataObjects.SavedGame.GetMaxScore(level);
    }

    public bool ResetScore()
    {
        if (null == DataObjects.SavedGame)
            return false;

        return DataObjects.SavedGame.ResetScore();
    }

    public void SetHp(int h)
    {
        DataObjects.SceneInfo.hp = h;
        TriggerOptionalHpAction();
    }

    public void AddHp(int v)
    {
        DataObjects.SceneInfo.hp += v;
        DataObjects.SceneInfo.hp = Mathf.Min(SceneInfo.HP_MAX, DataObjects.SceneInfo.hp);
        TriggerOptionalHpAction();
    }

    public void LoseHp(int v)
    {
        DataObjects.SceneInfo.hp -= v;
        DataObjects.SceneInfo.hp = Mathf.Max(0, DataObjects.SceneInfo.hp);
        TriggerOptionalHpAction();
    }

    public int GetHp()
    {
        return DataObjects.SceneInfo.hp;
    }

    internal void RestoreHp()
    {
        DataObjects.SceneInfo.hp = SceneInfo.HP_DEFAULT;
    }

    public void UpdateBestScore()
    {
        if (null != DataObjects.SceneInfo)
        {
            LevelInfo p = DataObjects.LevelInfo(SceneManager.GetActiveScene().name);
            if (p != null)
            {
                p.bestScoreRecord = Mathf.Max((int)p.bestScoreRecord, (int)DataObjects.SceneInfo.TempScore);
            }
        }
    }

    public void OnSceneChange()
    {
        if (null != DataObjects.SceneInfo && null != DataObjects.SavedGame && null != DataObjects.DataManager && null != DataObjects.GameController)
        {
            DataObjects.GameController.UpdateBestScore();
            SaveTempScore();

            DataObjects.SceneInfo.ResetLevel();

            optionalHpAction = null;
            optionalScoreAction = null;
        }
    }

    public void SaveTempScore()
    {
        if (null != DataObjects.SceneInfo && null != DataObjects.SavedGame && null != DataObjects.DataManager)
        {
            LevelInfo p = DataObjects.LevelInfo(SceneManager.GetActiveScene().name);
            if (p != null)
            {
                p.accumulatedScore += DataObjects.SceneInfo.TempScore;
                DataObjects.SceneInfo.tempScore = 0;
                DataObjects.SceneInfo.tempScoreSaved = 0;
            }
            DataObjects.DataManager.Save();
        }
    }

    public void OnPauseLevel()
    {
        if (null != DataObjects.SceneInfo && null != DataObjects.SavedGame && null != DataObjects.DataManager)
        {
            LevelInfo p = DataObjects.LevelInfo(SceneManager.GetActiveScene().name);
            if (p != null)
            {
                DataObjects.SceneInfo.tempScoreSaved += DataObjects.SceneInfo.tempScore;
                DataObjects.SceneInfo.tempScore = 0;
            }
            DataObjects.DataManager.Save();
        }
    }

}
