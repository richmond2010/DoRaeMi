using UnityEngine;

/// <summary>Script that contains all the button functionality.</summary>
public class ButtonManager : MonoBehaviour {

	public void StartFrequencyListener (string pitch) { PitchCalibrator.StartFrequencyListener (pitch); }

	public void SetPitchFrequency () { PitchCalibrator.SetPitchFrequency (); }

	public void Calibrate () { PitchCalibrator.Calibrate (); }
}
