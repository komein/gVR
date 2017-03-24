using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplayer : MonoBehaviour, IUICanReinitialize
{
    public string levelTitle;

    public Image pBar;
    public Image pBarBackground;

    public Image pBarTemp;

    public Text status;

    public StarProgressBar optionalBar;

    public Color minColor;
    public Color midColor;
    public Color maxColor;

    public Color completeColor;

    public float congratulationDelay = 5f;

    GameManager storage;
    LevelInfo level;

    public bool mainMenuMode = true;
    
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
        SceneInfo l = DataObjects.sceneInfo;
        SavedGame game = DataObjects.savedGame;

        if (null != game && null != l)
        {
            level = game.GetLevelByName(l.title);

            if (null == level)
            {
                if (!mainMenuMode)
                {
                    LevelInfoContainer c = FindObjectOfType<LevelInfoContainer>();
                    if (null != c)
                    {
                        levelTitle = c.levelTitle;
                    }
                }
                level = game.GetLevelByName(levelTitle);
            }
        }

    }

    private void LoadScore()
    {
        if (null != DataObjects.gameController && null != DataObjects.dataManager)
        {
            DataObjects.dataManager.LoadWithoutAction();
            DataObjects.gameController.AddOptionalScoreAction(UpdateText);
            DataObjects.gameController.TriggerOptionalScoreAction();
        }
    }

    internal void ShowScoreProgressBarAnimated(float t, float time = 1f)
    {
        UpdateLevelInfo();

        long accScore = level.accumulatedScore;
        long tempScore = DataObjects.sceneInfo.tempScore;
        long maxScore = level.maxScore;

        if (null != pBarBackground)
        {
            pBarBackground.enabled = true;
        }

        if (null == pBar || null == pBarTemp || null == level || null == storage)
        {
            return;
        }

        if (accScore >= maxScore || t < 0)
        {
            return;
        }

        pBarTemp.enabled = true;

        if (maxScore == 0)
            maxScore = 1; // just to be sure

        float pBarFill = accScore / (float)(maxScore);

        ShowScore(tempScore + accScore, maxScore);

        float pBarTempFill = tempScore * t / time / (float)(maxScore - accScore);
        DrawProgressBar(pBar, 0, pBarFill, 1);
        DrawProgressBar(pBarTemp, pBarFill, 1, pBarTempFill);
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
                ShowScoreProgressBar();
            }
            
            if (null != optionalBar)
            {
                optionalBar.FillStarsAnimated(level.GetStarRecord(DataObjects.sceneInfo.tempScore));
            }
        }
    }

    private void ShowScore(long score, long max)
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

            if (DataObjects.sceneInfo.multiplier > 1)
            {
                status.text = status.text + " (x" + DataObjects.sceneInfo.multiplier + "!)";
            }
        }
    }

    public void ShowScoreProgressBar()
    {
        long accScore = level.accumulatedScore;
        long tempScore = DataObjects.sceneInfo.tempScore;
        long maxScore = level.maxScore;

        if (null != pBarBackground)
        {
            pBarBackground.enabled = true;
        }

        if (null == pBar)
        {
            return;
        }

        if (0 == maxScore)
            maxScore = 1; // just to be sure

        float pBarFill = accScore / (float)(maxScore);

        if (mainMenuMode)
        {
            if (null != pBarTemp)
            {
                pBarTemp.enabled = false;
            }

            ShowScore(accScore, maxScore);
            DrawProgressBar(pBar, 0, 1, pBarFill);
        }
        else
        {
            ShowScore(tempScore + accScore, maxScore);

            if (null == pBarTemp)
            {
                DrawProgressBar(pBar, 0, 1, pBarFill);
                return;
            }

            if (accScore < maxScore)
            {
                pBarTemp.enabled = true;
                float pBarTempFill = tempScore / (float)(maxScore - accScore);
                DrawProgressBar(pBar, 0, pBarFill, 1);
                DrawProgressBar(pBarTemp, pBarFill, 1, pBarTempFill);
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
