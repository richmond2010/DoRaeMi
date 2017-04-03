using UnityEngine;
using UnityEngine.UI;
//using System.Collections;
using System.Linq;

/// <summary>The class to spawn objects using the beat of the music.</summary>
public class SpawnOnBeat : MonoBehaviour, BeatDetector.AudioCallbacks {
	/// <summary>The prefab to spawn when a beat is detected.</summary>
	public GameObject gameObjectSpawn;

	public Transform parentObject;

	/// <summary>Frequency of the music beat.</summary>
	int beatFrequency;

	void Start () {
		BeatDetector processor = FindObjectOfType<BeatDetector> ();
		processor.AddAudioCallback (this);
	}

	public void OnBeatDetected (int beatFrequency) {
		GameObject go;

		if (!ProgramManager.instance.useDefaultDict) {
			if (ProgramManager.pitchFreqDict.Any (pitch => beatFrequency == pitch.Value)) {
				Debug.Log (ProgramManager.pitchFreqDict.First (pitch => beatFrequency == pitch.Value).Key);
			} else {
				// Find a frequency value that is lower the current value.
				for (int i = 0; i < ProgramManager.pitchFreqDict.Count; i++) {
					if (ProgramManager.pitchFreqDict.ElementAt (i).Value < beatFrequency) {

						try {
							int avgPoint = (ProgramManager.pitchFreqDict.ElementAt (i).Value +
								ProgramManager.pitchFreqDict.ElementAt (i - 1).Value) / 2;

							if (beatFrequency >= avgPoint) {
								go = Instantiate (gameObjectSpawn, transform.position, Quaternion.identity) as GameObject;
								go.GetComponent<Text> ().text = ProgramManager.instance.isShowingFrequency ? beatFrequency.ToString () : ProgramManager.pitchFreqDict.ElementAt (i).Key;
							} else {
								go = Instantiate (gameObjectSpawn, transform.position, Quaternion.identity) as GameObject;
								go.GetComponent<Text> ().text = ProgramManager.instance.isShowingFrequency ? beatFrequency.ToString () : ProgramManager.pitchFreqDict.ElementAt (i - 1).Key;
							}
						}

						// If the program cannot find a frequency in the dictionary that is higher than the detected frequency.
						catch (System.Exception) {
							Debug.Log ("Pitch too high.");
						}

						return;
					}
				}

				//Debug.Log ("Pitch too low.");
			}
		} else {
			if (ProgramManager.defaultPitchDict.Any (pitch => beatFrequency == pitch.Value)) {
				Debug.Log (ProgramManager.defaultPitchDict.First (pitch => beatFrequency == pitch.Value).Key);
			} else {
				// Find a frequency value that is lower the current value.
				for (int i = 1; i < ProgramManager.defaultPitchDict.Count; i++) {
					if (ProgramManager.defaultPitchDict.ElementAt (i).Value < beatFrequency) {

						int avgPoint = (ProgramManager.defaultPitchDict.ElementAt (i).Value +
							ProgramManager.defaultPitchDict.ElementAt (i - 1).Value) / 2;

						if (beatFrequency >= avgPoint) {
							go = Instantiate (gameObjectSpawn, parentObject, false) as GameObject;
							go.GetComponentInChildren<Text> ().text = ProgramManager.instance.isShowingFrequency ? beatFrequency.ToString () : ProgramManager.defaultPitchDict.ElementAt (i).Key;
						} else {
							go = Instantiate (gameObjectSpawn, transform.position, Quaternion.identity, parentObject) as GameObject;
							go.GetComponent<Text> ().text = ProgramManager.instance.isShowingFrequency ? beatFrequency.ToString () : ProgramManager.defaultPitchDict.ElementAt (i - 1).Key;
						}
					}
				}

				//Debug.Log ("Pitch too kek.");
			}
		}
	}
}
