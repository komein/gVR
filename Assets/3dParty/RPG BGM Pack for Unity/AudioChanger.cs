using UnityEngine;
using System.Collections;

public class AudioChanger : MonoBehaviour {
	const int numoftracks = 30;
	public AudioClip[] main;
	public AudioClip[] intro;
	public float[] delaytime = new float[numoftracks];
	struct tracks{
		public float delaytime;
		public AudioClip intro;
		public AudioClip main;
	}
	private int t=0;
	private AudioSource audioSource;
	private bool nowPlaying = false;
	tracks[] track;

	//track name//
	private string[] trackname= new string[numoftracks] {
		"battle_1","battle_2","battle_3","battle_4","battle_5","battle_6","battle_7","battle_8",
		"field_1","field_1(no insects)","field_2","field_3",
		"field_4","field_5","field_6","field_7","menu_1","menu_2","menu_3","minigame_1",
		"minigame_2","minigame_3","shop_1","shop_2","shop_3","shop_4","event_1","event_2","event_3","event_4"
		};

	// Use this for initialization
	void Start () {
		track = new tracks[numoftracks];
		for (int s=0; s < numoftracks; s++) {
			track[s].delaytime=0.0f;
			track[s].intro=null;
			track[s].main=null;
		}
		for (int i=0; i < numoftracks; i++) {
			track[i].delaytime=delaytime[i];
			track[i].intro=intro[i];
			track[i].main=main[i];
		}
	}

	void OnGUI ()
	{
		GUI.Box (new Rect (350, 200, 300, 70), "Change Music");
		if(GUI.Button (new Rect (370, 225, 70,30), "Back")){
			t--;
			t=LoopNum(t);
			PlayWithDelay(t,nowPlaying);
		}
		if(GUI.Button (new Rect (460, 225, 80,30), "Play/Stop")){
			nowPlaying=!nowPlaying;
			PlayWithDelay(t,nowPlaying);
		}
		if(GUI.Button (new Rect (560, 225, 70,30), "Next")){
			t++;
			t=LoopNum (t);
			PlayWithDelay(t,nowPlaying);
		}

		int	p = GUI.SelectionGrid(new Rect(25, 25, 240, 400), t, trackname, 2);
		if (p != t) {
			t = p;
			nowPlaying=true;
			PlayWithDelay(t,nowPlaying);
		}


	}



	void PlayWithDelay(int t,bool nowPlaying) {
		AudioSource[] audioSources = gameObject.GetComponents<AudioSource>();
		if (nowPlaying) {
			if (track [t].delaytime == 0) {
				audioSources[0].Stop();
				audioSources[1].clip = main[t];
				audioSources[1].Play();
			} else {
				audioSources[0].clip = intro[t];
				audioSources[0].Play();
				audioSources[1].clip = main[t];
				audioSources[1].PlayDelayed(track[t].delaytime);
			}
		} else {
			audioSources[0].Stop();
			audioSources[1].Stop();
		}
	}

	int LoopNum(int a) {
		if(a < 0)
			a = trackname.Length-1;
		if (a > (trackname.Length-1) )
			a = 0;
		return a;
	}


}
