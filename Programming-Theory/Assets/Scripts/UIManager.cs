using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour {
	public float timer = 3f;

	private bool _canCount = false;
	private TMP_Text _startCounter;
	private GameObject _controls;

	public static void SetCursorVisibility(bool visibility) { // ABSTRACTION
		if (visibility)
			Cursor.lockState = CursorLockMode.None;
		else
			Cursor.lockState = CursorLockMode.Locked;

		Cursor.visible = visibility;
	}

	private void Start() {
		SetCursorVisibility(false);

		_startCounter = GameObject.Find("Start Counter").GetComponent<TMP_Text>();
		_startCounter.enabled = true;
		_startCounter.gameObject.SetActive(false);

		StartCoroutine(WaitForControlsToCount());
	}

	private IEnumerator WaitForControlsToCount() { // ABSTRACTION
		if (GameManager.Instance.choosenCarType == GameManager.Cars.Convertible ||
			GameManager.Instance.choosenCarType == GameManager.Cars.Pickup) {
			_controls = GameObject.Find("Formation 1");
			GameObject.Find("Formation 2").SetActive(false);
		}
		else if (GameManager.Instance.choosenCarType == GameManager.Cars.Jumpy ||
			GameManager.Instance.choosenCarType == GameManager.Cars.SharkTruck) {
			_controls = GameObject.Find("Formation 2");
			GameObject.Find("Formation 1").SetActive(false);
		}

		_controls.SetActive(true);
		yield return new WaitForSeconds(7f);
		_startCounter.gameObject.SetActive(true);
		_canCount = true;
	}

	private void Update() {
		if (_canCount) {
			if (timer >= 0f)
				timer -= Time.smoothDeltaTime;

			_startCounter.text = Mathf.RoundToInt(timer % 60).ToString();
			if (timer <= .5f) {
				GameManager.IsGameStarted = true;
				_startCounter.text = "GO!";

				StartCoroutine(HideTimer());
			}
		}
	}

	private IEnumerator HideTimer() { // ABSTRACTION
		yield return new WaitForSeconds(2f);
		_startCounter.gameObject.SetActive(false);
		_canCount = false;
	}
}
