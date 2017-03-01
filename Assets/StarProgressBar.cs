using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarProgressBar : MonoBehaviour {

    public StarContainer star1;
    public StarContainer star2;
    public StarContainer star3;

    public ProgressStar st1;
    public ProgressStar st2;
    public ProgressStar st3;

    public int obtainedStars = 2;

    public void FillStarsAnimated(int v)
    {
        obtainedStars = v;
        StartCoroutine(FillStarsCoroutine());
    }

    public void FillStarsNoAnimation(int v)
    {
        obtainedStars = v;

        if (obtainedStars < 0 || obtainedStars > 3)
            return;

        if (obtainedStars > 0)
        {
            if (null != star1)
                star1.HightlightStarNoAnimation();
            else
            {
                if (null != st1)
                {
                    st1.FillNoAnimation();
                }
            }
        }


        if (obtainedStars > 1)
        {
            if (null != star2)
                star2.HightlightStarNoAnimation();
            else
            {
                if (null != st1)
                {
                    st2.FillNoAnimation();
                }
            }
        }

        if (obtainedStars > 2)
        {
            if (null != star3)
                star3.HightlightStarNoAnimation();
            else
            {
                if (null != st1)
                {
                    st3.FillNoAnimation();
                }
            }
        }
    }

	
	private IEnumerator FillStarsCoroutine()
    {
        if (obtainedStars < 0 || obtainedStars > 3)
            yield return null;

        if (obtainedStars > 0)
        {
            if (null != star1)
                star1.HightlightStarAnimated();
            else
            {
                if (null != st1)
                {
                    st1.FillAnimated();
                }
            }
            yield return new WaitForSeconds(0.5f);
        }


        if (obtainedStars > 1)
        {
            if (null != star2)
                star2.HightlightStarAnimated();
            else
            {
                if (null != st1)
                {
                    st2.FillAnimated();
                }
            }
            yield return new WaitForSeconds(0.5f);
        }

        if (obtainedStars > 2)
        {
            if (null != star3)
                star3.HightlightStarAnimated();
            else
            {
                if (null != st1)
                {
                    st3.FillAnimated();
                }
            }
        }


        yield return null;
    }
}
