using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils {
	public static float Remap(this float value, float from1, float from2, float to1, float to2) { // ABSTRACTION
		return (value - from1) / (from2 - from1) * (to2 - to1) + to1;
	}
}
