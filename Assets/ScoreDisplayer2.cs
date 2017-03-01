using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplayer2 : MonoBehaviour
{
    private const string DISPLAY_TEXT_FORMAT = "{0}/{1}\n{2}hp";
    private const string DISPLAY_TEXT_FORMAT_MAXED = "{0}(MAXED)\n{1}hp";

    public int levelNumber;

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

    bool deadFlag;
    bool messageLockFlag;

    bool mainMenuMode = false;


    private void Awake()
    {
        InitializeStuff();
    }

    void Start()
    {
        storage = FindObjectOfType<DataStorage>();
        SceneInfo l = storage.GetCurrentLevel();

        level = storage.savedGame.GetLevelByName(l.title);

        if (null == level) // explicit method for main menu
        {
            mainMenuMode = true;
            level = storage.savedGame.GetLevel(levelNumber);
        }

        LoadScore();
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

    private void InitializeStuff()
    {
        deadFlag = false;
        messageLockFlag = false;
    }

    public void UpdateText()
    {
        if (null == level)
        {
            Debug.LogError("null level");
            return;
        }

        if (deadFlag) // нужен из-за особенностей вызовов методов у персистент объектов между загрузками сцен
        {
            return;
        }

        if (null != storage)
        {
            if (!storage.isAlive)
            {
                deadFlag = true;
            }
            else
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

        long accScore = level.accumulatedScore;
        long tempScore = storage.levelInfo.tempScore;
        long maxScore = level.maxScore;

        float pBarFill = accScore / (float)(maxScore);

        if (mainMenuMode)
        {
            pBarTemp.enabled = false;
            ShowScore(accScore, maxScore);
            DrawProgressBar(pBar, 0, 1, pBarFill);
        }
        else
        {
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

}
