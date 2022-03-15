using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LapsManager : MonoBehaviour {

	[SerializeField] private Image checkpointIcon;
	[SerializeField] private MinimapController minimapController;
	[SerializeField] private GameObject[] _ringLaps = new GameObject[3];
	[SerializeField] private List<GameObject> _currentList;

	private int _currentLap = 0;
	private GameObject _nextCheckpoint;

	[HideInInspector] public static GameObject LastCheckpoint;

	private float _timer = 0f;
	private System.TimeSpan _currentTime;
	private System.TimeSpan _totalTime;
	private bool _isCounting = false;
	[SerializeField] private TMP_Text _timerText;
	[SerializeField] private TMP_Text _lapCounterText;

	private void Start() {
		_currentLap = 0;
		LastCheckpoint = GameObject.Find("Player Spawn");
		PopulateList(_currentList, _ringLaps[_currentLap]);
	}

	private void Update() {
		if (_isCounting) _timer = Mathf.Max(0, _timer + Time.deltaTime);
		_currentTime = System.TimeSpan.FromSeconds(_timer);
		_timerText.text = _currentTime.Hours.ToString("00") + ":" +
			_currentTime.Minutes.ToString("00") + ":" +
			_currentTime.Seconds.ToString("00") + "." +
			_currentTime.Milliseconds / 100;
	}

	private void PopulateList(List<GameObject> list, GameObject parent) { // ABSTRACTION
		Transform[] bodies = parent.GetComponentsInChildren<Transform>();
		foreach (Transform body in bodies) {
			if (!body.name.Contains("lap")) {
				list.Add(body.gameObject);
				body.gameObject.SetActive(false);
			}
		}

		AddNextCheckpoint(list);
	}

	private void AddNextCheckpoint(List<GameObject> rings) { // ABSTRACTION
		if (rings.Count == 0) {
			_isCounting = false;
			_currentList.Clear();
			_ringLaps[_currentLap].SetActive(false);
			_currentLap++;
			if (_currentLap <= 2) {
				_ringLaps[_currentLap].SetActive(true);
				PopulateList(_currentList, _ringLaps[_currentLap]);

				_lapCounterText.text = (_currentLap + 1).ToString();
			} else
				HighscoreController.Instance.EndGame(_totalTime);

			_timer = 0f;
			_totalTime += _currentTime;
			if (_currentLap <= 2) PopupSaveLastTime();
		} else {
			if (GameManager.IsGameStarted)
				_isCounting = true;

			if (_currentLap <= 2) {
				rings[0].SetActive(true);
				_nextCheckpoint = rings[0];

				minimapController.RegisterMapObject(_nextCheckpoint, checkpointIcon);
			}
		}
	}

	public void RemoveLastCheckpoint(GameObject ring) { // ABSTRACTION
		if (!_nextCheckpoint.GetComponent<LapRing>().shouldSkip)
			LastCheckpoint = _nextCheckpoint;

		if (_nextCheckpoint == ring) {
			_currentList[0].SetActive(false);
			minimapController.RemoveMapObject(_nextCheckpoint);

			_currentList.RemoveAt(0);
			_currentList.TrimExcess();
			AddNextCheckpoint(_currentList);
		}
	}

	private void PopupSaveLastTime() { // ABSTRACTION
		GameObject _timerClone = Instantiate(_timerText.gameObject, _timerText.transform.position, _timerText.transform.rotation);
		_timerClone.transform.SetParent(_timerText.transform.parent);
		_timerClone.transform.localScale = new Vector3(1, 1, 1);
		_timerClone.transform.localPosition = _timerText.transform.localPosition;
		_timerClone.GetComponent<TMP_Text>().color = Color.red;
		Vector3 desiredPos = new Vector3(_timerText.transform.localPosition.x, _timerText.transform.localPosition.y - 70f, _timerText.transform.localPosition.z);
		_timerClone.transform.localPosition = desiredPos;

		_timerClone.AddComponent<TimePopup>();
		_timerClone.GetComponent<TimePopup>().timerText = _timerText.transform;

		Destroy(_timerClone, 6f);
	}
}
