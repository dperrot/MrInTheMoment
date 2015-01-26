using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NPCScript : MonoBehaviour {

	public GameObject NPCPlatform;
	public float moveNormRate;
	public float moveSlowRate;
	public float heightRand;
	public float horizontalRand;
	public float groundHeightRand;
	public float groundLevel;
	public float killPos;
	public float transitionTime;
	public Color normalColor;
	public Color selectColor;
	public List<Sprite> spriteList = new List<Sprite>();
	public List<int> spriteIntList = new List<int>();

	private float timeLimit;
	private float timeStart;
	private float moveRate;
	private float height;
	private float horizontal;
	private float groundHeight;
	private float updateLerp = 0;
	private int spriteId;
	private Vector2 origin = Vector2.zero;
	private Vector2 offset = Vector2.zero;
	private Vector2 tVert  = Vector2.zero;
	private RectTransform thisTransform;
	private bool canSurvive = false;
	private bool isDown = false;
	private bool isSelected = false;
	private Image img;
	private int id;
	private NPCPlatformScript platform;

	// Use this for initialization
	public void InitNPC(int i, float limit) {
		spriteId = spriteIntList[Random.Range(0, spriteIntList.Count-1)];
		img = gameObject.GetComponent<Image>();
		img.sprite = spriteList[spriteId+3];
		img.color = normalColor;
		moveRate = moveNormRate;
		id = i;
		timeLimit = limit + i * GameManager.Instance().NPCBetweenTime;
		timeStart = timeLimit;
		GameManager.Instance().AddNPCScript(this);
		height = Random.Range(0f, heightRand);
		horizontal = Random.Range(horizontalRand/2f, horizontalRand);
		horizontal *= (Random.value - 0.5f < 0 ? -1 : 1);
		groundHeight = groundLevel + Random.Range(-groundHeightRand, groundHeightRand);

		tVert.x = horizontal;
		tVert.y = groundHeight;
		GameObject obj = Instantiate(NPCPlatform) as GameObject;
		obj.transform.SetParent(transform.parent);
		((RectTransform)obj.transform).anchoredPosition = tVert;
		platform = obj.GetComponent<NPCPlatformScript>();
		platform.SetNPCParent(this);

		thisTransform = (RectTransform) gameObject.transform;
		StartCoroutine(NPCUpdate());
	}

	public int GetID() { return id; }

	IEnumerator NPCUpdate() {
		while(timeLimit > 0) {
			timeLimit -= Time.deltaTime * moveRate / moveNormRate;
			origin.x -= Time.deltaTime * moveRate;
			if(origin.x <= killPos) {
				GameManager.Instance().RemoveNPCScript(this);
				Destroy(gameObject);
			}
			offset.x = Mathf.Lerp(0f, horizontal, updateLerp);
			if(updateLerp < 0.25f) {
				offset.y = Mathf.Lerp(0f, height, updateLerp * 2);
			} else if(updateLerp < 0.5f) {
				offset.y = Mathf.SmoothStep(0f, height, updateLerp * 2);
			} else if(updateLerp < 0.75f) {
				offset.y = Mathf.SmoothStep(height, groundHeight, -1f + updateLerp * 2);
			} else {
				offset.y = Mathf.Lerp(height, groundHeight, -1f + updateLerp * 2);
			}
			thisTransform.anchoredPosition = origin + offset;
			updateLerp = 1 - timeLimit / timeStart;
			yield return null;
		}

		if(canSurvive) {
			img.sprite = spriteList[spriteId];
			GameManager.Instance().PlayMusic(7, 1);
		} else {
			isDown = true;
			GameManager.Instance().UpdateMorality();
			img.sprite = spriteList[spriteId+4];
			GameManager.Instance().PlayMusic(5, 1);
		}
		Destroy(platform.gameObject);

		updateLerp = 0f;
		while(true) {
			if(img.sprite == spriteList[spriteId+4] && updateLerp > 1f) { 
				img.sprite = spriteList[spriteId+2]; 
			} else if(img.sprite == spriteList[spriteId] && updateLerp > 1f) {
				img.sprite = spriteList[spriteId+1];
			}
			updateLerp += Time.deltaTime / transitionTime;
			origin.x -= Time.deltaTime * moveRate;
			thisTransform.anchoredPosition = origin + offset;
			if(origin.x <= killPos) {
				GameManager.Instance().RemoveNPCScript(this);
				GameManager.Instance().RemoveSelection(this);
				Destroy(gameObject);
			}
			yield return null;
		}
	}

	public float getTimeLeft() { return timeLimit * moveNormRate / moveSlowRate; }

	public void SetMoveRate(float lerp) {
		moveRate = Mathf.Lerp(moveNormRate, moveSlowRate, lerp);
	}

	public void OnSelected() {
		if(isSelected && GameManager.Instance().RemoveSelection(this)) {
			img.color = normalColor;
			isSelected = false;
		} else if (GameManager.Instance().AddSelection(this)) {
			img.color = selectColor;
			isSelected = true;
		}
	}

	public void ResetColor() { img.color = normalColor; }

	public bool CanBeSaved() { return !isDown; }

	public NPCPlatformScript GetPlatform() { return platform; }

	public void IsSaved() { canSurvive = true; }

	public float GetMoveRate() { return moveRate; }
}
