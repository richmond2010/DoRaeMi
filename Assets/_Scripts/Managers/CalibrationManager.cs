using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>The script that manages the calibration.</summary>
public class CalibrationManager : MonoBehaviour {
	/// <summary>The current instance of this script.</summary>
	public static CalibrationManager instance;

	/// <summary>The reference to the game object of the calibration canvas.</summary>
	[Header ("Canvas References")]
	public GameObject _calibrationCanvas;
	/// <summary>The reference to the game object of the listener canvas.</summary>
	public GameObject _listenerCanvas;

	void Start () { instance = this; }
}
