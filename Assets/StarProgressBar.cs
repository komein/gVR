using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarProgressBar : MonoBehaviour {

    public StarContainer star1;
    public StarContainer star2;
    public StarContainer star3;

    public int obtainedStars = 2;

    public void FillStars()
    {
        StartCoroutine(FillStarsCoroutine());
    }
	
	private IEnumerator FillStarsCoroutine()
    {
        if (obtainedStars < 0 || obtainedStars > 3)
            yield return null;

        if (obtainedStars > 0)
        {
            star1.HightlightStar();
            yield return new WaitForSeconds(0.5f);
        }


        if (obtainedStars > 1)
        {
            star2.HightlightStar();
            yield return new WaitForSeconds(0.5f);
        }

        if (obtainedStars > 2)
        {
            star3.HightlightStar();
        }


        yield return null;
    }
}
