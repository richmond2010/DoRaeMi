using UnityEngine;
using System.Linq;

[RequireComponent (typeof(AudioSource))]
public class MicrophoneAnalyser : MonoBehaviour {

	/// <summary>The current instance of this script.</summary>
	public static MicrophoneAnalyser instance;

	/// <summary>The volume of the recorded audio in RMS.</summary>
	//public float rmsValue;
	/// <summary>The volume of the recorded audio in decibels.</summary>
	public float dbValue;
	/// <summary>The frequency of the recorded audio in hertz.</summary>
	public int hzValue;
	/// <summary>Is sound detected?</summary>
	public bool isSoundDetected = false;

	/// <summary>The array of samples of recorded audio.</summary>
	[HideInInspector] public float[] samples;
	/// <summary>The spectrum of the recorded audio.</summary>
	[HideInInspector] public float[] spectrum;

	/// <summary>Reference to the AudioSource component attached to this object.</summary>
	AudioSource _audioSource;

	void Awake () { instance = this; }

	void Start () {
		// Initialise the variables.
		samples = new float[ProgramManager.SAMPLE_SIZE];
		spectrum = new float[ProgramManager.SAMPLE_SIZE];

		_audioSource = GetComponent<AudioSource> ();

		// Starts the microphone for input.
		StartMicrophone ();
	}
		
	void Update () {
		// Restart microphone if audio stopped playing.
		if (!_audioSource.isPlaying) {
			StartMicrophone ();
		}

		// Gets volume and pitch values.
		FFTAnalyser.AnalyseSound (_audioSource, out samples, out spectrum, out dbValue, out hzValue);

		// If frequency value is not zero.
		if (hzValue != 0) {
			isSoundDetected = true;

			// If program is calibrated.
			if (ProgramManager.isProgramCalibrated) {
				if (ProgramManager.pitchFreqDict.Any (pitch => hzValue == pitch.Value)) {
					Debug.Log (ProgramManager.pitchFreqDict.First (pitch => hzValue == pitch.Value).Key);
				} else {
					// Find a frequency value that is lower the current value.
					for (int i = 0; i < ProgramManager.pitchFreqDict.Count; i++) {
						if (ProgramManager.pitchFreqDict.ElementAt (i).Value < hzValue) {

							try {
								int avgPoint = (ProgramManager.pitchFreqDict.ElementAt (i).Value +
								               ProgramManager.pitchFreqDict.ElementAt (i - 1).Value) / 2;

								if ((int)hzValue >= avgPoint) {
									Debug.Log (ProgramManager.pitchFreqDict.ElementAt (i).Key);
								} else {
									Debug.Log (ProgramManager.pitchFreqDict.ElementAt (i - 1).Key);
								}
							}

							// If the program cannot find a frequency in the dictionary that is higher than the detected frequency.
							catch (System.Exception) {
								Debug.Log ("Pitch too high.");
							}

							return;
						}
					}

					Debug.Log ("Pitch too low.");
				}

			}

			// If program is not calibrated.
			else {
				PitchCalibrator.lastFrequency = hzValue;
			}
		} else {
			isSoundDetected = false;
		}
	}

	/// <summary>Starts the mic listener in low latency.</summary>
	void StartMicrophone () {
		// Starts the default microphone.
		_audioSource.clip = Microphone.Start (Microphone.devices[0], true, 999, ProgramManager.SAMPLE_RATE);

		// Keep inside loop if microphone not detected
		while (!(Microphone.GetPosition (Microphone.devices[0]) > 0)) { }

		// audioSource needs to be played for the pitch detection algorithm to work.
		_audioSource.Play ();
	}
}
