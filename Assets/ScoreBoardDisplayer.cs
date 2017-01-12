using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBoardDisplayer : MonoBehaviour {

    ScoreDisplayer score;

	void Start () {
        score = FindObjectOfType<ScoreDisplayer>();
        if (null != score)
        {
        }	
	}
}
