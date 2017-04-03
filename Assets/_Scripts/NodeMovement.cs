using UnityEngine;

/// <summary>Script that allows the nodes to move left.</summary>
public class NodeMovement : MonoBehaviour {
	public float stuff;

	void FixedUpdate () {
		transform.Translate (Vector3.left * stuff * Time.deltaTime);
	}
}
