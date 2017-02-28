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

    public Text status;

    public Color minColor;
    public Color midColor;
    public Color maxColor;

    public float congratulationDelay = 5f;

    DataStorage storage;

    int lvl;

    bool deadFlag;
    bool messageLockFlag;

    void Start()
    {
        InitializeStuff();
        LoadScore();
    }

    private void LoadScore() // TODO переделать (см комменты)
    {
        if (null != storage)
        {
            storage.Load(); // для гарантии, что если сейв есть, из него загрузились данные
            lvl = storage.GetCurrentLevel(); // берем загруженный уровень
            storage.SetOptionalAction(UpdateText);
        }
    }

    private void InitializeStuff()
    {
        deadFlag = false;
        messageLockFlag = false;
        storage = FindObjectOfType<DataStorage>();
        //message.enabled = true;
        //message.text = "";
    }

    public void UpdateText()
    {
        if (null == gameObject)
            return;

        if (deadFlag) // нужен из-за особенностей вызовов методов у персистент объектов между загрузками сцен
        {
            return;
        }

        if (null != storage)
        {
            if (!storage.isAlive)
            {
                deadFlag = true;
                //ShowDeadMessage();
            }
            else
            {
                ShowScoreProgressBar();
            }
        }
    }
    /*
    private void ShowDeadMessage()
    {
        if (null != message)
        {
            message.text = "Kitty is tired, returning to menu!";
        }
        if (null != nextLvl)
        {
            nextLvl.text = "";
        }
    }
    */
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
        long score = storage.GetScore(levelNumber);
        if (null != pBarBackground)
        {
            pBarBackground.enabled = true;
        }
        long toNextLvl = storage.GetMaxScore(levelNumber);
        long toCurrentLvl = 0;

        ShowScore(score, toNextLvl);
        DrawProgressBar(score, toNextLvl, toCurrentLvl);
    }

    private void DrawProgressBar(long score, long toNextLvl, long toCurrentLvl)
    {
        if (null != pBar)
        {
            pBar.fillAmount = ((score - toCurrentLvl) / (float)(toNextLvl - toCurrentLvl));
            ThreeColorsLerp();
        }
    }

    private void ThreeColorsLerp()
    {
        if (pBar.fillAmount < 0.5f)
        {
            pBar.color = Color.Lerp(minColor, midColor, pBar.fillAmount * 2f);
        }
        else if (pBar.fillAmount == 1f)
        {
            pBar.color = Color.blue;
        }
        else
        {
            pBar.color = Color.Lerp(midColor, maxColor, (pBar.fillAmount - 0.5f) * 2f);
        }
    }

    private void ShowCompletedProgress()
    {
        if (null != pBarBackground)
        {
            pBarBackground.enabled = false;
        }
        /* if (null != nextLvl)
         {
             nextLvl.text = "You have unlocked all the levels!";
         }*/
        if (null != pBar)
        {
            pBar.fillAmount = 0;
        }
    }

    private IEnumerator ShowCongratulation()
    {
        /* if (null != message)
         {
             messageLockFlag = true;
             message.text = "You gained access to a new level!";
             yield return new WaitForSeconds(congratulationDelay);
             message.text = "";
             messageLockFlag = false;
         }*/
        yield return null;
    }

}
