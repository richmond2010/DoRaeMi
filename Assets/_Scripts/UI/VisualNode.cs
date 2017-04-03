using UnityEngine;
using UnityEngine.UI;

/// <summary>Script for interactive input.</summary>
public class VisualNode : MonoBehaviour {
	/// <summary>Is the node in the ring?</summary>
	bool _isInRing;

	void OnTriggerEnter2D (Collider2D col) { if (col.name == "Ring") _isInRing = true; }

	void OnTriggerExit2D () { _isInRing = false; }

	void Update () {
		if (_isInRing && ProgramManager.instance.isUsingMidi && !ProgramManager.instance.isShowingFrequency) {
			if (MidiManager.GetKeyDown (ProgramManager.pitchMidiDict [GetComponentInChildren<Text> ().text])) {
				Destroy (gameObject);
			}
			if (Input.GetKey (KeyCode.Space)) {
				Destroy (gameObject);
			}
		}

		if (MidiManager.GetKeyDown (ProgramManager.pitchMidiDict [GetComponentInChildren<Text> ().text])) {
			Debug.LogWarning ("kek");
		}
	}
}
