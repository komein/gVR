using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreDisplayer : MonoBehaviour, IUICanReinitialize
{
    public string levelTitle;

    public Image pBar;
    public Image pBarBackground;
    public Image pBarTemp;

    public TextMeshProUGUI status;
    public StarProgressBar optionalBar;

    public Color minColor;
    public Color midColor;
    public Color maxColor;
    public Color completeColor;

    public float congratulationDelay = 5f;

    GameManager storage;
    LevelInfo level;

    public bool mainMenuMode = true;
    public bool AutoUpdate = true;

    int currentStarRecord = 0;

    public long SavedScore
    {
        get
        {
            if (null != level)
                return level.accumulatedScore;
            return -1;
        }
    }

    long CurrentPlayScore // starts from 0 after every pause
    {
        get
        {
            return DataObjects.sceneInfo.tempScore;
        }
    }

    long CurrentLevelScore // 
    {
        get
        {
            return DataObjects.sceneInfo.tempScoreSaved;
        }
    }

    long MaxLevelScore
    {
        get
        {
            if (null != level)
                return level.maxScore;
            return -1;
        }
    }


    // assuming CurrentLevelScore += CurrentPlayScore after any kind of pause, so this thing is true only for one check
    public bool IsLevelCompleted
    {
        get
        {
            if (SavedScore + CurrentLevelScore < MaxLevelScore)
            {
                if (SavedScore + CurrentLevelScore + CurrentPlayScore >= MaxLevelScore)
                {
                    return true;
                }
            }
            return false;
        }
    }

    void Start()
    {
        storage = DataObjects.gameManager;

        UpdateLevelInfo();
        FillStarsNoAnimation();

        LoadScore();
    }

    private void FillStarsNoAnimation()
    {
        if (null != level)
        {
            if (null != optionalBar)
            {
                optionalBar.FillStarsNoAnimation(level.GetStarRecord(CurrentLevelScore + CurrentPlayScore));
            }
        }
    }

    // if on game level, uses SceneInfo to get current level info;
    // if in mainMenu, asks the info about level according to
    // value of levelTitle, which must be set in inspector therefore
    public void UpdateLevelInfo()
    {
        SavedGame game = DataObjects.savedGame;

        if (null != game)
        {
            level = game.GetLevelByName(SceneManager.GetActiveScene().name);

            if (null == level)
            {
                level = game.GetLevelByName(levelTitle);
            }
        }
    }

    private void LoadScore()
    {
        if (null != DataObjects.gameController && null != DataObjects.dataManager)
        {
            DataObjects.dataManager.LoadWithoutAction();
            if (AutoUpdate)
            {
                DataObjects.gameController.AddOptionalScoreAction(UpdateTextWithCheck);
                DataObjects.gameController.TriggerOptionalScoreAction();
            }
            else
            {
                UpdateText();
            }
        }
    }

    internal void ShowScoreProgressBarAnimated(float t, bool hideMultiplier = false, float time = 1f)
    {
        ShowScoreProgressBarAnimated(t, SavedScore + CurrentLevelScore, MaxLevelScore, CurrentPlayScore, time, hideMultiplier);
    }

    internal void ShowScoreProgressBarAnimated(float t, long acc, long max, long tmp, float time = 1f, bool hideMultiplier = false)
    {
        if (null != pBarBackground)
        {
            pBarBackground.enabled = true;
        }

        if (null == pBar || null == pBarTemp || null == level || null == storage)
        {
            return;
        }

        if (acc >= max || t < 0)
        {
            return;
        }

        pBarTemp.enabled = true;

        if (max == 0)
        {
            max = 1; // just to be sure
        }

        float pBarFill = acc / (float)(max);

        ShowScore(tmp + acc, max, hideMultiplier);

        float pBarTempFill = tmp * t / time / (float)(max);

        DrawProgressBar(pBar, 0, 1, pBarFill);
        DrawProgressBar(pBarTemp, 0, 1, pBarFill + pBarTempFill);
    }

    public void UpdateTextWithCheck()
    {
        if (IsLevelCompleted)
        {
            DataObjects.gameManager.PauseLevel(PauseType.win);
        }

        UpdateText();
    }

    public void UpdateText()
    {

        if (null == level)
        {
            return;
        }

        if (null != DataObjects.gameController)
        {
            if (DataObjects.gameController.isAlive)
            {
                ShowScoreProgressBar(SavedScore + CurrentLevelScore, CurrentPlayScore, MaxLevelScore);
            }
            
            if (null != optionalBar)
            {
                int record = level.GetStarRecord(CurrentPlayScore + CurrentLevelScore);
                if (currentStarRecord < record)
                {
                    currentStarRecord = record;
                    optionalBar.FillStarsAnimated(currentStarRecord);
                }
            }
        }
    }

    private void ShowScore(long score, long max, bool showMultiplier)
    {
        if (null != status)
        {
            if (score < max)
            {
                status.text = score + "/" + max;
            }
            else
            {
                status.text = score.ToString();
            }

            if (DataObjects.sceneInfo.multiplier > 1 && showMultiplier)
            {
                status.text = status.text + " (x" + DataObjects.sceneInfo.multiplier + "!)";
            }
        }
    }

    public void ShowScoreProgressBar(bool hideMultiplier = false)
    {
        ShowScoreProgressBar(SavedScore + CurrentLevelScore, CurrentPlayScore, MaxLevelScore, hideMultiplier);
    }

    public void ShowScoreProgressBar(long acc, long temp, long max, bool hideMultiplier = false)
    {
        if (null != pBarBackground)
        {
            pBarBackground.enabled = true;
        }

        if (null == pBar)
        {
            return;
        }

        if (0 == max)
            max = 1; // just to be sure

        float pBarFill = acc / (float)(max);

        if (mainMenuMode)
        {
            if (null != pBarTemp)
            {
                pBarTemp.enabled = false;
            }

            ShowScore(acc, max, AutoUpdate && !hideMultiplier);
            DrawProgressBar(pBar, 0, 1, pBarFill);
        }
        else
        {
            ShowScore(temp + acc, max, AutoUpdate && !hideMultiplier);

            if (null == pBarTemp)
            {
                DrawProgressBar(pBar, 0, 1, pBarFill);
                return;
            }

            if (acc < max)
            {
                pBarTemp.enabled = true;
                float pBarTempFill = temp / (float)(max);
                DrawProgressBar(pBar, 0, 1, pBarFill);
                if (AutoUpdate)
                {
                    DrawProgressBar(pBarTemp, 0, 1, pBarFill + pBarTempFill);
                }
                else
                {
                    pBarTemp.enabled = false;
                }
            }
            else
            {
                pBarTemp.enabled = false;
                DrawProgressBar(pBar, 0, 1, 1);
            }
        }
    }

    private void DrawProgressBar(Image bar, float startAnchor, float endAnchor, float fillAmount)
    {
        if (null != bar)
        {
            bar.rectTransform.anchorMin = new Vector2(startAnchor, 0);
            bar.rectTransform.anchorMax = new Vector3(endAnchor, 1);
            bar.rectTransform.offsetMax = Vector2.zero;
            bar.rectTransform.offsetMin = Vector2.zero;

            bar.fillAmount = fillAmount;
            ThreeColorsLerp(bar);
        }
    }

    private void ThreeColorsLerp(Image bar)
    {
        if (null == bar)
            return;

        if (bar.fillAmount < 0.5f)
        {
            bar.color = Color.Lerp(minColor, midColor, pBar.fillAmount * 2f);
        }
        else if (bar.fillAmount == 1f)
        {
            bar.color = completeColor;
        }
        else
        {
            bar.color = Color.Lerp(midColor, maxColor, (pBar.fillAmount - 0.5f) * 2f);
        }
    }

    private void ShowCompletedProgress()
    {
        if (null != pBarBackground)
        {
            pBarBackground.enabled = false;
        }
        if (null != pBar)
        {
            pBar.fillAmount = 0;
        }
    }

    private IEnumerator ShowCongratulation()
    {
        yield return null;
    }

    public void Reinitialize()
    {
        UpdateLevelInfo();
        FillStarsNoAnimation();
        UpdateText();
    }

}
