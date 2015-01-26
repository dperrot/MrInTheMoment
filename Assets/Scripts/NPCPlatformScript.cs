using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NPCPlatformScript : MonoBehaviour {

	public float moveNormRate;
	public float moveSlowRate;
	public float killPos;
	public float framerate;
	public List<Sprite> spriteList;

	private float moveRate;
	private Vector2 tempVec = Vector2.zero;
	private RectTransform rTransform;
	private NPCScript NPCParent;
	private Image img;
	private Color white = new Color(1,1,1,1);

	// Use this for initialization
	void Start () {
		rTransform = ((RectTransform) transform);
		moveRate = moveNormRate;
		img = gameObject.GetComponent<Image>();
	}

	void Update () {
		tempVec.x = Time.deltaTime * moveRate;
		rTransform.anchoredPosition -= tempVec;
		if(rTransform.anchoredPosition.x <= killPos) { 
			Destroy(gameObject);
		}
	}

	public void SetNPCParent(NPCScript npc) { NPCParent = npc; }

	public void PlacePlatform() {
		if(NPCParent.CanBeSaved()) {
			NPCParent.IsSaved();
			StartCoroutine(PlatformPlace());
		}
	}

	IEnumerator PlatformPlace() {
		for(int i=0;i<spriteList.Count;i++) {
			img.sprite = spriteList[i];
			img.color = white;
			float t = 0f;
			while (t < 1f) {
				yield return null;
				t += Time.deltaTime * framerate;
			}
		}
	}

	public void SetMoveRate(float lerp) {
		moveRate = Mathf.Lerp(moveNormRate, moveSlowRate, lerp);
	}

	public Vector2 GetPos() { return rTransform.anchoredPosition; }

	public float GetMoveRate() { return moveRate; }
}
