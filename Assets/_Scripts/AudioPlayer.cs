using UnityEngine;

/// <summary>Script that delays the playing of the audio.</summary>
[RequireComponent (typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour {
	void Start () {
		// Obtain the audio clip from the Program Manager.
		GetComponent<AudioSource> ().clip = ProgramManager.instance.GetComponent<AudioSource> ().clip;
		// Delay the audio playing by 2.5 seconds.
		GetComponent<AudioSource> ().PlayDelayed (2.5f);
	}
}
