using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StarProgressBar : MonoBehaviour
{

    public List <ProgressStar> stars;

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
    }

    public int obtainedStars
    {
        get
        {
            if (null == stars)
                return 0;

            return stars.FindAll(p => p.filled == true).Count;
        }
    }

    private bool animationLock = false;

    public void UnfillStars()
    {
        foreach(var v in stars)
        {
            if (null != v)
                v.MakeEmpty();
        }
    }

    public void SetTextValues(LevelInfo level)
    {
        List<StarContainer> conts = GetComponentsInChildren<StarContainer>().ToList();

        if (conts.Count > 0)
        {
            for (int i = 0; i < conts.Count; i++ )
            {
                Text t = conts[i].GetComponentInChildren<Text>();
                if (null != t)
                {
                    t.text = level.starRecords[i].ToString();
                }
            }
        }
    }

    public void FillStarsAnimated(int v)
    {
        if (animationLock)
            return;

        if (gameObject.activeInHierarchy)
        {
            animationLock = true;
            StartCoroutine(FillStarsCoroutine(v));
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
                        stars[i].FillNoAnimation();
                    }
                }
            }
        }
    }

	
	private IEnumerator FillStarsCoroutine(int v)
    {
        if (v < 0 || v > 3)
        {
            animationLock = false;
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
                        if (!stars[i].filled)
                        {
                            stars[i].FillAnimated();
                            yield return new WaitForSeconds(0.5f);
                        }
                    }
                }
            }
        }

        animationLock = false;

        yield return null;
    }
}
