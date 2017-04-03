using UnityEngine;
using System.Collections;

/// <summary>The manager for audio visualisation.</summary>
public class AudioVisualiser : MonoBehaviour {
	/// <summary>The current instance of this script.</summary>
	public static AudioVisualiser instance;

	public GameObject visualisationPrefab;
	GameObject[] prefabArray = new GameObject[128];

	/// <summary>The reference to the audio source for spectrum extraction.</summary>
	public AudioSource _audioSource;

	/// <summary>The array of samples.</summary>
	float[] samples = new float[1024];

	void Start () {
		instance = this;

		//_audioSource = GetComponent<AudioSource> ();

		for (int i = 0; i < 128; i++) {
			GameObject _tempCube = Instantiate<GameObject> (visualisationPrefab);
			_tempCube.transform.position = new Vector3 (transform.position.x + (0.144f * i) - 9.25f, 0, 0);
			_tempCube.transform.SetParent (transform);
			_tempCube.name = "Visualisation" + i;
			prefabArray [i] = _tempCube;
		}

	}

	void Update () {
		if (_audioSource)
			_audioSource.GetSpectrumData (samples, 0, FFTWindow.BlackmanHarris);

		for (int i = 0; i < 128; i++) {
			//if (samples [i] > 0.02f) {
				prefabArray [i].transform.localScale = new Vector3 (0.5f, samples [i] * 50, 1);
			//} else {
			//	prefabArray [i].transform.localScale = new Vector3 (0.5f, 0, 1);
			//}
		}
	}
}
