using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinCanvas : MonoBehaviour
{
    public StarProgressBar starBar;
    public ScoreDisplayer scoreBar;

    public Text scoreText;
    public Text recordText;

    bool animationLock = false;
	
    internal void ShowScore()
    {
        if (!animationLock)
        {
            animationLock = true;
            StartCoroutine(ScoreCoroutine());
        }
    }

    IEnumerator ScoreCoroutine()
    {
        if (null != DataObjects.savedGame && null != DataObjects.sceneInfo)
        {
            LevelInfo p = DataObjects.savedGame.GetLevelByName(DataObjects.sceneInfo.title);
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

                    yield return new WaitForSeconds(1f);
                }
                else
                {
                    scoreBar.ShowScoreProgressBar();
                }

                if (null != DataObjects.gameController)
                    DataObjects.gameController.UpdateBestScore();

                p = DataObjects.savedGame.GetLevelByName(DataObjects.sceneInfo.title);

                starBar.FillStarsAnimated(p.starRecord);
            }
        }

        animationLock = false;

        yield return null;
    }
}
