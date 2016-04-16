using UnityEngine;
using System.Collections;

public class PersonFeatures : MonoBehaviour {

	private Material colour;
	private Renderer renderer;
	private bool enabled = true;

	void Start() {
		renderer = GetComponent<Renderer>();
		renderer.enabled = true;
	}

	public void SetColour(Material colour) {
		this.colour = colour;
		renderer.sharedMaterial = colour;
	}

	public Material GetColour() {
		return colour;
	}

	public bool GetEnabled() {
		return enabled;
	}

	public void SetEnabled(bool enabled) {
		this.enabled = enabled;
		renderer.enabled = enabled;
	}
}
