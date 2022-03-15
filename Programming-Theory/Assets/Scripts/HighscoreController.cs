using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class HighscoreController : MonoBehaviour {
	public static HighscoreController Instance { get; private set; } // ENCAPSULATION

	[SerializeField] private TMP_Text convertibleScore;
	[SerializeField] private TMP_Text pickupScore;
	[SerializeField] private TMP_Text jumpyScore;
	[SerializeField] private TMP_Text sharkScore;
	[SerializeField] private GameObject highscoreContents;

	private bool _isGamePaused = false;

	private void Awake() {
		if (Instance != null) {
			Destroy(gameObject);
			return;
		}

		Instance = this;
	}

	public void EndGame(System.TimeSpan time) { // ABSTRACTION
		GameManager.IsGameOver = true;
		GameManager.Instance.SaveHighscore(time, GameManager.Instance.choosenCarType);

		UIManager.SetCursorVisibility(true);
		ResolveScreen();
	}

	private void ResolveScreen() { // ABSTRACTION
		_isGamePaused = !_isGamePaused;
		highscoreContents.SetActive(_isGamePaused);

		if (_isGamePaused)
			Time.timeScale = 0f;
		else
			Time.timeScale = 1f;
	}

	public void BackToMenu() { // ABSTRACTION
		ResolveScreen();
		SceneManager.LoadScene(0);
	}

	private void Update() {
		if (GameManager.IsGameOver) {
			GameManager.HighscoreData score = GameManager.Instance.highscoreData;
			System.TimeSpan cScore = score.convertibleScore;
			System.TimeSpan pScore = score.pickupScore;
			System.TimeSpan jScore = score.jumpyScore;
			System.TimeSpan sScore = score.sharkScore;

			convertibleScore.text = cScore.Hours.ToString("00") + ":" +
				cScore.Minutes.ToString("00") + ":" +
				cScore.Seconds.ToString("00") + "." +
				cScore.Milliseconds/100;

			pickupScore.text = pScore.Hours.ToString("00") + ":" +
				pScore.Minutes.ToString("00") + ":" +
				pScore.Seconds.ToString("00") + "." +
				pScore.Milliseconds/100;

			jumpyScore.text = jScore.Hours.ToString("00") + ":" +
				jScore.Minutes.ToString("00") + ":" +
				jScore.Seconds.ToString("00") + "." +
				jScore.Milliseconds/100;

			sharkScore.text = sScore.Hours.ToString("00") + ":" +
				sScore.Minutes.ToString("00") + ":" +
				sScore.Seconds.ToString("00") + "." +
				sScore.Milliseconds/100;
		}
	}
}
