﻿using UnityEngine;
using System.Linq;

[RequireComponent (typeof(AudioSource))]
public class FFTAnalyser : MonoBehaviour {
	/// <summary>Frequency of the microphone.</summary>
	private const int FREQUENCY = 48000;
	/// <summary>Sample count.</summary>
	private const int SAMPLE_COUNT = 1024;
	/// <summary>Minimum RMS volume to extract the pitch.</summary>
	private const float THRESHOLD = 0.02f;

	/// <summary>The current instance of this script.</summary>
	public static FFTAnalyser instance;

	/// <summary>Clamp the decibels.</summary>
	public int clamp = 160;
	/// <summary>Is sound detected?</summary>
	public bool isSoundDetected = false;

	/// <summary>The volume of the recorded audio in RMS.</summary>
	public float rmsValue;
	/// <summary>The volume of the recorded audio in decibels.</summary>
	public float dbValue;
	/// <summary>The frequency of the recorded audio in hertz.</summary>
	public int hzValue;

	/// <summary>The array of samples of recorded audio.</summary>
	private float[] samples;
	/// <summary>The spectrum of the recorded audio.</summary>
	private float[] spectrum;

	/// <summary>Reference to the AudioSource component attached to this object.</summary>
	AudioSource _audioSource;

	public void Awake () { instance = this; }

	public void Start () {
		// init variables
		samples = new float[SAMPLE_COUNT];
		spectrum = new float[SAMPLE_COUNT];

		_audioSource = GetComponent<AudioSource> ();

		StartMicrophone ();
	}
		
	void Update () {
		// If the audio has stopped playing, this will restart the mic play the clip.
		if (!_audioSource.isPlaying) {
			StartMicrophone ();
		}

		// Gets volume and pitch values
		AnalyseSound ();

		if (hzValue != 0) {
			isSoundDetected = true;

			// if all notes have already been calibrated
			if (GameManager.instance.isCalibrated) {
				if (PitchCalibrator.pitchFreqDict.Any (pitch => hzValue == pitch.Value)) {
					Debug.Log (PitchCalibrator.pitchFreqDict.First (pitch => hzValue == pitch.Value).Key);
				} else {
					
					for (int i = 0; i < PitchCalibrator.pitchFreqDict.Count; i++) {
						if (PitchCalibrator.pitchFreqDict.ElementAt (i).Value < hzValue) {

							try {
								int avgPoint = (PitchCalibrator.pitchFreqDict.ElementAt (i).Value +
								              PitchCalibrator.pitchFreqDict.ElementAt (i - 1).Value) / 2;

								if ((int)hzValue >= avgPoint) {
									Debug.Log (PitchCalibrator.pitchFreqDict.ElementAt (i).Key);
								} else {
									Debug.Log (PitchCalibrator.pitchFreqDict.ElementAt (i - 1).Key);
								}
							}
							// if there is no previous element
							catch (System.Exception) {
								Debug.Log ("Pitch too low.");
							}

							return;
						}
					}

					Debug.Log ("Pitch too high.");
				}

			}

			// if uncalibrated
			else {
				PitchCalibrator.lastFrequency = hzValue;
				Debug.Log ("Volume: " + dbValue.ToString ("F1") + " dB\n" + "Pitch: " + hzValue + " Hz");
			}
		} else {
			isSoundDetected = false;
		}
	}

	/// <summary>Starts the mic listener in low latency.</summary>
	void StartMicrophone () {
		// starts the default microphone.
		_audioSource.clip = Microphone.Start ("OCTA-CAPTURE", true, 999, FREQUENCY);

		// breaks out of the function if there are no microphones
		while (!(Microphone.GetPosition ("OCTA-CAPTURE") > 0)) { }

		_audioSource.Play ();
	}

	/// Obtain volume and frequency of the recorded audio
	void AnalyseSound () {
		// Obtain samples from the recorded audio.
		_audioSource.GetOutputData (samples, 0);

		// Calculate the RMS of the volume
		float sum = 0;
		for (int i = 0; i < SAMPLE_COUNT; i++) {
			sum += Mathf.Pow (samples [i], 2);
		}
		rmsValue = Mathf.Sqrt (sum / SAMPLE_COUNT);
		dbValue = 20 * Mathf.Log10 (rmsValue / 0.1f);

		// Clamp the value in decibels to the minimum if it is less than it
		if (dbValue < -clamp) {
			dbValue = -clamp;
		}

		// Obtains the audio spectrum
		_audioSource.GetSpectrumData (spectrum, 0, FFTWindow.Blackman);
		float highestSample = 0;
		float sampleIndex = 0;

		// Find the highest sample
		for (int i = 0; i < SAMPLE_COUNT; i++) {
			if (spectrum [i] > highestSample && spectrum [i] > THRESHOLD) {
				highestSample = spectrum [i];
				sampleIndex = (float)i;
			}
		}

		// Interpolate index using the immediate left and right neighbours
		if (sampleIndex > 0 && sampleIndex < SAMPLE_COUNT - 1) {
			float dL = spectrum [(int)sampleIndex - 1] / spectrum [(int)sampleIndex];
			float dR = spectrum [(int)sampleIndex + 1] / spectrum [(int)sampleIndex];
			sampleIndex += 0.5f * (dR * dR - dL * dL);
		}

		// Convert index to frequency
		hzValue = (int)(sampleIndex * (FREQUENCY / 2) / SAMPLE_COUNT);
	}
}
