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

    public long accScore = -1;
    long tempScore = -1;
    long maxScore = -1;

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
                optionalBar.FillStarsNoAnimation(level.starRecord);
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

        GetScores();
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
                UpdateLevelInfo();
                UpdateText();
            }
        }
    }

    internal void ShowScoreProgressBarAnimated(float t, float time = 1f)
    {
        ShowScoreProgressBarAnimated(t, accScore, maxScore, tempScore, time);
    }

    internal void ShowScoreProgressBarAnimated(float t, long acc, long max, long tmp, float time = 1f)
    {

        UpdateLevelInfo();


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

        ShowScore(tmp + acc, max, true);

        float pBarTempFill = tmp * t / time / (float)(max);

        DrawProgressBar(pBar, 0, 1, pBarFill);
        DrawProgressBar(pBarTemp, 0, 1, pBarFill + pBarTempFill);
    }

    private void GetScores()
    {
        accScore = level.accumulatedScore;
        tempScore = DataObjects.sceneInfo.tempScore;
        maxScore = level.maxScore;
    }

    public void UpdateTextWithCheck()
    {
        UpdateLevelInfo();

        if (accScore < maxScore)
        {
            if (accScore + tempScore >= maxScore)
            {
                // make it win
                PlayerController cat = FindObjectOfType<PlayerController>();
                if (null != cat)
                {
                    cat.FinishLevel(PlayerController.PauseType.win);
                    return;
                }
            }
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
                ShowScoreProgressBar(accScore, tempScore, maxScore);
            }
            
            if (null != optionalBar)
            {
                optionalBar.FillStarsAnimated(level.GetStarRecord(tempScore));
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

    public void ShowScoreProgressBar()
    {
        ShowScoreProgressBar(accScore, tempScore, maxScore);
    }

    public void ShowScoreProgressBar(long acc, long temp, long max)
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

            ShowScore(acc, max, AutoUpdate);
            DrawProgressBar(pBar, 0, 1, pBarFill);
        }
        else
        {
            ShowScore(temp + acc, max, AutoUpdate);

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
        if (null != optionalBar)
        {
            optionalBar.UnfillStars();
        }
        UpdateText();
    }
}
