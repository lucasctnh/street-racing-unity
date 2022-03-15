using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimePopup : MonoBehaviour {
	public Vector3 desiredPos;
	public Transform timerText;

	private float _timerFactor = 0f;

	private void Update() {
		desiredPos = new Vector3(timerText.localPosition.x, timerText.localPosition.y - 70f, timerText.localPosition.z);
		transform.localPosition = Vector3.Lerp(timerText.localPosition, desiredPos, _timerFactor);
		if (_timerFactor <= 1f) _timerFactor += .1f;
	}
}
