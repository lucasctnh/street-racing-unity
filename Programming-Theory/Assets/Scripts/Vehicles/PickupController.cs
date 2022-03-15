using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupController : CarController { // INHERITANCE
	private void Start() {
		if (GameManager.Instance.IsSelectedVehicle(carType) && !GameManager.IsGameOver) {
			InitiateVehicle(transform);
			OnEachWheel(UpdateMesh);
		}
	}

	private void Update() {
		if (GameManager.Instance.IsSelectedVehicle(carType) && !GameManager.IsGameOver)
			ShowSpeed();
	}

	private void FixedUpdate() {
		if (GameManager.Instance.IsSelectedVehicle(carType) && !GameManager.IsGameOver && GameManager.IsGameStarted) {
			float motor = Input.GetAxis("Vertical");
			float steering = Input.GetAxis("Horizontal");
			bool handBrake = Input.GetButton("Jump");

			Move(motor, steering, handBrake);
		}
	}
}
