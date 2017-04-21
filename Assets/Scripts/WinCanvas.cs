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

        if (LastLevel())
        {
            WriteMessage("Thanks for playing!");
        }
        else
        {
            WriteMessage("New level avaliable!");
        }
    }

    private bool LastLevel()
    {
        if (null != DataObjects.SavedGame)
        {
            if (null != DataObjects.SavedGame.levels)
            {
                LevelInfo lastOne = DataObjects.SavedGame.levels.FindLast(p => p != null);
                if (null != lastOne)
                {
                    return SceneManager.GetActiveScene().name == lastOne.title;
                }
            }
        }
        return false;
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

    internal void ShowScore(PauseType reason, LevelInfo p)
    {
        if (null != c)
        {
            StopCoroutine(c);
        }
        starBar.UnfillStars();
        
        if (null != p)
        {
            scoreBar.ShowScoreProgressBar(p.accumulatedScore + DataObjects.SceneInfo.tempScoreSaved, DataObjects.SceneInfo.tempScore, p.maxScore, reason == PauseType.gameOver);

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

            c = StartCoroutine(ScoreCoroutine(reason, p.accumulatedScore + DataObjects.SceneInfo.tempScoreSaved, DataObjects.SceneInfo.tempScore, p.maxScore, p.bestScoreRecord, p.starRecords, p.GetStarRecord(DataObjects.SceneInfo.TempScore)));
        }

    }

    IEnumerator ScoreCoroutine(PauseType reason, long acc, long temp, long max, long bestRecord, List<long> starRecords, int currentStarRecord)
    {

        if (null != DataObjects.SavedGame && null != DataObjects.SceneInfo)
        {
            //if (p != null)
            {
                scoreText.text = "Score: " + DataObjects.SceneInfo.TempScore;

                if (bestRecord < DataObjects.SceneInfo.TempScore)
                {
                    recordText.text = "New record: " + DataObjects.SceneInfo.TempScore + "!";

                    if (null != DataObjects.GameController)
                        DataObjects.GameController.UpdateBestScore();
                }
                else
                {
                    recordText.text = "Record: " + bestRecord;
                }

                starBar.SetTextValues(starRecords);
                scoreBar.ShowScoreProgressBarAnimated(0, acc, max, temp, 1f, reason == PauseType.gameOver);

                yield return new WaitForSeconds(1f);

                if (acc < max)
                {
                    float time = 1f;
                    float timeSteps = 60f;

                    float step = time / timeSteps;

                    for (float t = 0; t < time; t += step)
                    {
                        scoreBar.ShowScoreProgressBarAnimated(t, acc, max, temp, time, reason == PauseType.gameOver);
                        yield return new WaitForSeconds(step);
                    }

                    //yield return new WaitForSeconds(1f);
                }
                else
                {
                    scoreBar.ShowScoreProgressBar();
                }

                starBar.FillStarsAnimated(currentStarRecord);
            }
        }

        yield return null;
    }
}
