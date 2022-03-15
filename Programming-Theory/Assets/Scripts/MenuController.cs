using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class MenuController : MonoBehaviour {
	public Animator startAnimator;
	public Animator arrowAnimator;
	public Animator fadeOutAnimator;

	public GameObject selection1;
	public GameObject selection2;
	public GameObject selection3;
	public GameObject selection4;

	public GameObject border;

	private readonly Dictionary<GameManager.Cars, GameObject> _selectionsDic = new Dictionary<GameManager.Cars, GameObject>();

	private GameObject _highlightedSelection;
	private GameObject _chosenSelection;
	public TMP_Text chosenText;

	public float rotationSpeed = 60f;

	private void Start() {
		GameManager.IsGameStarted = false;
		UIManager.SetCursorVisibility(true);

		_selectionsDic.Add(GameManager.Cars.Convertible, selection1);
		_selectionsDic.Add(GameManager.Cars.Pickup, selection2);
		_selectionsDic.Add(GameManager.Cars.Jumpy, selection3);
		_selectionsDic.Add(GameManager.Cars.SharkTruck, selection4);

		ChooseSelection(GameManager.Cars.Convertible);
	}

	private void Update() {
		if (Input.anyKeyDown) {
			startAnimator.SetTrigger("OnClick");
			arrowAnimator.SetBool("HasClicked", true);
		}

		if (_highlightedSelection != null) {
			Transform highlightedCar = _highlightedSelection.GetComponent<MenuSelection>().car;
			highlightedCar.localRotation *= Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, Vector3.up);
		}
		else if (_chosenSelection != null) {
			MenuSelection menuSelection = _chosenSelection.GetComponent<MenuSelection>();
			menuSelection.car.localRotation *= Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, Vector3.up);
		}

		if (_chosenSelection != null) {
			MenuSelection menuSelection = _chosenSelection.GetComponent<MenuSelection>();
			border.GetComponent<Image>().color = menuSelection.difficultyColor;
			border.transform.SetParent(_chosenSelection.transform);
			border.transform.SetAsFirstSibling();
			border.transform.position = menuSelection.borderReferencePosition.position;
		}
	}

	public void ChooseSelection(GameManager.Cars cRef) { // ABSTRACTION
		_chosenSelection = _selectionsDic[cRef];

		MenuSelection menuSelection = _selectionsDic[cRef].GetComponent<MenuSelection>();
		chosenText.text = menuSelection.car.name;
		chosenText.color = menuSelection.difficultyColor;

		GameManager.Instance.choosenCarType = cRef;
	}

	public void HighlightSelection(GameManager.Cars cRef) { // ABSTRACTION
		_highlightedSelection = _selectionsDic[cRef];
	}

	public void RemoveHighlightedSelection() { // ABSTRACTION
		_highlightedSelection = null;
	}

	public void Play() { // ABSTRACTION
		if (GameManager.Instance == null)
			return;

		GameManager.IsGameOver = false;

		StartCoroutine(ChangeScene());
	}

	private IEnumerator ChangeScene() { // ABSTRACTION
		fadeOutAnimator.SetTrigger("Fade");

		yield return new WaitForSeconds(.75f);
		SceneManager.LoadScene(1);
	}
}
