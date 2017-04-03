using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>The class that calculates the pitch-frequency table using four notes that are played.</summary>
public class PitchCalibrator : MonoBehaviour {
	/// <summary>The value of the last frequency that was detected.</summary>
	public static int lastFrequency;
	static bool c5a4, a4f4, f4c4;
	/// <summary>The pitch of the pitch that the program is currently listening to.</summary>
	static string listeningTo = "";

	/// <summary>Starts the calibration listener.</summary>
	/// <param name="pitch">The pitch the recorded frequency is for.</param>
	public static void StartFrequencyListener (string pitch) {
		CalibrationManager.instance._calibrationCanvas.SetActive (false);
		CalibrationManager.instance._listenerCanvas.SetActive (true);

		listeningTo = pitch;
		lastFrequency = 0;							// reset frequency
	}

	/// <summary>Sets the frequency into the pitch.</summary>
	public static void SetPitchFrequency () {
		if (!string.IsNullOrEmpty (listeningTo) && lastFrequency != 0) {
			ProgramManager.pitchFreqDict [listeningTo] = lastFrequency;

			switch (listeningTo) {
			case "C4":
				ProgramManager.isC4Calibrated = true;
				break;
			case "F4":
				ProgramManager.isF4Calibrated = true;
				break;
			case "A4":
				ProgramManager.isA4Calibrated = true;
				break;
			case "C5":
				ProgramManager.isC5Calibrated = true;
				break;
			}

			listeningTo = "";							// not listening for any pitch

			CalibrationManager.instance._listenerCanvas.SetActive (false);
			CalibrationManager.instance._calibrationCanvas.SetActive (true);
		}
	}

	/// <summary>Checks if there are any pitches that are off.</summary>
	static void AnomalyCheck () {
		// Check if pitches are higher than one another.
		c5a4 = ProgramManager.pitchFreqDict ["C5"] > ProgramManager.pitchFreqDict ["A4"] ? true : false;
		a4f4 = ProgramManager.pitchFreqDict ["A4"] > ProgramManager.pitchFreqDict ["F4"] ? true : false;
		f4c4 = ProgramManager.pitchFreqDict ["F4"] > ProgramManager.pitchFreqDict ["C4"] ? true : false;
	}

	/// <summary>Calculates the calibrated frequency of the target pitch.</summary>
	/// <param name="halfSteps">The number of half steps from the defined pitch; positive if higher, negative if lower.</param>
	/// <param name="referencedFrequency">The frequency of the referenced pitch.</param>
	static int FrequencyCalculation (int halfSteps, int referencedFrequency) {
		return (int)(referencedFrequency * Mathf.Pow (Mathf.Pow (2f, 1f / 12), (float)halfSteps));
	}

	/// <summary>Starts the calibration.</summary>
	public static void Calibrate () {
		// Checks if base frequencies are sensible.
		AnomalyCheck ();

		// Adjusts their values if any pitch doesn't make sense.
		if (!f4c4 && a4f4) {
			ProgramManager.pitchFreqDict ["C4"] = FrequencyCalculation (-4, ProgramManager.pitchFreqDict ["F4"]);
		} else if (!f4c4 && !a4f4 && c5a4) {
			ProgramManager.pitchFreqDict ["C4"] = FrequencyCalculation (-7, ProgramManager.pitchFreqDict ["A4"]);
			ProgramManager.pitchFreqDict ["F4"] = FrequencyCalculation (-3, ProgramManager.pitchFreqDict ["A4"]);
		} else if (!f4c4 && !a4f4 && !c5a4) {
			Debug.LogWarning ("Calibration failed.");
			return;								// unable to calibrate
		}

		// Check corrected values before calibration.
		AnomalyCheck ();

		// Fill the gaps in between.
		if (f4c4 && a4f4 && c5a4) {
			ProgramManager.pitchFreqDict ["C#4"] = (
				FrequencyCalculation (1, ProgramManager.pitchFreqDict ["C4"]) +
				FrequencyCalculation (-4, ProgramManager.pitchFreqDict ["F4"])
			) / 2;

			ProgramManager.pitchFreqDict ["D4"] = (
				FrequencyCalculation (2, ProgramManager.pitchFreqDict ["C4"]) +
				FrequencyCalculation (-3, ProgramManager.pitchFreqDict ["F4"])
			) / 2;

			ProgramManager.pitchFreqDict ["D#4"] = (
				FrequencyCalculation (3, ProgramManager.pitchFreqDict ["C4"]) +
				FrequencyCalculation (-2, ProgramManager.pitchFreqDict ["F4"])
			) / 2;

			ProgramManager.pitchFreqDict ["E4"] = (
				FrequencyCalculation (4, ProgramManager.pitchFreqDict ["C4"]) +
				FrequencyCalculation (-1, ProgramManager.pitchFreqDict ["F4"])
			) / 2;

			ProgramManager.pitchFreqDict ["F#4"] = (
				FrequencyCalculation (1, ProgramManager.pitchFreqDict ["F4"]) +
				FrequencyCalculation (-3, ProgramManager.pitchFreqDict ["A4"])
			) / 2;

			ProgramManager.pitchFreqDict ["G4"] = (
				FrequencyCalculation (2, ProgramManager.pitchFreqDict ["F4"]) +
				FrequencyCalculation (-2, ProgramManager.pitchFreqDict ["A4"])
			) / 2;

			ProgramManager.pitchFreqDict ["G#4"] = (
				FrequencyCalculation (3, ProgramManager.pitchFreqDict ["F4"]) +
				FrequencyCalculation (-1, ProgramManager.pitchFreqDict ["A4"])
			) / 2;

			ProgramManager.pitchFreqDict ["B4"] = (
				FrequencyCalculation (1, ProgramManager.pitchFreqDict ["A4"]) +
				FrequencyCalculation (-1, ProgramManager.pitchFreqDict ["C5"])
			) / 2;
		} else {
			Debug.LogWarning ("Calibration unsuccessful.");
			return;
		}

		// Populate from C#5 to C6.
		ProgramManager.pitchFreqDict ["C#5"] = FrequencyCalculation ( 1, ProgramManager.pitchFreqDict ["C5"]);
		ProgramManager.pitchFreqDict ["D5"]  = FrequencyCalculation ( 2, ProgramManager.pitchFreqDict ["C5"]);
		ProgramManager.pitchFreqDict ["D#5"] = FrequencyCalculation ( 3, ProgramManager.pitchFreqDict ["C5"]);
		ProgramManager.pitchFreqDict ["E5"]  = FrequencyCalculation ( 4, ProgramManager.pitchFreqDict ["C5"]);
		ProgramManager.pitchFreqDict ["F5"]  = FrequencyCalculation ( 5, ProgramManager.pitchFreqDict ["C5"]);
		ProgramManager.pitchFreqDict ["F#5"] = FrequencyCalculation ( 6, ProgramManager.pitchFreqDict ["C5"]);
		ProgramManager.pitchFreqDict ["G5"]  = FrequencyCalculation ( 7, ProgramManager.pitchFreqDict ["C5"]);
		ProgramManager.pitchFreqDict ["G#5"] = FrequencyCalculation ( 8, ProgramManager.pitchFreqDict ["C5"]);
		ProgramManager.pitchFreqDict ["A5"]  = FrequencyCalculation ( 9, ProgramManager.pitchFreqDict ["C5"]);
		ProgramManager.pitchFreqDict ["A#5"] = FrequencyCalculation (10, ProgramManager.pitchFreqDict ["C5"]);
		ProgramManager.pitchFreqDict ["B5"]  = FrequencyCalculation (11, ProgramManager.pitchFreqDict ["C5"]);
		ProgramManager.pitchFreqDict ["C6"]  = FrequencyCalculation (12, ProgramManager.pitchFreqDict ["C5"]);

		// End calibration.
		ProgramManager.pitchFreqDict.OrderBy (pitch => pitch.Value);
		ProgramManager.isProgramCalibrated = true;
		Debug.Log ("Calibration successful!");
	}
}
