using UnityEngine;
using UnityEngine.UI;

/// <summary>The frequency display during calibration.</summary>
public class FrequencyDisplay : MonoBehaviour {
	Text _display;
	void Start () { _display = GetComponent<Text> (); }
	void Update () { _display.text = "Frequency: " + PitchCalibrator.lastFrequency + " Hz"; }
}
