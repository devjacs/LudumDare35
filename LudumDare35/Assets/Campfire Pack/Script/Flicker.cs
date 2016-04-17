using UnityEngine;
using System.Collections;

public class Flicker : MonoBehaviour {

	private Light light;
	private Color originalColor;
	private float timePassed;
	private float changeValue;

	void Start()
	{
		light = GetComponent<Light> ();

		if (light != null) 
		{
			originalColor = light.color;
		} 
		else 
		{
			enabled = false;
			return;
		}

		changeValue = 0;
		timePassed = 0;
	}

	// Update is called once per frame
	void Update () {
		timePassed = Time.time;
		timePassed = timePassed - Mathf.Floor (timePassed);
		light.color = originalColor * CalculateChange ();
	}

	private float CalculateChange()
	{
		changeValue = -Mathf.Sin (timePassed * 12 * Mathf.PI) * 0.05f + 0.95f;
		return changeValue;
	}
}
