using UnityEngine;
using System.Collections.Generic;

/// <summary>The script that manages the entire program.</summary>
public class ProgramManager : MonoBehaviour {
	/// <summary>Sampling rate.</summary>
	public static readonly int SAMPLE_RATE = 44100;
	/// <summary>Sample size.</summary>
	public static readonly int SAMPLE_SIZE = 1024;
	/// <summary>Minimum value of the band to determine sample index.</summary>
	public static readonly float THRESHOLD = 0.01f;

	/// <summary>The current instance of this script.</summary>
	public static ProgramManager instance;
	/// <summary>Is the program calibrated?</summary>
	public static bool isProgramCalibrated = false;
	/// <summary>Is the C4 key calibrated?</summary>
	public static bool isC4Calibrated = false;
	/// <summary>Is the F4 key calibrated?</summary>
	public static bool isF4Calibrated = false;
	/// <summary>Is the A4 key calibrated?</summary>
	public static bool isA4Calibrated = false;
	/// <summary>Is the C5 key calibrated?</summary>
	public static bool isC5Calibrated = false;
	/// <summary>The default values for the pitch-frequency dictionary, according to ISO Standard 16 (A4 = 440 Hz).</summary>
	public static Dictionary<string, int> defaultPitchDict = new Dictionary<string, int> ();
	/// <summary>The calibrated pitch-frequency dictionary.</summary>
	public static Dictionary<string, int> pitchFreqDict = new Dictionary<string, int> ();
	/// <summary>The pitch-MIDI ID dictionary.</summary>
	public static Dictionary<string, int> pitchMidiDict = new Dictionary<string, int> ();

	/// <summary>Is the program using the default pitch-frequency dictionary?</summary>
	public bool useDefaultDict;
	/// <summary>Is the program using MIDI?</summary>
	[HideInInspector] public bool isUsingMidi;
	/// <summary>Is the program displaying frequency?</summary>
	public bool isShowingFrequency;

	void Awake () {
		if (!instance) instance = this;
		else Destroy (gameObject);
		DontDestroyOnLoad (gameObject);
	}

	void Start () {
		#region pitchMidi init
		pitchMidiDict.Add ("C4", 60);
		pitchMidiDict.Add ("C#4",61);
		pitchMidiDict.Add ("D4", 62);
		pitchMidiDict.Add ("D#4",63);
		pitchMidiDict.Add ("E4", 64);
		pitchMidiDict.Add ("F4", 65);
		pitchMidiDict.Add ("F#4",66);
		pitchMidiDict.Add ("G4", 67);
		pitchMidiDict.Add ("G#4",68);
		pitchMidiDict.Add ("A4", 69);
		pitchMidiDict.Add ("A#4",70);
		pitchMidiDict.Add ("B4", 71);
		pitchMidiDict.Add ("C5", 72);
		pitchMidiDict.Add ("C#5",73);
		pitchMidiDict.Add ("D5", 74);
		pitchMidiDict.Add ("D#5",75);
		pitchMidiDict.Add ("E5", 76);
		pitchMidiDict.Add ("F5", 77);
		pitchMidiDict.Add ("F#5",78);
		pitchMidiDict.Add ("G5", 79);
		pitchMidiDict.Add ("G#5",80);
		pitchMidiDict.Add ("A5", 81);
		pitchMidiDict.Add ("A#5",82);
		pitchMidiDict.Add ("B5", 83);
		pitchMidiDict.Add ("C6", 84);
		#endregion

		#region defaultPitch init
		defaultPitchDict.Add ("C4", 262);
		defaultPitchDict.Add ("C#4",277);
		defaultPitchDict.Add ("D4", 294);
		defaultPitchDict.Add ("D#4",311);
		defaultPitchDict.Add ("E4", 330);
		defaultPitchDict.Add ("F4", 349);
		defaultPitchDict.Add ("F#4",370);
		defaultPitchDict.Add ("G4", 392);
		defaultPitchDict.Add ("G#4",415);
		defaultPitchDict.Add ("A4", 440);
		defaultPitchDict.Add ("A#4",466);
		defaultPitchDict.Add ("B4", 494);
		defaultPitchDict.Add ("C5", 523);
		defaultPitchDict.Add ("C#5",554);
		defaultPitchDict.Add ("D5", 587);
		defaultPitchDict.Add ("D#5",622);
		defaultPitchDict.Add ("E5", 659);
		defaultPitchDict.Add ("F5", 698);
		defaultPitchDict.Add ("F#5",740);
		defaultPitchDict.Add ("G5", 784);
		defaultPitchDict.Add ("G#5",831);
		defaultPitchDict.Add ("A5", 880);
		defaultPitchDict.Add ("A#5",932);
		defaultPitchDict.Add ("B5", 988);
		defaultPitchDict.Add ("C6", 1047);
		#endregion
	}
}
