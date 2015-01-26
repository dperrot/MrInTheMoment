using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public BackgroundScript background;
	public MainCharacterScript mainCharacter;
	public GrayOverlayScript grayOverlay;
	public FireSpawnScript fireSpawn;
	public NPCSpawnScript NPCSpawn;
	public List<AudioSource> mPlayer = new List<AudioSource>();
	public float inBetweenTransition;
	public float musicVolume;
	public int maxMoralityCheck;
	public float minWaitTime;
	public float maxWaitTime;
	public float NPCDelay;
	public float minNPCDropTime;
	public float maxNPCDropTime;
	public float minSlowTime;
	public float maxSlowTime;
	public float NPCBetweenTime;
	public Text highScoreText;
	public Text curScoreText;
	public Text moralityCheckText;
	public Text timeLeftText;
	
	private List<FireScript> fireList = new List<FireScript>();
	private List<NPCScript> NPCList = new List<NPCScript>();
	private List<NPCScript> NPCSelectList = new List<NPCScript>();
	private GameCoroutineType curCoroutine = GameCoroutineType.Inactive;
	private float highScore;
	private float score;
	private int moralityCheck;

	// Singleton stuff
	private static GameManager instance;
	public static GameManager Instance() {
		return instance;
	}
	private void Awake() {
		if(instance != null) { Destroy(gameObject); return; }
		instance = this;
	}

	public GameCoroutineType getCoroutine() { return curCoroutine; }

	// Music handling + volume control set.
	public void PlayMusic(int id, float volume) { 
		mPlayer[id].volume = volume;
		mPlayer[id].Play(); 
	}
	public void StopMusic(int id) { mPlayer[id].Stop(); }

	// Text updating function
	public void UpdateText(Text text, string msg) { text.text = msg; }

	// Score updating
	public void UpdateScore(float t) {
		score += t;
		UpdateText(curScoreText, "Score: " + score.ToString("F1"));
		if(score > highScore) {
			highScore = score;
			UpdateText(highScoreText, "High Score: " + highScore.ToString("F1"));
		}
	}

	public void UpdateMorality() {
		UpdateText(moralityCheckText, "Morality Check: " + --moralityCheck);
		if(moralityCheck <= 0) { ActivateDeath(); }
	}

	public void ActivateDeath() {
		StopGame();
		background.CanScroll(false);
		while(fireList.Count > 0) { 
			Destroy(fireList[0].gameObject);
			fireList.RemoveAt(0);
		}
		while(NPCList.Count > 0) { 
			Destroy(NPCList[0].gameObject);
			NPCList.RemoveAt(0);
		}
		while(NPCSelectList.Count > 0) {
			NPCSelectList.RemoveAt(0);
		}
		mainCharacter.ActivateDeath();
	}

	// Add/remove fires + NPCs
	public void AddFireScript(FireScript f) { fireList.Add(f); }
	public void RemoveFireScript(FireScript f) { fireList.Remove(f); }
	public void AddNPCScript(NPCScript n) { NPCList.Add(n); }
	public void RemoveNPCScript(NPCScript n) { NPCList.Remove(n); }
	public bool AddSelection(NPCScript n) { 
		if (curCoroutine != GameCoroutineType.SlowSpeedUpdate) { return false; }
		NPCSelectList.Add(n); return true;
	}
	public bool RemoveSelection(NPCScript n) { NPCSelectList.Remove(n); return true; }

	// Game stop/start facilitation
	public void StartGame() {
		PlayMusic(0, musicVolume);
		PlayMusic(1, 0);
		StopMusic(2);
		StopMusic(6);

		background.CanScroll(true);
		score = 0f;
		moralityCheck = maxMoralityCheck;
		UpdateText(moralityCheckText, "Morality Check: " + moralityCheck);
		float wait = Random.Range(minWaitTime, maxWaitTime);
		fireSpawn.StartDelay(wait);
		NPCSpawn.StartDelay(wait+NPCDelay);
		StartCoroutine(NormSpeedUpdate());
	}

	public void ResetValues() {
		background.SetScrollRate(0);
		mainCharacter.SetRate(0);
		grayOverlay.SetAlpha(0);
		mPlayer[0].volume = 1f;
		mPlayer[1].volume = 0f;
	}

	public void StopGame() {
		if(curCoroutine != GameCoroutineType.NormSpeedUpdate) { ResetValues(); }
		curCoroutine = GameCoroutineType.Inactive;
		StopMusic(0);
		StopMusic(1);
		PlayerPrefs.SetFloat("highScore", highScore);
	}

	public void StartTransition() {
		StartCoroutine(InBetweenUpdate());
	}

	// Coroutines for update handling.
	IEnumerator NormSpeedUpdate() {
		curCoroutine = GameCoroutineType.NormSpeedUpdate;
		while(curCoroutine == GameCoroutineType.NormSpeedUpdate) {
			UpdateScore(Time.deltaTime);
			yield return null;
		}
	}

	IEnumerator SlowSpeedUpdate(float timeLeft) {
		curCoroutine = GameCoroutineType.SlowSpeedUpdate;
		while(curCoroutine == GameCoroutineType.SlowSpeedUpdate && timeLeft > 0) {
			timeLeft -= Time.deltaTime;
			UpdateText(timeLeftText, timeLeft.ToString("F1"));
			if(NPCSelectList.Count == NPCList.Count) { break; }
			yield return null;
		}
		//mainCharacter.SavePeople(NPCSelectList, timeLeft * NPCList[0].GetMoveRate() / NPCList[0].moveNormRate, NPCBetweenTime);
		for(int i=0;i<NPCSelectList.Count;i++) {
			if(NPCSelectList[i] == NPCList[i]) { NPCList[i].GetPlatform().PlacePlatform(); }
		}
		StartCoroutine(InBetweenUpdate());
	}

	IEnumerator InBetweenUpdate() {
		float prog = 0f;
		if(curCoroutine == GameCoroutineType.NormSpeedUpdate) {
			curCoroutine = GameCoroutineType.InBetweenUpdate;
			PlayMusic(3, mPlayer[3].volume);
			while(prog < 1f && curCoroutine != GameCoroutineType.Inactive) {
				prog += Time.deltaTime / inBetweenTransition;
				mPlayer[0].volume = (1f - prog) * musicVolume;
				mPlayer[1].volume = prog * musicVolume;
				background.SetScrollRate(prog);
				mainCharacter.SetRate(prog);
				grayOverlay.SetAlpha(prog);
				for(int i=0;i<fireList.Count;i++) { fireList[i].SetMoveRate(prog); }
				for(int i=0;i<NPCList.Count;i++) { 
					NPCList[i].SetMoveRate(prog); 
					NPCList[i].GetPlatform().SetMoveRate(prog);
				}
				yield return null;
			}
			StartCoroutine(SlowSpeedUpdate(NPCList[0].getTimeLeft() - NPCBetweenTime * 2));
		} else {
			curCoroutine = GameCoroutineType.InBetweenUpdate;
			for(int i=0;i<NPCSelectList.Count;i++) { NPCSelectList[i].ResetColor(); }
			UpdateText(timeLeftText, "");
			PlayMusic(4, mPlayer[4].volume);
			while(prog < 1f && curCoroutine != GameCoroutineType.Inactive) {
				prog += Time.deltaTime / inBetweenTransition;
				mPlayer[1].volume = (1f - prog) * musicVolume;
				mPlayer[0].volume = prog * musicVolume;
				background.SetScrollRate(1f - prog);
				mainCharacter.SetRate(1f - prog);
				grayOverlay.SetAlpha(1f - prog);
				for(int i=0;i<fireList.Count;i++) { fireList[i].SetMoveRate(1f - prog); }
				for(int i=0;i<NPCList.Count;i++) { 
					NPCList[i].SetMoveRate(1f - prog); 
					NPCList[i].GetPlatform().SetMoveRate(1f - prog);
				}
				yield return null;
			}
			float wait = Random.Range(minWaitTime, maxWaitTime);
			fireSpawn.StartDelay(wait);
			NPCSpawn.StartDelay(wait+NPCDelay);
			if(curCoroutine != GameCoroutineType.Inactive) { StartCoroutine(NormSpeedUpdate()); }
		}
	}

	public void MainMenu() {
		background.CanScroll(true);
		highScore = PlayerPrefs.GetFloat("highScore");
		UpdateText(highScoreText, "High Score: " + highScore.ToString("F1"));
		PlayMusic(2, musicVolume);
		StopMusic(6);
	}

	// Initialization on start.
	void Start() {
		MainMenu ();
	}
	
}
