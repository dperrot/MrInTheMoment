using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FireScript : MonoBehaviour {

	public float moveNormRate;
	public float moveSlowRate;
	public float killPos;
	public Vector2 sizeRand;
	public List<Sprite> spriteList;

	private float moveRate;
	private Vector2 tempVec;
	private RectTransform rTransform;

	// Use this for initialization
	void Start () {
		rTransform = ((RectTransform) transform);
		moveRate = moveNormRate;
		gameObject.GetComponent<Image>().sprite = spriteList[Random.Range(0,spriteList.Count-1)];
		tempVec = new Vector2(Random.Range(0f, sizeRand.x), Random.Range(0f, sizeRand.y));
		rTransform.sizeDelta += tempVec;
		tempVec = Vector2.zero;
		GameManager.Instance().AddFireScript(this);
	}
	
	// Update is called once per frame
	void Update () {
		tempVec.x = Time.deltaTime * moveRate;
		rTransform.anchoredPosition -= tempVec;
		if(rTransform.anchoredPosition.x <= killPos) { 
			Destroy(gameObject);
			GameManager.Instance().RemoveFireScript(this);
		}
	}

	public void SetMoveRate(float lerp) {
		moveRate = Mathf.Lerp(moveNormRate, moveSlowRate, lerp);
	}
}
