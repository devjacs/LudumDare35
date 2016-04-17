﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	private int currentRound = 1;
	private int secondsLeftInRound = 20;
	private int secondsInterval = 1;
	private int blinkInterval = 3;
	private float nextSecond = 0;

	//temp variables
	public Material[] capsuleColours = new Material[2]; //red & blue
	public GameObject[] people = new GameObject[12];
	private int evilPerson;

	public Text countdownText;

	public SFXManager sfxManager;

	// Use this for initialization
	void Start () {
		RestartGame();
		//set the players positions..
		int radius = 5;
		for(int i = 1; i <= 12; i++) {
			float x = radius * Mathf.Cos(Mathf.PI / -2 + (2 * i * Mathf.PI) / 12);
			float z = radius * Mathf.Sin(Mathf.PI / -2 + (2 * i * Mathf.PI) / 12);
			people[i-1].transform.position = new Vector3(x, 1.25f, z);
		}
	}

	public void RestartGame() {
		currentRound = 1;
		secondsLeftInRound = 20;
		nextSecond = 0;
		countdownText.text = secondsLeftInRound.ToString();
		evilPerson = Random.Range(0, people.Length);
		foreach (GameObject person in people) {
			int materialIndex = Random.Range(0, 2);
			person.GetComponent<PersonFeatures>().SetColour(capsuleColours[materialIndex]);
		}

		sfxManager.PlayBeginningSound();
	}

	void UpdateBadGuy() {
		/******** Update Bad Guys appearance *********/
		//they have to change and can't be the same as the previous turn..
		Material currentMaterial = people[evilPerson].GetComponent<PersonFeatures>().GetColour();
		int materialIndex = Random.Range(0, 2);
		while (capsuleColours[materialIndex] == currentMaterial) {
			materialIndex = Random.Range(0, 2);
		}
		people[evilPerson].GetComponent<PersonFeatures>().SetColour(capsuleColours[materialIndex]);



		/******** Remove one player **********/
		int personToKill = Random.Range(0, people.Length);
		while (personToKill == evilPerson || !people[personToKill].GetComponent<PersonFeatures>().GetEnabled()) {
			personToKill = Random.Range(0, people.Length);
		}
		people[personToKill].GetComponent<PersonFeatures>().SetEnabled(false);

		sfxManager.PlayBeginningSound();
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
				//when the screen is black
				UpdateBadGuy();
			} else {
				secondsLeftInRound--;
				nextSecond += secondsInterval;
			}
			countdownText.text = secondsLeftInRound.ToString();
		}
	}
}
