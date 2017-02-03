using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Batcher : MonoBehaviour {

	void Start () {
        StaticBatchingUtility.Combine(this.gameObject);
	}
	
}
