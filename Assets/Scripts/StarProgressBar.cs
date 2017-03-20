using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StarProgressBar : MonoBehaviour
{

    public List <ProgressStar> stars;

    private void Awake()
    {
        List<ProgressStar> s = GetComponentsInChildren<ProgressStar>().ToList();
        if (s.Count > 0)
            stars = s;
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

    public void FillStarsAnimated(int v)
    {
        if (animationLock)
            return;

        animationLock = true;
        StartCoroutine(FillStarsCoroutine(v));
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
