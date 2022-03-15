using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class CarController : MonoBehaviour {
	private enum SpeedUnit { Imperial, Metric }
	protected enum WheelType { FrontLeft, FrontRight, RearLeft, RearRight }

	[Serializable]
	private struct CameraPrefs {
		public float distance;
		public float height;
	}

	[Serializable]
	protected struct Wheel {
		public WheelType wheelType;
		public GameObject mesh;
		public WheelCollider collider;
		public TrailRenderer skidmark;
		public float defaultForwardStiffness;
		public float defaultSidewaysStiffness;
	}

	[HideInInspector] public float MotorTorqueDirection { get; protected set; } // ENCAPSULATION
 	[HideInInspector] public float SteerAngle { get; protected set; } // ENCAPSULATION
 	[HideInInspector] public bool IsHandBraking { get; protected set; } // ENCAPSULATION

	[HideInInspector] protected float MaxSteerAngle { get; set; } // ENCAPSULATION
	[HideInInspector] protected bool AreWheelsOnGround { get; set; } // ENCAPSULATION

	public GameManager.Cars carType;

	[SerializeField] protected Rigidbody _rigidbody;
	[SerializeField] protected float _airRotation = 10f;
	[SerializeField] private CameraPrefs _camPrefs;
	[SerializeField] private Transform _centerOfMass;
	[SerializeField] private ParticleSystem _tireSmoke;
	[SerializeField] private Wheel[] _wheels = new Wheel[4];
	[SerializeField] private float _maxTorque = 2000f, _maxBrakeTorque = 500f, _maxSteerAngle = 30f;
	[SerializeField] private float _downForce = 100f;
	[SerializeField] private SpeedUnit _speedUnit;
	[SerializeField] private float _topSpeed = 140;

	protected float CurrentSpeed { // ENCAPSULATION
		get {
			float speed = _rigidbody.velocity.magnitude;
			if (_speedUnit == SpeedUnit.Imperial)
				speed *= 2.23693629f;
			else
				speed *= 3.6f;
			return speed;
		}
	}

	private TMP_Text _speedText;
	private TMP_Text _speedUnitText;
	private Transform _speedometerPointer;

	protected void InitiateVehicle(Transform vehicle) { // ABSTRACTION
		if (_centerOfMass != null && _rigidbody != null)
			_rigidbody.centerOfMass = _centerOfMass.localPosition;

		MinimapFollowCar.Car = vehicle;
		PlayerCamera.ChangeCamPrefs(vehicle, _camPrefs.distance, _camPrefs.height);

		Physics.IgnoreLayerCollision(6, 7, true);

		MaxSteerAngle = _maxSteerAngle;

		Transform playerSpawn = GameObject.Find("Player Spawn").transform;
		Vector3 startPos = new Vector3(
			playerSpawn.position.x,
			transform.position.y,
			playerSpawn.position.z
		);
		transform.position = startPos;

		GameManager.Instance.player = vehicle.gameObject;
	}

	protected void ShowSpeed() { // ABSTRACTION
		if (_speedText != null) {
			_speedText.text = ((int)CurrentSpeed).ToString();

			if (_speedUnit == SpeedUnit.Imperial)
				_speedUnitText.text = " MPH";
			else
				_speedUnitText.text = " KPH";

			// Rotate pointer based on speed
			float pointerRotation = CurrentSpeed.Remap(0, 200, 120, -120);
			_speedometerPointer.transform.localRotation = Quaternion.Euler(0f, pointerRotation, 0f);
		}
	}

	protected void OnEachWheel(Action<Wheel> method) { // ABSTRACTION
		for (int i = 0; i < _wheels.Length; ++i) {
			if (_wheels[i].mesh == null || _wheels[i].collider == null)
				return;

			method(_wheels[i]);
		}
	}

	protected void UpdateMesh(Wheel wheel) { // ABSTRACTION
		wheel.collider.GetWorldPose(out Vector3 position, out Quaternion rotation);
		wheel.mesh.transform.position = position;
		wheel.mesh.transform.rotation = rotation;
	}

	protected virtual void Move(float motorInput, float steerInput, bool handBrakeInput) { // ABSTRACTION
		MotorTorqueDirection = motorInput;
		SteerAngle = steerInput * MaxSteerAngle;
		IsHandBraking = handBrakeInput;

		HandleMovement();
	}

	protected void HandleMovement() { // ABSTRACTION
		if (!AreAnyWheelsOnGround())
			HandleMovementOnAir();

		OnEachWheel(HandleMovementOnGround);

		HandleSpeed();
	}

	private bool AreAnyWheelsOnGround() { // ABSTRACTION
		int onAirCount = 0;

		for (int i = 0; i < _wheels.Length; ++i) {
			if (_wheels[i].mesh == null || _wheels[i].collider == null)
				break;

			HandleWheelOnGround(_wheels[i], ref onAirCount);
		}

		AreWheelsOnGround = !(onAirCount == 4);
		return !(onAirCount == 4);
	}

	private void HandleWheelOnGround(Wheel wheel, ref int wheelsOnAirCounter) { // ABSTRACTION
		if (wheel.collider.GetGroundHit(out WheelHit hit)) {
			Material groundMaterial = hit.collider.GetComponent<Renderer>().material;
			if (groundMaterial != null) {
				wheel.collider.forwardFriction = HandleFriction(groundMaterial.name,
					wheel.collider.forwardFriction,
					wheel.defaultForwardStiffness,
					4f
				);

				wheel.collider.sidewaysFriction = HandleFriction(groundMaterial.name,
					wheel.collider.sidewaysFriction,
					wheel.defaultSidewaysStiffness,
					2f
				);
			}

			if (IsHandBraking || hit.sidewaysSlip > .4f) {
				wheel.skidmark.emitting = true;
				if (_tireSmoke.isStopped || CurrentSpeed > 0) _tireSmoke.Play();
			} else {
				wheel.skidmark.emitting = false;
				if (_tireSmoke.isPlaying) _tireSmoke.Stop();
			}
		} else {
			wheel.skidmark.emitting = false;
			if (_tireSmoke.isPlaying) _tireSmoke.Stop();

			wheelsOnAirCounter++;
		}
	}

	private WheelFrictionCurve HandleFriction(string groundMaterialName, WheelFrictionCurve frictionCurve,
	float defaultStiffness, float divisor) { // ABSTRACTION
		if (groundMaterialName.Contains("Grass"))
			frictionCurve.stiffness = defaultStiffness / divisor;
		else if (groundMaterialName.Contains("Road"))
			frictionCurve.stiffness = defaultStiffness;

		return frictionCurve;
	}

	protected virtual void HandleMovementOnAir() { // ABSTRACTION
		RotateRigidbodyAroundAxis(transform.up, SteerAngle, _airRotation / 3f);
		RotateRigidbodyAroundAxis(transform.right, MotorTorqueDirection, _airRotation);
	}

	protected void RotateRigidbodyAroundAxis(Vector3 axis, float input, float increaser) { // ABSTRACTION
		_rigidbody.transform.RotateAround(
			_centerOfMass.position,
			axis,
			increaser * Mathf.Clamp(input, -1f, 1f) * Time.deltaTime);
	}

	private void HandleMovementOnGround(Wheel wheel) { // ABSTRACTION
		if (CheckWheelType(wheel, "front"))
			wheel.collider.steerAngle = SteerAngle;

		if (!IsHandBraking) {
			wheel.collider.brakeTorque = 0f;

			if (MotorTorqueDirection > 0f) // Accelerating
				wheel.collider.motorTorque = MotorTorqueDirection * _maxTorque / 4f;
			else if (MotorTorqueDirection < 0f) // Deccelerating
				wheel.collider.motorTorque = MotorTorqueDirection * _maxBrakeTorque / 40f;
			else {
				if (CheckWheelType(wheel, "rear"))
					wheel.collider.brakeTorque = _maxBrakeTorque * 1000f;
			}
		} else {
			wheel.collider.motorTorque = 0f;

			if (CheckWheelType(wheel, "rear"))
				wheel.collider.brakeTorque = _maxBrakeTorque * 10000f;
		}

		UpdateMesh(wheel);
		StickToGround(wheel);
	}

	private bool CheckWheelType(Wheel wheel, string type) { // ABSTRACTION
		if (type == "front")
			return (wheel.wheelType == WheelType.FrontLeft || wheel.wheelType == WheelType.FrontRight);
		else if (type == "rear")
			return (wheel.wheelType == WheelType.RearLeft || wheel.wheelType == WheelType.RearRight);
		else
			return false;
	}

	private void StickToGround(Wheel wheel) { // ABSTRACTION
		wheel.collider.attachedRigidbody.AddForce(
			-transform.up * _downForce * wheel.collider.attachedRigidbody.velocity.magnitude
		);
	}

	private void HandleSpeed() { // ABSTRACTION
		float speed = _rigidbody.velocity.magnitude;
		if (_speedUnit == SpeedUnit.Imperial) {
			if (speed > _topSpeed)
				_rigidbody.velocity = (_topSpeed / 2.23693629f) * _rigidbody.velocity.normalized;
		} else {
			speed *= 3.6f;
			if (speed > _topSpeed)
				_rigidbody.velocity = (_topSpeed / 3.6f) * _rigidbody.velocity.normalized;
		}
	}

	private void Awake() {
		_speedText = GameObject.Find("Speed Text").GetComponent<TMP_Text>();
		_speedUnitText = GameObject.Find("Speed Unit Text").GetComponent<TMP_Text>();
		_speedometerPointer = GameObject.Find("Speedometer Pointer").transform;
	}
}
