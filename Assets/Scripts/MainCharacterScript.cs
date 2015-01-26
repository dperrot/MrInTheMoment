using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MainCharacterScript : MonoBehaviour {

	public float normRate;
	public float slowRate;
	public float minDeathY;
	public float initVelocity;
	public float acceleration;
	public Sprite deathSprite;
	public List<Sprite> spriteList;

	private float totalRate = 0f;
	private int sI = 0;
	private float rate;
	private Image img;
	private bool isAlive = true;
	private RectTransform objTrans;
	private Vector2 original;
	private Vector2 tempVec = Vector2.zero;

	// Use this for initialization
	void Start () {
		objTrans = (RectTransform) gameObject.transform;
		img = gameObject.GetComponent<Image>();
		rate = normRate;
		original = objTrans.anchoredPosition;
	}
	
	// Update is called once per frame
	void Update () {
		if (isAlive) {
			totalRate += Time.deltaTime * rate;
			if(totalRate >= 1.0f) {
				sI = (sI+1) % spriteList.Count;
				img.sprite = spriteList[sI];
				totalRate = 0f;
			}
		}
	}

	public void ActivateDeath() {
		isAlive = false;
		img.sprite = deathSprite;
		StartCoroutine(DeathSequence());
	}

	IEnumerator DeathSequence() {
		yield return new WaitForSeconds(.5f);
		yield return null;
		GameManager.Instance().PlayMusic(6, GameManager.Instance().musicVolume);
		tempVec.y = initVelocity * Time.deltaTime;
		while(objTrans.anchoredPosition.y > minDeathY) {
			objTrans.anchoredPosition = objTrans.anchoredPosition + tempVec;
			yield return null;
			tempVec.y += acceleration * Time.deltaTime;
		}
		WindowManager.Instance().SetWindow(WindowTypes.DeathWindow);
	}

	public void ResetChar() {
		objTrans.anchoredPosition = original;
		isAlive = true;
		img.sprite = spriteList[sI];
	}

	//public void SavePeople(List<NPCScript> npcs, float tBuffer, float tInterval) {
	//	StartCoroutine(SavePeopleEnum(npcs, tBuffer, tInterval));
	//}

	//IEnumerator SavePeopleEnum(List<NPCScript> npcs, float tBuffer, float tInterval) {
	//	float mRate = npcs[0].GetMoveRate();
	//	float mNormRate = npcs[0].moveNormRate;
	//	for(int i=0;i<npcs.Count;i++) {
	//		yield return StartCoroutine(WalkToNPCSpot(npcs[i], tBuffer+tInterval));
	//		tBuffer = 0;
	//	}
	//	StartCoroutine(WalkToOrigin(tInterval, mRate, mNormRate));
	//}

	//IEnumerator WalkToNPCSpot(NPCScript npc, float tAdjust) {
	//	if(!npc.CanBeSaved()) { yield break; }
	//	float t = 0f;
	//	NPCPlatformScript npcPlat = npc.GetPlatform();
	//	while(t < 1f) {
	//		yield return null;
	//		t += Time.deltaTime / tAdjust * (npcPlat.GetMoveRate() / npcPlat.moveNormRate);
	//		objTrans.anchoredPosition = Vector2.Lerp(original, original - (npcPlat.GetPos() - original), t);
	//	}
	//	npcPlat.PlacePlatform();
	//}

	//IEnumerator WalkToOrigin(float tAdjust, float mRate, float mNormRate) {
	//	Vector2 lastPos = objTrans.anchoredPosition;
	//	float t = 0f;
	//	while(t < 1f) {
	//		yield return null;
	//		t += Time.deltaTime / tAdjust * mRate / mNormRate;
	//		objTrans.anchoredPosition = Vector2.Lerp (lastPos, original, t);
	//	}
	//}

	public void SetRate(float lerp) {
		rate = Mathf.Lerp(normRate, slowRate, lerp);
	}
}

