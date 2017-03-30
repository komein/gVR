using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMusic : MonoBehaviour
{

	void Start () {
        Player p = FindObjectOfType<Player>();

        if (null != p)
        {
            transform.SetParent(p.transform);
            transform.localPosition = Vector3.zero;
        }
	}
}
