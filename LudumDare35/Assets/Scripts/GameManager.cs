using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	enum GameState {GameStart, Playing, GameOver};

	private int currentRound;
	private int secondsLeftInRound;
	private int secondsInterval = 1;
	private int blinkInterval = 3;
	private float nextSecond;
	private int bullets;
	private GameState currentGameState;
	private bool initialOpenEyes = false;
	private bool won = false;
	private float lastShot = 0; //this is a bit of a hack to prevent double bullets being shot at once..

	//temp variables
	public Material[] capsuleColours = new Material[2]; //red & blue
	public GameObject[] people = new GameObject[12];
	private int evilPerson;
	private bool badGuyKilledThisRound = false;

	public Text countdownText;
	public GameObject topLid;
	public GameObject bottomLid;
	public Image menuImage;
	public Image bullet1, bullet2;

	public SFXManager sfxManager;
	public MusicManager musicManager;

	// Use this for initialization
	void Start () {
		musicManager.PlayMenuSnapshot();
		//RestartGame();
		currentGameState = GameState.GameStart;
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
		nextSecond = Time.time + 5;
		bullets = 2;
		bullet1.enabled = true;
		bullet2.enabled = true;
		initialOpenEyes = false;
		countdownText.text = secondsLeftInRound.ToString();
		currentGameState = GameState.Playing;
		sfxManager.PlayBeginningSound();
	}

	void SetColours() {
		evilPerson = Random.Range(0, people.Length);
		foreach (GameObject person in people) {
			int materialIndex = Random.Range(0, 2);
			person.GetComponent<PersonFeatures>().SetColour(capsuleColours[materialIndex]);
		}
		foreach (GameObject person in people) {
			person.GetComponent<PersonFeatures>().SetEnabled(true);
		}
		sfxManager.PlayBeginningSound();
		musicManager.AdvanceMusicSnapshot();
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
		musicManager.AdvanceMusicSnapshot();
	}
	
	// Update is called once per frame
	void Update () {

		//check for shooting
		if (Cardboard.SDK.Triggered || Input.GetMouseButtonUp(0)) {
			//TODO: play a gun noise
			if (currentGameState == GameState.GameStart || (currentGameState == GameState.GameOver && Time.time >= lastShot + 3)) {
				BlinkClose();
				initialOpenEyes = false;
				menuImage.enabled = false;
				RestartGame();
			} else if (currentGameState == GameState.Playing && initialOpenEyes) {
				//shoot!
				Shoot();
			}
		}

		if (currentGameState == GameState.Playing) {
			//open eyes when first playing
			if (!initialOpenEyes && Time.time >= nextSecond - 1) {
				SetColours();
				BlinkOpen();
				initialOpenEyes = true;
			}

			//blink & kill
			if (secondsLeftInRound == 0 && !badGuyKilledThisRound) {
				if (Time.time >= nextSecond - 1.5) {
					UpdateBadGuy();
					badGuyKilledThisRound = true;
				}
			}

			//update the clock, check for blinks
			if (Time.time >= nextSecond) {
				if (secondsLeftInRound == 0) {
					BlinkOpen();
					secondsLeftInRound = 20;
					countdownText.color = Color.white;
					nextSecond += secondsInterval + 1;
				} else if (secondsLeftInRound == 1) {
					BlinkClose();
					secondsLeftInRound = 0;
					countdownText.color = Color.red;
					nextSecond += blinkInterval;
					badGuyKilledThisRound = false;
				} else {
					secondsLeftInRound--;
					nextSecond += secondsInterval;
				}
				countdownText.text = secondsLeftInRound.ToString();
			}
		}
	}

	void BlinkClose() {
		topLid.gameObject.GetComponent<Animator>().Play("CloseTopLid");
		bottomLid.gameObject.GetComponent<Animator>().Play("CloseBottomLid");
	}

	void BlinkOpen() {
		topLid.gameObject.GetComponent<Animator>().Play("OpenTopLid");
		bottomLid.gameObject.GetComponent<Animator>().Play("OpenBottomLid");
	}

	void Shoot() {
		if (Time.time >= lastShot + 3) {
			lastShot = Time.time;
			bullets--;
			if (bullets == 1) {
				bullet1.enabled = false;
			} else if (bullets == 0) {
				bullet2.enabled = false;
			}
			Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit)) {
				foreach (GameObject person in people) {
					if (hit.collider.gameObject == person) {
						//they hit a person..
						if (person == people[evilPerson]) {
							//you succeeded
							won = true;
							currentGameState = GameState.GameOver;
							Debug.Log("game over! you win");
							menuImage.enabled = true;
						} else {
							//you killed a person
							person.GetComponent<PersonFeatures>().SetEnabled(false);
						}
					}
				}
			}
			if (!won && bullets == 0) {
				//you lose
				currentGameState = GameState.GameOver;
				Debug.Log("game over! you lose!");
				menuImage.enabled = true;
			}
		}
	}
}
