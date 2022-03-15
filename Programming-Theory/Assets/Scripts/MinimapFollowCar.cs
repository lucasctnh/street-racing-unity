using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapFollowCar : MonoBehaviour {
	public static Transform Car { get; set; } // ENCAPSULATION

	private void LateUpdate() {
		transform.position = new Vector3(Car.position.x, transform.position.y, Car.position.z);
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, Car.eulerAngles.y, transform.eulerAngles.z);
	}
}
