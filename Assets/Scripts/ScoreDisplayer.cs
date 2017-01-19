using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplayer : MonoBehaviour
{
    private const string DISPLAY_TEXT_FORMAT = "{0}/{1}\n{2}hp";
    private const string DISPLAY_TEXT_FORMAT_MAXED = "{0}(MAXED)\n{1}hp";

    public Text nextLvl;
    public Text message;
    public Image pBar;
    public Image pBarBackground;

    public Color minColor;
    public Color midColor;
    public Color maxColor;

    DataStorage storage;

    int lvl;

    bool deadFlag;
    bool messageLockFlag; 

    void Start()
    {
        deadFlag = false;
        messageLockFlag = false;
        storage = FindObjectOfType<DataStorage>();
        message.enabled = true;
        message.text = "";

        if (null != storage)
        {
            storage.Load();
            lvl = storage.GetCurrentLevel();
            storage.SetOptionalAction(UpdateText);
            storage.Load();
        }
    }

    public void UpdateText()
    {
        if (deadFlag)
            return;

        if (null != storage)
        {
            if (!storage.isAlive)
            {
                deadFlag = true;
                if (null != message)
                {
                    message.text = "Kitty is tired, returning to menu!";
                }
                return;
            }

            int curLvl = storage.GetCurrentLevel();
            if (curLvl > lvl)
            {
                StopAllCoroutines();
                StartCoroutine(ShowCongratulation());
            }

            lvl = curLvl;

            long score = storage.GetScore();
            if (null != message && !messageLockFlag)
            {
                if (storage.multiplier > 1)
                    message.text = "Score: " + score + " (x" + storage.multiplier + " bonus!)";
                else
                    message.text = "Score: " + score;
            }

            if (storage.IsMaxLvl())
            {
                if (null != pBarBackground)
                {
                    pBarBackground.enabled = false;
                }
                if (null != nextLvl)
                {
                    nextLvl.text = "You have unlocked all the levels!";
                }
                if (null != pBar)
                {
                    pBar.fillAmount = 0;
                }
                return;
            }
            else
            {
                if (null != pBarBackground)
                {
                    pBarBackground.enabled = true;
                }
                long toNextLvl = storage.GetNextLvlUnlock();
                long toCurrentLvl = storage.GetCurrentLevelUnlock();

                if (null != nextLvl)
                {
                    nextLvl.text = "To next level: " + toNextLvl.ToString();
                }

                if (null != pBar)
                {
                    pBar.fillAmount = ((score - toCurrentLvl) / (float)(toNextLvl - toCurrentLvl));
                    if (pBar.fillAmount < 0.5f)
                    {
                        pBar.color = Color.Lerp(minColor, midColor, pBar.fillAmount * 2f);
                    }
                    else
                    {
                        pBar.color = Color.Lerp(midColor, maxColor, (pBar.fillAmount - 0.5f) * 2f);
                    }
                }
            }
        }
    }

    private IEnumerator ShowCongratulation()
    {
        if (null != message)
        {
            messageLockFlag = true;
            message.text = "You gained access to a new level!";
            yield return new WaitForSeconds(5f);
            message.text = "";
            messageLockFlag = false;
        }
        yield return null;
    }

}
