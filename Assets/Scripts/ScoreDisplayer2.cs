using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplayer2 : MonoBehaviour, IUICanReinitialize
{
    public string levelTitle;

    public Image pBar;
    public Image pBarBackground;

    public Image pBarTemp;

    public Text status;

    public Color minColor;
    public Color midColor;
    public Color maxColor;

    public Color completeColor;

    public float congratulationDelay = 5f;

    DataStorage storage;
    LevelInfo level;

    bool mainMenuMode = false;
    
    void Start()
    {
        storage = FindObjectOfType<DataStorage>();

        UpdateLevelInfo();
        LoadScore();
    }

    public void UpdateLevelInfo()
    {
        SceneInfo l = storage.GetCurrentLevel();
        level = storage.savedGame.GetLevelByName(l.title);

        if (null == level)
        {
            mainMenuMode = true;
            level = storage.savedGame.GetLevelByName(levelTitle);
        }
    }

    private void LoadScore()
    {
        if (null != storage)
        {
            storage.LoadWithoutAction();
            storage.SetOptionalScoreAction(UpdateText);
            storage.OptionalScoreAction();
        }
    }

    public void UpdateText()
    {
        if (null == level)
        {
            Debug.LogError("null level");
            return;
        }

        if (null != storage)
        {
            if (storage.isAlive)
            {
                ShowScoreProgressBar();
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
        }
    }

    private void ShowScoreProgressBar()
    {
        if (null != pBarBackground)
        {
            pBarBackground.enabled = true;
        }

        if (null == pBar)
            return;

        long accScore = level.accumulatedScore;
        long tempScore = storage.levelInfo.tempScore;
        long maxScore = level.maxScore;

        float pBarFill = accScore / (float)(maxScore);

        if (mainMenuMode)
        {
            if (null != pBarTemp)
                pBarTemp.enabled = false;

            ShowScore(accScore, maxScore);
            DrawProgressBar(pBar, 0, 1, pBarFill);
        }
        else
        {
            if (null == pBarTemp)
                return;

            pBarTemp.enabled = true;

            ShowScore(tempScore + accScore, maxScore);

            if (accScore < maxScore)
            {
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
        Debug.Log("reinit " + gameObject.name);
        UpdateLevelInfo();
        UpdateText();
    }
}
