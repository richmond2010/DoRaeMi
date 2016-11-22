using UnityEngine;
using System.Collections;

/// <summary></summary>Controller.</summary>
public class Controller : MonoBehaviour {
	/// <summary>The current instance of this script.</summary>
	public static Controller instance;
	/// <summary>The force of the jump.</summary>
	public float force;

	/// <summary>Is the avatar jumping?</summary>
	bool isJumping;

	void Awake () { instance = this; }

	void Start () { isJumping = false; }

	// if avatar comes into contact with floor, jump switch is disabled.
	void OnCollisionEnter (Collision col) { if (col.gameObject.layer == 8) isJumping = false; }

	void Update () {
		// if player inputs 'Space' and jump switch is disabled
		if (Input.GetKeyDown (KeyCode.Space) && !isJumping) {
			// add upward force to avatar
			instance.GetComponent<Rigidbody> ().AddForce (Vector3.up * force);
			// enable jump switch
			isJumping = true;
		}
	}
}
