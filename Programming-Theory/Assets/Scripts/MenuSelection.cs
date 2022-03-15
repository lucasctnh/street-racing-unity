using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuSelection : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler { // INHERITANCE

	public MenuController menuController;
	public GameManager.Cars carRef;
	public Transform car;
	public Transform borderReferencePosition;
	public Color difficultyColor;
	public bool isBorderActive = false;

	public void OnPointerClick(PointerEventData eventData) {
		menuController.ChooseSelection(carRef);
	}

	public void OnPointerEnter(PointerEventData eventData) {
		menuController.HighlightSelection(carRef);
	}

	public void OnPointerExit(PointerEventData eventData) {
		menuController.RemoveHighlightedSelection();
	}
}
