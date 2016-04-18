using UnityEngine;
using System.Collections;

public class PersonFeatures : MonoBehaviour {

	private Material colour;
	//private Renderer renderer;
	private bool enabled = true;

	private Material hairColour;
	private Material skinColour;
	private Vector3 position;

	public void SetHairColour(Material hairMaterial) {
		this.hairColour = hairMaterial;
		var i = this.GetComponentInChildren<Transform>().Find("Hair");
		GameObject hair = i.gameObject;
		if (hair != null) {
			hair.GetComponent<Renderer>().material = hairMaterial;
		}
	}

	public Material GetHairColour() {
		return this.hairColour;
	}

	public Material GetSkinColour() {
		return this.skinColour;
	}

	public void SetSkinColour(Material skinMaterial) {
		this.skinColour = skinMaterial;
		GameObject person = this.GetComponentInChildren<Transform>().Find("Cube").gameObject;
		Material[] materials = person.GetComponent<Renderer>().materials;
		for (int i = 0; i < materials.Length; i++) {
			if (materials[i].name == "Skin (Instance)" || materials[i].name == "Material_004" || materials[i].name == "Skin") {
				materials[i] = skinMaterial;
			}
		}
		person.GetComponent<Renderer>().materials = materials;
	}

	public bool GetEnabled() {
		return enabled;
	}

	public void SetEnabled(bool enabled) {
		this.enabled = enabled;
		//this.GetComponentInChildren<Transform>().Find("Cube").gameObject.SetActive(enabled);
		this.GetComponentInChildren<Transform>().Find("Cube").gameObject.SetActive(enabled);
		//this.gameObject.SetActive(enabled);
		//renderer.enabled = enabled;
	}
}
