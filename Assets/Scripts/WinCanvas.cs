using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinCanvas : MonoBehaviour
{
    public StarProgressBar starBar;
    public ScoreDisplayer scoreBar;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI recordText;
    public TextMeshProUGUI messageText;

    bool animationLock = false;

    public GameObject continueButton;
    public GameObject retryButton;

    public void MakePauseScreen()
    {
        if (null != continueButton)
        {
            continueButton.gameObject.SetActive(true);
        }
        WriteMessage("Game paused");
    }

    public void MakeWinScreen()
    {
        if (null != continueButton)
        {
            continueButton.gameObject.SetActive(true);
        }
        WriteMessage("New level avaliable!");
    }

    public void WriteMessage(string s)
    {
        if (null != messageText)
        {
            messageText.text = s;
        }
    }

    public void MakeGameOverScreen()
    {
        if (null != continueButton)
        {
            continueButton.gameObject.SetActive(false);
        }
        WriteMessage("Game Over");
    }

    internal void ShowScore(PlayerController.PauseType reason)
    {
        scoreBar.UpdateLevelInfo();
        scoreBar.ShowScoreProgressBar();
        
        if (!animationLock)
        {
            animationLock = true;
            StartCoroutine(ScoreCoroutine());
        }

        switch (reason)
        {
            case PlayerController.PauseType.pause:
                MakePauseScreen();
                break;
            case PlayerController.PauseType.win:
                MakeWinScreen();
                break;
            case PlayerController.PauseType.gameOver:
                MakeGameOverScreen();
                break;
        }

    }

    IEnumerator ScoreCoroutine()
    {
        GameMusic gm = FindObjectOfType<GameMusic>();
        if (null != gm)
        {
            gm.Play("victory");
        }

        if (null != DataObjects.savedGame && null != DataObjects.sceneInfo)
        {
            LevelInfo p = GetLevelInfo();

            if (p != null)
            {
                scoreText.text = "Score: " + DataObjects.sceneInfo.tempScore;

                if (p.bestScoreRecord < DataObjects.sceneInfo.tempScore)
                {
                    recordText.text = "New record: " + DataObjects.sceneInfo.tempScore + "!";
                }
                else
                {
                    recordText.text = "Record: " + p.bestScoreRecord;
                }

                starBar.SetTextValues(p);
                scoreBar.ShowScoreProgressBarAnimated(0);

                yield return new WaitForSeconds(1f);

                if (p.accumulatedScore < p.maxScore)
                {
                    float time = 1f;
                    float timeSteps = 60f;

                    float step = time / timeSteps;

                    for (float t = 0; t < time; t += step)
                    {
                        scoreBar.ShowScoreProgressBarAnimated(t, time);
                        yield return new WaitForSeconds(step);
                    }

                    //yield return new WaitForSeconds(1f);
                }
                else
                {
                    scoreBar.ShowScoreProgressBar();
                }

                if (null != DataObjects.gameController)
                    DataObjects.gameController.UpdateBestScore();

                p = GetLevelInfo();

                starBar.FillStarsAnimated(p.starRecord);
            }
        }

        animationLock = false;

        yield return null;
    }

    private static LevelInfo GetLevelInfo()
    {
        LevelInfo p = DataObjects.savedGame.GetLevelByName(SceneManager.GetActiveScene().name);
        if (null == p)
        {
            p = DataObjects.savedGame.GetLevelByName(SceneManager.GetActiveScene().name);
        }

        return p;
    }
}
