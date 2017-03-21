using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinCanvas : MonoBehaviour
{
    public StarProgressBar starBar;
    public ScoreDisplayer2 scoreBar;

    public Text scoreText;
    public Text recordText;

    DataStorage storage;

    bool animationLock = false;
	
    internal void ShowScore()
    {
        if (null == storage)
            storage = FindObjectOfType<DataStorage>();

        if (!animationLock)
        {
            animationLock = true;
            StartCoroutine(ScoreCoroutine());
        }
    }

    IEnumerator ScoreCoroutine()
    {
        Debug.Log(storage);

        if (null != storage)
        {
            SceneInfo info = storage.sceneInfo;

            if (null != info)
            {
                Debug.Log(info.title);
                LevelInfo p = storage.savedGame.GetLevelByName(info.title);
                if (p != null)
                {
                    scoreText.text = "Score: " + info.tempScore;

                    if (p.bestScoreRecord < info.tempScore)
                    {
                        recordText.text = "New record: " + info.tempScore + "!";
                        recordText.color = Color.green;
                    }
                    else
                    {
                        recordText.text = "Record: " + p.bestScoreRecord;
                    }

                    scoreBar.UpdateText();

                    starBar.SetTextValues(p);

                    yield return new WaitForSeconds(1f);

                    storage.UpdateBestScore();

                    p = storage.savedGame.GetLevelByName(info.title);

                    starBar.FillStarsAnimated(p.starRecord);
                }
            }
        }

        animationLock = false;

        yield return null;
    }
}
