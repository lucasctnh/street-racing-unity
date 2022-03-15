using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {
	[SerializeField] private GameObject pauseMenuButtons;
	[SerializeField] private bool isGamePaused = false;

	private void Start() {
		pauseMenuButtons.SetActive(isGamePaused);
	}

	private void Update() {
		if (GameManager.IsGameStarted && !GameManager.IsGameOver && Input.GetKeyDown(KeyCode.Escape))
			ResolvePauseMenu(true);
	}

	private void ResolvePauseMenu(bool shouldShowCursor = false) { // ABSTRACTION
		isGamePaused = !isGamePaused;
		pauseMenuButtons.SetActive(isGamePaused);

		if (isGamePaused)
			Time.timeScale = 0f;
		else
			Time.timeScale = 1f;

		UIManager.SetCursorVisibility(shouldShowCursor);
	}

	public void ResumeGame() { // ABSTRACTION
		ResolvePauseMenu();
	}

	public void RecoverVehicle() { // ABSTRACTION
		FindPlayerAndRecover();
		ResolvePauseMenu();
	}

	private void FindPlayerAndRecover() { // ABSTRACTION
		GameManager.Instance.player.transform.position = new Vector3(
			LapsManager.LastCheckpoint.transform.position.x,
			LapsManager.LastCheckpoint.transform.position.y,
			LapsManager.LastCheckpoint.transform.position.z
		);
		GameManager.Instance.player.transform.rotation = Quaternion.Euler(
			0f,
			LapsManager.LastCheckpoint.transform.rotation.eulerAngles.y - 90f,
			0f
		);

		GameManager.Instance.player.GetComponent<Rigidbody>().velocity = Vector3.zero;
		GameManager.Instance.player.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
	}

	public void BackToMenu() { // ABSTRACTION
		ResolvePauseMenu(true);
		SceneManager.LoadScene(0);
	}
}
