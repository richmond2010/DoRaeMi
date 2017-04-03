using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class FFTAnalyser {
	/// <summary>Obtain volume and frequency from the referenced AudioSource.</summary>
	/// <param name="audioSource">The AudioSource to analyse.</param>
	/// <param name="samples">The array of samples to write into.</param>
	/// <param name="spectrum">The spectrum array to write into.</param>
	/// <param name="hzValue">The frequency output.</param>
	public static void AnalyseSound (AudioSource audioSource, out float[] samples, out float[] spectrum, out float dbValue, out int hzValue) {

		samples = new float[ProgramManager.SAMPLE_SIZE];
		spectrum = new float[ProgramManager.SAMPLE_SIZE];

		// Obtain samples from the recorded audio.
		audioSource.GetOutputData (samples, 0);

		// Calculate the RMS of the volume.
		float sum = 0;
		for (int i = 0; i < ProgramManager.SAMPLE_SIZE; i++) {
			sum += Mathf.Pow (samples [i], 2);
		}
		float rmsValue = Mathf.Sqrt (sum / ProgramManager.SAMPLE_SIZE);
		dbValue = 20 * Mathf.Log10 (rmsValue / 0.1f);

		// Obtains the audio spectrum data.
		audioSource.GetSpectrumData (spectrum, 0, FFTWindow.BlackmanHarris);
		float highestSample = 0;
		float sampleIndex = 0;

		// Locates the highest sample in the array of spectrums.
		for (int i = 0; i < ProgramManager.SAMPLE_SIZE; i++) {
			if (spectrum [i] > highestSample && spectrum [i] > ProgramManager.THRESHOLD) {
				highestSample = spectrum [i];
				sampleIndex = (float)i;
			}
		}

		// Interpolate index using the immediate left and right neighbours.
		if (sampleIndex > 0 && sampleIndex < ProgramManager.SAMPLE_SIZE - 1) {
			float dL = spectrum [(int)sampleIndex - 1] / spectrum [(int)sampleIndex];
			float dR = spectrum [(int)sampleIndex + 1] / spectrum [(int)sampleIndex];
			sampleIndex += 0.5f * (dR * dR - dL * dL);
		}

		// Convert sample index to frequency.
		hzValue = (int)(sampleIndex * (ProgramManager.SAMPLE_RATE * 0.5045f) / ProgramManager.SAMPLE_SIZE);
	}

	/// <summary>Obtain volume and frequency from the referenced AudioSource.</summary>
	/// <param name="audioSource">The AudioSource to analyse.</param>
	/// <param name="filter">The pitch-frequency dictionary to use as a filter.</param>
	/// <param name="samples">The array of samples to write into.</param>
	/// <param name="spectrum">The spectrum array to write into.</param>
	/// <param name="hzValue">The frequency output.</param>
	public static void AnalyseSound (AudioSource audioSource, Dictionary<string, int> filter, out float[] samples, out float[] spectrum, out int hzValue) {

		samples = new float[ProgramManager.SAMPLE_SIZE];
		spectrum = new float[ProgramManager.SAMPLE_SIZE];

		// Obtains the audio spectrum data.
		audioSource.GetSpectrumData (spectrum, 0, FFTWindow.BlackmanHarris);
		float highestSample = 0;
		float sampleIndex = 0;

		int lowestIndex = (int)System.Math.Round (ProgramManager.SAMPLE_SIZE * (float)filter.Min (x => x.Value) / (float)(ProgramManager.SAMPLE_RATE * 0.5045f));
		int highestIndex = (int)System.Math.Round (ProgramManager.SAMPLE_SIZE * (float)filter.Max (x => x.Value) / (float)(ProgramManager.SAMPLE_RATE * 0.5045f));

		// Locates the highest sample in the array of spectrums.
		for (int i = lowestIndex; i <= highestIndex; i++) {
			if (spectrum [i] > highestSample && spectrum [i] > ProgramManager.THRESHOLD) {
				highestSample = spectrum [i];
				sampleIndex = (float)i;
			}
		}

		// Convert sample index to frequency.
		hzValue = (int)(sampleIndex * (ProgramManager.SAMPLE_RATE * 0.5045f) / ProgramManager.SAMPLE_SIZE);
	}
}
