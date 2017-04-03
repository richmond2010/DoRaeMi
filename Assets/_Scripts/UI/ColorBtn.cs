using UnityEngine;
using UnityEngine.UI;

/// <summary>Modifies the colour of the button this script is attached to.</summary>
public class ColorBtn : MonoBehaviour {
	[Tooltip ("The name of the reference parameter in ProgramManager.")]
	public string parameter;

	/// <summary>The boolean value of the input parameter.</summary>
	bool _bParameter;
	/// <summary>The reference to the Button component of the GameObject this script is attached to.</summary>
	Button _button;
	/// <summary>The target ColorBlock object to change the colour of the button into.</summary>
	ColorBlock _tempColorBlock;

	void Start () {
		// init values
		_button = GetComponent<Button> ();
		_tempColorBlock = _button.colors;
	}

	void Update () {
		// obtain value of parameter
		switch (parameter) {
		case "isProgramCalibrated":
			_bParameter = ProgramManager.isProgramCalibrated;
			break;
		case "isC4Calibrated":
			_bParameter = ProgramManager.isC4Calibrated;
			break;
		case "isF4Calibrated":
			_bParameter = ProgramManager.isF4Calibrated;
			break;
		case "isA4Calibrated":
			_bParameter = ProgramManager.isA4Calibrated;
			break;
		case "isC5Calibrated":
			_bParameter = ProgramManager.isC5Calibrated;
			break;
		default:
			Debug.LogError ("Please check the name input parameter.");
			break;
		}

		// chance colour
		if (_bParameter) {
			_tempColorBlock.normalColor = new Color (.5f, 1, .5f);	// green
			_button.colors = _tempColorBlock;
		} else {
			_tempColorBlock.normalColor = new Color (1, .5f, .5f);	// red
			_button.colors = _tempColorBlock;
		}
	}
}
