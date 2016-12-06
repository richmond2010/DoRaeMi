using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>The class that calculates the pitch-frequency table using four notes that are played.</summary>
public class PitchCalibrator : MonoBehaviour {
	/// <summary>The pitch-frequency dictionary.</summary>
	public static Dictionary<string, int> pitchFreqDict = new Dictionary<string, int> ();

	public static int lastFrequency;
	static bool c5g4, g4e4, e4c4;
	//static bool isListening = false;
	static string listeningTo = "";

	/// <summary>Starts the calibration listener.</summary>
	/// <param name="pitch">The pitch the recorded frequency is for.</param>
	public static void StartFrequencyListener (string pitch) {
		GameManager.instance._calibrationCanvas.SetActive (false);
		GameManager.instance._listenerCanvas.SetActive (true);

		listeningTo = pitch;
		lastFrequency = 0;							// reset frequency
	}

	/// <summary>Sets the frequency into the pitch.</summary>
	public static void SetPitchFrequency () {
		if (!string.IsNullOrEmpty (listeningTo) && lastFrequency != 0) {
			pitchFreqDict [listeningTo] = lastFrequency;
			listeningTo = "";							// not listening for any pitch

			GameManager.instance._listenerCanvas.SetActive (false);
			GameManager.instance._calibrationCanvas.SetActive (true);
		}
	}

	/// <summary>Checks if there are any pitches that are off.</summary>
	static void AnomalyCheck () {
		// check if the pitches are higher than one another, returns false if anomaly detected.
		c5g4 = pitchFreqDict ["c5"] > pitchFreqDict ["g4"] ? true : false;
		g4e4 = pitchFreqDict ["g4"] > pitchFreqDict ["e4"] ? true : false;
		e4c4 = pitchFreqDict ["e4"] > pitchFreqDict ["c4"] ? true : false;
	}

	/// <summary>Calculates the calibrated frequency of the target pitch.</summary>
	/// <param name="halfSteps">The number of half steps from the defined pitch; positive if above, negative if under.</param>
	/// <param name="definedFrequency">The frequency of the defined pitch.</param>
	static int FrequencyCalculation (int halfSteps, int definedFrequency) {
		return (int)(definedFrequency * Mathf.Pow (Mathf.Pow (2f, 1f / 12), (float)halfSteps));
	}

	/// <summary>Starts the calibration.</summary>
	public static void Calibrate () {
		// checks if base frequencies are sensible
		AnomalyCheck ();

		// if any pitch is off, adjust their values
		if (!e4c4 && g4e4) {
			pitchFreqDict ["c4"] = FrequencyCalculation (-4, pitchFreqDict ["e4"]);
		} else if (!e4c4 && !g4e4 && c5g4) {
			pitchFreqDict ["c4"] = FrequencyCalculation (-7, pitchFreqDict ["g4"]);
			pitchFreqDict ["e4"] = FrequencyCalculation (-3, pitchFreqDict ["g4"]);
		} else if (!e4c4 && !g4e4 && !c5g4) {
			Debug.LogWarning ("Calibration failed.");
			return;								// unable to calibrate
		}

		// check corrected values before calibration
		AnomalyCheck ();

		// fill in the gaps in between
		if (e4c4 && g4e4 && c5g4) {
			pitchFreqDict ["d4"] = (
			    FrequencyCalculation (2, pitchFreqDict ["c4"]) +
			    FrequencyCalculation (-2, pitchFreqDict ["e4"])
			) / 2;

			pitchFreqDict ["f4"] = (
			    FrequencyCalculation (1, pitchFreqDict ["e4"]) +
			    FrequencyCalculation (-2, pitchFreqDict ["g4"])
			) / 2;

			pitchFreqDict ["a4"] = (
			    FrequencyCalculation (2, pitchFreqDict ["g4"]) +
			    FrequencyCalculation (-3, pitchFreqDict ["c5"])
			) / 2;

			pitchFreqDict ["b4"] = (
			    FrequencyCalculation (4, pitchFreqDict ["g4"]) +
			    FrequencyCalculation (-1, pitchFreqDict ["c5"])
			) / 2;
		} else
			return;								// unable to calibrate

		// range is c4 to c6 - fill in the rest
		pitchFreqDict ["d5"] = FrequencyCalculation ( 2, pitchFreqDict ["c5"]);
		pitchFreqDict ["e5"] = FrequencyCalculation ( 4, pitchFreqDict ["c5"]);
		pitchFreqDict ["f5"] = FrequencyCalculation ( 5, pitchFreqDict ["c5"]);
		pitchFreqDict ["g5"] = FrequencyCalculation ( 7, pitchFreqDict ["c5"]);
		pitchFreqDict ["a5"] = FrequencyCalculation ( 9, pitchFreqDict ["c5"]);
		pitchFreqDict ["b5"] = FrequencyCalculation (11, pitchFreqDict ["c5"]);
		pitchFreqDict ["c6"] = FrequencyCalculation (12, pitchFreqDict ["c5"]);

		// sort dictionary into ascending order
		PitchCalibrator.pitchFreqDict.OrderBy (pitch => pitch.Value);
		// calibrated switch
		GameManager.instance.isCalibrated = true;
		Debug.Log ("Calibration successful!");
	}
}
