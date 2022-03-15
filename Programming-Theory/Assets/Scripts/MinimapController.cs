using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MonoBehaviour {

	[System.Serializable]
	public struct MapObject {
		public Image icon;
		public GameObject owner;
	}

	public Camera mapCamera;

	public List<MapObject> mapObjects = new List<MapObject>();

	public void RegisterMapObject(GameObject obj, Image img) { // ABSTRACTION
		Image image = Instantiate(img, transform);
		mapObjects.Add(new MapObject() { owner = obj, icon = image });
	}

	public void RemoveMapObject(GameObject obj) { // ABSTRACTION
		List<MapObject> newList = new List<MapObject>();
		for (int i = 0; i < mapObjects.Count; i++) {
			if (mapObjects[i].owner == obj) {
				Destroy(mapObjects[i].icon);
				if (transform.GetChild(1) != null) Destroy(transform.GetChild(1).gameObject);
				continue;
			} else
				newList.Add(mapObjects[i]);
		}

		mapObjects.RemoveRange(0, mapObjects.Count);
		mapObjects.AddRange(newList);
	}

	public void DrawMapIcons() { // ABSTRACTION
		foreach (MapObject mo in mapObjects) {
			Vector3 screenPos = mapCamera.WorldToViewportPoint(mo.owner.transform.position);

			RectTransform rt = GetComponent<RectTransform>();
			Vector3[] corners = new Vector3[4];
			rt.GetLocalCorners(corners);

			screenPos.x *= rt.rect.width;
			screenPos.y *= rt.rect.height;
			screenPos.z = 0;

			mo.icon.transform.localPosition = new Vector3(
				Mathf.Clamp(screenPos.x - rt.rect.width/2, corners[0].x, corners[2].x),
				Mathf.Clamp(screenPos.y - rt.rect.height/2, corners[0].y, corners[1].y),
				screenPos.z
			);
		}
	}

	private void Update() {
		DrawMapIcons();
	}
}
