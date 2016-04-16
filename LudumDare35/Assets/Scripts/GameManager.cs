using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	private int currentRound = 1;
	private int secondsLeftInRound = 20;
	private int secondsInterval = 1;
	private int blinkInterval = 3;
	private float nextSecond = 0;

	public Text countdownText;


	// Use this for initialization
	void Start () {
		RestartGame();
	}

	public void RestartGame() {
		currentRound = 1;
		secondsLeftInRound = 20;
		nextSecond = 0;
		countdownText.text = secondsLeftInRound.ToString();
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time >= nextSecond) {
			//update time
			if (secondsLeftInRound == 0) {
				//TODO: finish blink
				secondsLeftInRound = 20;
				countdownText.color = Color.white;
				nextSecond += secondsInterval;
			} else if (secondsLeftInRound == 1) {
				//TODO: blink
				secondsLeftInRound = 0;
				countdownText.color = Color.red;
				nextSecond += blinkInterval;
			} else {
				secondsLeftInRound--;
				nextSecond += secondsInterval;
			}
			countdownText.text = secondsLeftInRound.ToString();
		}
	}
}
