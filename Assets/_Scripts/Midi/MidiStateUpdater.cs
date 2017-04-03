using UnityEngine;

public class MidiStateUpdater : MonoBehaviour {
	public delegate void Callback ();

	public static void CreateGameObject (Callback callback) {
		var go = new GameObject ("MIDI Updater");

		GameObject.DontDestroyOnLoad (go);
		go.hideFlags = HideFlags.HideInHierarchy;

		var updater = go.AddComponent<MidiStateUpdater> ();
		updater._callback = callback;
	}

	Callback _callback;

	void Update () { _callback (); }
}
