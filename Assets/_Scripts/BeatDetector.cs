using UnityEngine;
using System.Collections.Generic;

/// <summary>Beat detector.</summary>
[RequireComponent(typeof(AudioSource))]
public class BeatDetector : MonoBehaviour {
	/// <summary>The number of bands.</summary>
	const int NO_OF_BANDS = 12;
	/// <summary>The buffer size for use in FFT.</summary>
	public int bufferSize = 1024;
	/// <summary>The threshold to spawn.</summary>
	public float threshold = 0.1f;

	/// <summary>The sampling rate.</summary>
	int samplingRate = 44100;

	/// <summary>The referenced AudioSource component.</summary>
	AudioSource _audioSource;

	/// <summary>The list of available callbacks.</summary>
	List<AudioCallbacks> callbacks = new List<AudioCallbacks> ();

	int[] blipDelay;

	/// <summary>Counter to suppress double-beats.</summary>
	int sinceLast = 0;

	float framePeriod;

	/// <summary>The bandwidth of the current band.</summary>
	public float bandwidth { get { return (2f / (float)bufferSize) * (samplingRate / 2f); } }

	/* storage space */
	int colmax = 120;
	float[] samples;
	float[] spectrum;
	float[] averages;
	float[] acVals;
	float[] onsets;
	float[] scorefun;
	float[] dobeat;
	/// <summary>The current time index.</summary>
	int currTimeIndex = 0;

	/// <summary>The spectrum of the previous step.</summary>
	float[] spec;

	/// <summary>The largest lag (tempo) to track.</summary>
	int maxlag = 100;
	/// <summary>Smoothing constant.</summary>
	float decay = 0.997f;
	/// <summary>The autocorrelator.</summary>
	Autocorrelator auco;

	/// <summary>Trade-off constant between tempo deviation penalty and onset strength.</summary>
	float alph;

	void Start () {
		// Init AudioSource reference.
		_audioSource = GetComponent<AudioSource> ();
		// Obtains the audio clip from the Program Manager.
		_audioSource.clip = ProgramManager.instance.GetComponent<AudioSource> ().clip;

		// Init arrays.
		blipDelay = new int[16];
		onsets = new float[colmax];
		scorefun = new float[colmax];
		dobeat = new float[colmax];
		spectrum = new float[bufferSize];
		averages = new float[12];
		acVals = new float[maxlag];
		alph = 100 * threshold;

		samplingRate = _audioSource.clip.frequency;

		framePeriod = (float)bufferSize / (float)samplingRate;

		// Initialize a record of previous spectrum.
		spec = new float[NO_OF_BANDS];
		for (int i = 0; i < NO_OF_BANDS; ++i)
			spec [i] = 100.0f;

		auco = new Autocorrelator (maxlag, decay, framePeriod, bandwidth);

		_audioSource.Play ();
	}

	void Update () {
		if (_audioSource.isPlaying) {
			int hzValue;

			if (!ProgramManager.instance.useDefaultDict && ProgramManager.isProgramCalibrated) {
				// Uses calibrated dictionary.
				FFTAnalyser.AnalyseSound (_audioSource, ProgramManager.pitchFreqDict, out samples, out spectrum, out hzValue);
			} else {
				// Uses default dictionary.
				FFTAnalyser.AnalyseSound (_audioSource, ProgramManager.defaultPitchDict, out samples, out spectrum, out hzValue);
			}
			ComputeAverages (spectrum);

			// The sum of volumes in all bands in the spectrum.
			float onset = 0;
			for (int i = 0; i < NO_OF_BANDS; i++) {
				// Volume of this band.
				float dbValue = ((float)System.Math.Max (-100.0f, 20.0f * (float)System.Math.Log10 (averages [i]) + 160)) * 0.025f;
				// The difference in volume from the last frame.
				float dbDiff = dbValue - spec [i];
				// Store value for use in next loop.
				spec [i] = dbValue;
				onset += dbDiff;
			}

			onsets [currTimeIndex] = onset;

			// Updates autocorrelator and find peak lag (estimated tempo).
			auco.NewVal (onset);
			// Record largest value in (weighted) autocorrelation as it is the most likely tempo.
			float aMax = 0.0f;
			int tempopd = 0;
			for (int i = 0; i < maxlag; ++i) {
				float acVal = (float)System.Math.Sqrt (auco.AutoCo (i));
				if (acVal > aMax) {
					aMax = acVal;
					tempopd = i;
				}
				// Store in array backwards in line with traces.
				acVals [maxlag - 1 - i] = acVal;
			}

			// Calculates DP-ish function to update the best-score function.
			float smax = -999999;
			int smaxix = 0;
			alph = 100 * threshold;
			for (int i = tempopd / 2; i < System.Math.Min (colmax, 2 * tempopd); ++i) {
				// objective function - this beat's cost + score to last beat + transition penalty
				float score = onset + scorefun [(currTimeIndex - i + colmax) % colmax] - alph * (float)System.Math.Pow (System.Math.Log ((float)i / (float)tempopd), 2);
				// To keep track of the best-scoring predecesor.
				if (score > smax) {
					smax = score;
					smaxix = i;
				}
			}

			scorefun [currTimeIndex] = smax;
			float smin = scorefun [0];
			for (int i = 0; i < colmax; ++i)
				if (scorefun [i] < smin)
					smin = scorefun [i];
			for (int i = 0; i < colmax; ++i)
				scorefun [i] -= smin;

			smax = scorefun [0];
			smaxix = 0;
			for (int i = 0; i < colmax; ++i) {
				if (scorefun [i] > smax) {
					smax = scorefun [i];
					smaxix = i;
				}
			}

			dobeat [currTimeIndex] = 0;
			sinceLast++;
			// If current value is largest in the array, it is probably a beat
			if (smaxix == currTimeIndex) {
				
				// Makes sure that the most recent beat was not too recent
				if (sinceLast > tempopd / 4) {
					if (callbacks != null && hzValue != 0) {
						foreach (AudioCallbacks callback in callbacks) {
							callback.OnBeatDetected (hzValue);
						}
					}
					blipDelay [0] = 1;
					// Records that there was a beat at this frame.
					dobeat [currTimeIndex] = 1;
					// Resets counter of frames since previous beat.
					sinceLast = 0;
				}
			}

			if (++currTimeIndex == colmax)
				currTimeIndex = 0;
		}
	}

	/// <summary>Finds the index corresponding to the input frequency.</summary>
	public int CalculateIndex (int frequency) {
		// If frequency is lower than the bandwidth of spectrum [0].
		if (frequency < bandwidth / 2)
			return 0;
		// If frequency is within the bandwidth of spectrum [512].
		if (frequency > samplingRate / 2 - bandwidth / 2)
			return (bufferSize / 2);
		
		float fraction = (float)frequency / (float)samplingRate;
		return (int)System.Math.Round (bufferSize * fraction);
	}

	/// <summary>Computes the averages from the input data.</summary>
	public void ComputeAverages (float[] data) {
		for (int i = 0; i < 12; i++) {
			float avg = 0;
			int lowFreq = i != 0 ? (int)((samplingRate / 2) / (float)System.Math.Pow (2, 12 - i)) : 0;
			int hiFreq = (int)((samplingRate / 2) / (float)System.Math.Pow (2, 11 - i));
			int lowBound = CalculateIndex (lowFreq);
			int hiBound = CalculateIndex (hiFreq);
			for (int j = lowBound; j <= hiBound; j++) {
				avg += data [j];
			}
			avg /= (hiBound - lowBound + 1);
			averages [i] = avg;
		}
	}

	/// <summary>Adds a callback.</summary>
	public void AddAudioCallback (AudioCallbacks callback) { callbacks.Add (callback); }
	/// <summary>Removes the named callback.</summary>
	public void RemoveAudioCallback (AudioCallbacks callback) { callbacks.Remove (callback); }

	public interface AudioCallbacks {
		void OnBeatDetected (int beatFrequency);
	}
}

/// <summary>Computes an array of autocorrelators.</summary>
public class Autocorrelator {
	int delayLength;
	float decay;
	float[] delays;
	float[] outputs;
	int index;

	float[] bpms;
	float[] rweight;
	float wmidbpm = 120f;
	float woctavewidth;

	public Autocorrelator (int delayLength, float alpha, float framePeriod, float bandwidth) {
		woctavewidth = bandwidth;
		decay = alpha;
		this.delayLength = delayLength;
		delays = new float[delayLength];
		outputs = new float[delayLength];
		index = 0;

		// Calculates a log-lag gaussian weighting function for a 120 bpm bias.
		bpms = new float[delayLength];
		rweight = new float[delayLength];
		for (int i = 0; i < delayLength; ++i) {
			bpms [i] = 60.0f / (framePeriod * (float)i);
			rweight [i] = (float)System.Math.Exp (-0.5f * System.Math.Pow (System.Math.Log (bpms [i] / wmidbpm) / System.Math.Log (2.0f) / woctavewidth, 2.0f));
		}
	}

	public void NewVal (float val) {
		delays [index] = val;

		for (int i = 0; i < delayLength; ++i) {
			int delix = (index - i + delayLength) % delayLength;
			outputs [i] += (1 - decay) * (delays [index] * delays [delix] - outputs [i]);
		}

		if (++index == delayLength)
			index = 0;
	}

	/// <summary>Returns the autocorrelator value at a particular lag.</summary>
	public float AutoCo (int del) { return rweight [del] * outputs [del]; }
}