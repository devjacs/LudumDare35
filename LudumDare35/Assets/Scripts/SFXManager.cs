using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class SFXManager : MonoBehaviour {

	public AudioMixer sfxMixer;
	public AudioMixerSnapshot startSnapshot;
	public AudioMixerSnapshot menuSnapshot;
	public AudioMixerSnapshot inGameSnapshot;
	public AudioMixerSnapshot gameOverSnapshot;

	public AudioSource beginningSound;

	// Use this for initialization
	void Start () {
		menuSnapshot.TransitionTo(3.0f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void PlayBeginningSound() {
		beginningSound.Play();
	}
}
