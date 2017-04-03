using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>Script that contains all the button functionality.</summary>
public class ButtonManager : MonoBehaviour {

	public void StartFrequencyListener (string pitch) { PitchCalibrator.StartFrequencyListener (pitch); }

	public void SetPitchFrequency () { PitchCalibrator.SetPitchFrequency (); }

	public void Calibrate () { PitchCalibrator.Calibrate (); }

	public void EnterCalibration () { SceneManager.LoadScene ("Calibration"); }

	public void ExitCalibration () { SceneManager.LoadScene ("Menu"); }

	public void StartProgram () { SceneManager.LoadScene ("Analyser"); }
}
