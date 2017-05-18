using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StarProgressBar : MonoBehaviour
{
    public List <ProgressStar> stars;

    AudioSource aus;

    Coroutine c = null;

    private void Awake()
    {
        List<ProgressStar> s = GetComponentsInChildren<ProgressStar>().ToList();
        if (s.Count > 0)
        {
            stars = s;
        }
        else
        {
            List<StarContainer> sc = GetComponentsInChildren<StarContainer>().ToList();

            if (sc.Count > 0)
            {
                stars = new List<ProgressStar>();
                foreach(var v in sc)
                {
                    stars.Add(v.star);
                }
            }
        }

        aus = gameObject.AddComponent<AudioSource>();
    }

    public void UnfillStars()
    {
        if (null != stars)
        {
            foreach (var v in stars)
            {
                if (null != v)
                {
                    v.SetState(ProgressStar.StarState.empty, false);
                }
            }
        }
    }

    public void SetTextValues(List<long> starRecords)
    {
        List<StarContainer> conts = GetComponentsInChildren<StarContainer>().ToList();

        if (conts.Count > 0)
        {
            for (int i = 0; i < conts.Count; i++)
            {
                Text t = conts[i].GetComponentInChildren<Text>();
                if (null != t)
                {
                    t.text = starRecords[i].ToString();
                }
            }
        }
    }

    public void SetTextValues(LevelInfo level)
    {
        if (null != level)
        {
            SetTextValues(level.starRecords);
        }
    }

    public void FillStarsAnimated(int v)
    {
        if (gameObject.activeInHierarchy)
        {
            if (null != c)
            {
                StopCoroutine(c);
                MakeStarsNormalSize();
            }
            c = StartCoroutine(FillStarsCoroutine(v, gameObject.activeInHierarchy));
        }
    }

    private void MakeStarsNormalSize()
    {
        if (null != stars)
        {
            foreach (var v in stars)
            {
                if (null != v)
                {
                    v.MakeNormalSize();
                }
            }
        }
    }

    internal void SetStarsInactive()
    {
        if (null != stars)
        {
            foreach (var v in stars)
            {
                if (null != v)
                {
                    v.SetState(ProgressStar.StarState.inactive);
                }
            }
        }
    }

    public void FillStarsNoAnimation(int v)
    {
        UnfillStars();

        if (v < 0 || v > 3)
            return;

        for (int i = 0; i < v; i++)
        {
            if (null != stars)
            {
                if (stars.Count > i)
                {
                    if (null != stars[i])
                    {
                        stars[i].SetState(ProgressStar.StarState.filled);
                    }
                }
            }
        }
    }

	
	private IEnumerator FillStarsCoroutine(int v, bool playMusic)
    {
        if (v < 0 || v > 3)
        {
            yield return null;
        }

        for (int i = 0; i < v; i++)
        {
            if (null != stars)
            {
                if (stars.Count > i)
                {
                    if (null != stars[i])
                    {
                        if (!stars[i].IsFilled)
                        {
                            stars[i].SetState(ProgressStar.StarState.filled, true);
                            if (null != aus && playMusic)
                            {
                                aus.Stop();
                                DataObjects.SetMusic("star" + (i+1), aus);
                                if (null != aus.clip)
                                {
                                    aus.Play();
                                }
                            }
                            yield return new WaitForSeconds(0.5f);
                        }
                    }
                }
            }
        }

        yield return null;
    }
}
