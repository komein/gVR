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

    public GameObject continueButton;
    public GameObject retryButton;

    Coroutine c = null;

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

    internal void ShowScore(PauseType reason)
    {
        if (null != c)
        {
            StopCoroutine(c);
        }
        starBar.UnfillStars();
        
        scoreBar.ShowScoreProgressBar(reason == PauseType.gameOver);

        switch (reason)
        {
            case PauseType.pause:
                MakePauseScreen();
                break;
            case PauseType.win:
                MakeWinScreen();
                break;
            case PauseType.gameOver:
                MakeGameOverScreen();
                break;
        }

        c = StartCoroutine(ScoreCoroutine(reason));
    }

    IEnumerator ScoreCoroutine(PauseType reason)
    {

        if (null != DataObjects.savedGame && null != DataObjects.sceneInfo)
        {
            LevelInfo p = GetLevelInfo();

            if (p != null)
            {
                scoreText.text = "Score: " + DataObjects.sceneInfo.TempScore;

                if (p.bestScoreRecord < DataObjects.sceneInfo.TempScore)
                {
                    recordText.text = "New record: " + DataObjects.sceneInfo.TempScore + "!";

                    if (null != DataObjects.gameController)
                        DataObjects.gameController.UpdateBestScore();
                }
                else
                {
                    recordText.text = "Record: " + p.bestScoreRecord;
                }

                starBar.SetTextValues(p);
                scoreBar.ShowScoreProgressBarAnimated(0, reason == PauseType.gameOver);

                yield return new WaitForSeconds(1f);

                if (p.accumulatedScore < p.maxScore)
                {
                    float time = 1f;
                    float timeSteps = 60f;

                    float step = time / timeSteps;

                    for (float t = 0; t < time; t += step)
                    {
                        scoreBar.ShowScoreProgressBarAnimated(t, reason == PauseType.gameOver, time);
                        yield return new WaitForSeconds(step);
                    }

                    //yield return new WaitForSeconds(1f);
                }
                else
                {
                    scoreBar.ShowScoreProgressBar();
                }

                p = GetLevelInfo();

                starBar.FillStarsAnimated(p.GetStarRecord(DataObjects.sceneInfo.tempScore + DataObjects.sceneInfo.tempScoreSaved));
            }
        }

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
