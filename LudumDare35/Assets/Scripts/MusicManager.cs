using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour {

	public AudioMixer masterMixer;
	public AudioMixerSnapshot menuSnapshot;
	public AudioMixerSnapshot[] inGameSnapshots;
	public AudioMixerSnapshot gameoverSnapshot;
	public float fadeTime = 5.0f;
	private int currentSnapshot = 0;

	public void PlayMenuSnapshot() {
		menuSnapshot.TransitionTo(0.0f);
	}

	public void PlayGameoverSnapshot() {
		gameoverSnapshot.TransitionTo(0.0f);
	}

	public void AdvanceMusicSnapshot() {
		if (currentSnapshot < inGameSnapshots.Length) {
			inGameSnapshots[currentSnapshot].TransitionTo(fadeTime);
			currentSnapshot++;
		}
	}
}
