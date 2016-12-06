using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>Game manager.</summary>
public class GameManager : MonoBehaviour {
	/// <summary>The current instance of this script.</summary>
	public static GameManager instance;

	/// <summary>Are the pitch values calibrated?</summary>
	public bool isCalibrated = false;

	/// <summary>The reference to the game object of the calibration canvas.</summary>
	[Header ("Canvas References")]
	public GameObject _calibrationCanvas;
	/// <summary>The reference to the game object of the listener canvas.</summary>
	public GameObject _listenerCanvas;

	/// <summary>The reference to the button for calibrating pitch C4.</summary>
	[Header ("Calibration Button References")]
	public Button _calibrateC4;
	/// <summary>The reference to the button for calibrating pitch E4.</summary>
	public Button _calibrateE4;
	/// <summary>The reference to the button for calibrating pitch G4.</summary>
	public Button _calibrateG4;
	/// <summary>The reference to the button for calibrating pitch C5.</summary>
	public Button _calibrateC5;

	void Awake () { instance = this; }

	void Update () {
		ColorBlock _tempColorBlock;

		// C4 button color
		_tempColorBlock = _calibrateC4.colors;
		if (PitchCalibrator.pitchFreqDict.ContainsKey ("c4")) {
			_tempColorBlock.normalColor = new Color (.5f, 1, .5f);
			_calibrateC4.colors = _tempColorBlock;
		} else {
			_tempColorBlock.normalColor = new Color (1, .5f, .5f);
			_calibrateC4.colors = _tempColorBlock;
		}

		// E4 button color
		_tempColorBlock = _calibrateE4.colors;
		if (PitchCalibrator.pitchFreqDict.ContainsKey ("e4")) {
			_tempColorBlock.normalColor = new Color (.5f, 1, .5f);
			_calibrateE4.colors = _tempColorBlock;
		} else {
			_tempColorBlock.normalColor = new Color (1, .5f, .5f);
			_calibrateE4.colors = _tempColorBlock;
		}

		// G4 button color
		_tempColorBlock = _calibrateG4.colors;
		if (PitchCalibrator.pitchFreqDict.ContainsKey ("g4")) {
			_tempColorBlock.normalColor = new Color (.5f, 1, .5f);
			_calibrateG4.colors = _tempColorBlock;
		} else {
			_tempColorBlock.normalColor = new Color (1, .5f, .5f);
			_calibrateG4.colors = _tempColorBlock;
		}

		// C5 button color
		_tempColorBlock = _calibrateC5.colors;
		if (PitchCalibrator.pitchFreqDict.ContainsKey ("c5")) {
			_tempColorBlock.normalColor = new Color (.5f, 1, .5f);
			_calibrateC5.colors = _tempColorBlock;
		} else {
			_tempColorBlock.normalColor = new Color (1, .5f, .5f);
			_calibrateC5.colors = _tempColorBlock;
		}
	}
}
