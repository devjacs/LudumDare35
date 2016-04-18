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

	//public Material[] capsuleColours = new Material[2]; //red & blue
	public Material[] hairColours = new Material[2];
	public Material[] skinColours;
	public GameObject[] people = new GameObject[12];
	private int evilPerson;
	private bool badGuyKilledThisRound = false;

	public GameObject player;
	public Text countdownText;
	public GameObject topLid;
	public GameObject bottomLid;
	public Image menuImage;
	public Image bullet1, bullet2;

	public GameObject manPrefab, womanPrefab;

	public SFXManager sfxManager;
	public MusicManager musicManager;

	// Use this for initialization
	void Start () {
		musicManager.PlayMenuSnapshot();
		//RestartGame();
		currentGameState = GameState.GameStart;
		people = new GameObject[12];
		//set the players positions..
		GeneratePeople();
	}

	void GeneratePeople() {
		evilPerson = Random.Range(0, people.Length);

		int radius = 5;
		for (int i = 0; i < people.Length; i++) {
			
			//generate the hair style and the position
			float x = radius * Mathf.Cos(Mathf.PI / -2 + (2 * (i+1) * Mathf.PI) / 12);
			float z = radius * Mathf.Sin(Mathf.PI / -2 + (2 * (i+1) * Mathf.PI) / 12);
			int hairStyle = Random.Range(0, 2);
			if (hairStyle == 0) {
				people[i] = (GameObject)Instantiate(womanPrefab, new Vector3(x, 0.3f, z), Quaternion.identity);
			} else if (hairStyle == 1) {
				people[i] = (GameObject)Instantiate(manPrefab, new Vector3(x, 0.3f, z), Quaternion.identity);
			}
			people[i].transform.LookAt(2 * people[i].transform.position - player.transform.position);

			//generate the hair colour
			int hairColour = Random.Range(0, hairColours.Length);
			people[i].GetComponent<PersonFeatures>().SetHairColour(hairColours[hairColour]);

			//generate the skin tone
			int skinColour = Random.Range(0, skinColours.Length);
			people[i].GetComponent<PersonFeatures>().SetSkinColour(skinColours[skinColour]);

			people[i].GetComponent<PersonFeatures>().SetEnabled(true);
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
		for (int i = 0; i < people.Length; i++) {
			if (people[i] != null) {
				Destroy(people[i]);
			}
		}
		currentGameState = GameState.Playing;
		sfxManager.PlayBeginningSound();
		musicManager.AdvanceMusicSnapshot();
	}

	/*void SetColours() {
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
	}*/

	void UpdateBadGuy() {
		/******** Update Bad Guys appearance *********/
		//they have to change and can't be the same as the previous turn..
		int thingToChange = Random.Range(0, 3); // 0 = hair style (sex), 1 = hair colour, 2 = skin colour (3 = eyes if implemented.., 4 = shirt..?)
		if (thingToChange == 0) {
			//PersonFeatures.HairStyle currentHairStyle = people[evilPerson].GetComponent<PersonFeatures>().GetH
			int hairStyle = people[evilPerson].name == "Woman(Clone)" ? 0 : 1;
			Material currentHairColour = people[evilPerson].GetComponent<PersonFeatures>().GetHairColour();
			Material currentSkinColour = people[evilPerson].GetComponent<PersonFeatures>().GetSkinColour();
			Destroy(people[evilPerson]);
			if (hairStyle == 1) {
				people[evilPerson] = (GameObject)Instantiate(womanPrefab, new Vector3(people[evilPerson].transform.position.x, 0.3f, people[evilPerson].transform.position.z), Quaternion.identity);
				people[evilPerson].GetComponent<PersonFeatures>().SetHairColour(currentHairColour);
				people[evilPerson].GetComponent<PersonFeatures>().SetSkinColour(currentSkinColour);
			} else if (hairStyle == 0) {
				people[evilPerson] = (GameObject)Instantiate(manPrefab, new Vector3(people[evilPerson].transform.position.x, 0.3f, people[evilPerson].transform.position.z), Quaternion.identity);
				people[evilPerson].GetComponent<PersonFeatures>().SetHairColour(currentHairColour);
				people[evilPerson].GetComponent<PersonFeatures>().SetSkinColour(currentSkinColour);
			}
			people[evilPerson].transform.LookAt(2 * people[evilPerson].transform.position - player.transform.position);
		} else if (thingToChange == 1) {
			Material currentHairColour = people[evilPerson].GetComponent<PersonFeatures>().GetHairColour();
			int newHairColour = Random.Range(0, hairColours.Length);
			while (currentHairColour == hairColours[newHairColour]) {
				newHairColour = Random.Range(0, hairColours.Length);
			}
			people[evilPerson].GetComponent<PersonFeatures>().SetHairColour(hairColours[newHairColour]);
		} else if (thingToChange == 2) {
			Material currentSkinColour = people[evilPerson].GetComponent<PersonFeatures>().GetSkinColour();
			int newSkinColour = Random.Range(0, skinColours.Length);
			while (currentSkinColour == skinColours[newSkinColour]) {
				newSkinColour = Random.Range(0, skinColours.Length);
			}
			people[evilPerson].GetComponent<PersonFeatures>().SetSkinColour(skinColours[newSkinColour]);
		}

		/******** Remove one player **********/
		int personToKill = Random.Range(0, people.Length);
		while (personToKill == evilPerson || people[personToKill] == null) {
			personToKill = Random.Range(0, people.Length);
		}
		Destroy(people[personToKill]);
		sfxManager.PlayBeginningSound();
		musicManager.AdvanceMusicSnapshot();
	}
	
	// Update is called once per frame
	void Update () {

		//check for shooting
		if (Cardboard.SDK.Triggered || Input.GetMouseButtonUp(0)) {
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
				//SetColours();
				GeneratePeople();
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
			sfxManager.PlayGunshot();
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
							musicManager.PlayGameoverSnapshot();
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
				musicManager.PlayGameoverSnapshot();
			}
		}
	}
}
