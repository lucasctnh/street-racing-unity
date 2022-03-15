using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapRing : MonoBehaviour {
	public bool shouldSkip = false;
	private LapsManager lapsManager;

	private void Start() {
		lapsManager = transform.parent.parent.GetComponent<LapsManager>();
	}

	private void OnTriggerExit(Collider other) { // ABSTRACTION
		if (lapsManager != null)
			lapsManager.RemoveLastCheckpoint(gameObject);
	}
}
